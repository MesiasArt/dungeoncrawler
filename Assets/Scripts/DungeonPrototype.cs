using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using VirtualDungeon;

// The first playable slice. Launch any scene to explore the prototype.
public class DungeonPrototype : MonoBehaviour
{
    const float Cell = 3f;
    readonly string[] map = {
        "###########", "#.........#", "#.###.###.#", "#...#...#.#",
        "###.#.#.#.#", "#...#.#...#", "#.###.###.#", "#.........#", "#########.#" };
    readonly List<Vector2Int> enemies = new List<Vector2Int>();
    readonly List<GameObject> enemyObjects = new List<GameObject>();
    Vector2Int position = new Vector2Int(1,1);
    readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    int heading, hp = 30, enemyHP, activeEnemy = -1, victories;
    bool moving, battle, busy, lost;
    int selectedCombatAction;
    enum CombatMenu { Main, Hacks, Items }
    CombatMenu combatMenu;
    CombatHack[] availableHacks;
    public CombatInventorySettings inventorySettings;
    [SerializeField] int[] itemCounts;
    int MaximumHP { get { return inventorySettings==null ? 30 : Mathf.Max(1,inventorySettings.maximumHP); } }
    int TotalItems { get { int total=0; if(itemCounts!=null) foreach(int count in itemCounts) total+=Mathf.Max(0,count); return total; } }
    bool[,] explored;
    DungeonDoor exitDoor;
    public DungeonDoorSettings exitDoorSettings;
    bool levelComplete, showMainMenu;
    float levelFade;
    int completionSelection;
    Camera view;
    string message = "Explora. Los enemigos están en el mapa: puedes evitarlos.";
    float glitch;
    Material wall, floor, ceiling, pink, blue;
    Texture2D floorWallDistance;
    IntroSequence introduction;
    [SerializeField] CombatHack combatHack;
    InputAction interact, submit;
    bool enabledInteract, enabledSubmit, introductionCompleted;
    bool IntroductionActive { get { return introduction != null && introduction.Active; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetGameStart() { GameStartContext.PrepareNewGame(); }

    void StartIntroduction()
    {
        introduction = new IntroSequence(GameStartContext.Consume());
        if (!introduction.Active) return;
        interact = InputSystem.actions?.FindAction("Player/Interact");
        submit = InputSystem.actions?.FindAction("UI/Submit");
        enabledInteract = interact != null && !interact.enabled;
        enabledSubmit = submit != null && !submit.enabled;
        if (enabledInteract) interact.Enable();
        if (enabledSubmit) submit.Enable();
    }
    bool ConfirmPressed()
    {
        return (Keyboard.current!=null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Gamepad.current!=null && Gamepad.current.buttonSouth.wasPressedThisFrame);
    }
    void ProcessIntroductionInput(bool advance, bool skip, float elapsed)
    {
        if(!IntroductionActive) return;
        if(skip) introduction.Skip();
        else introduction.Tick(elapsed,advance);
        if(introduction.Interference) glitch=.1f;
        if(!introduction.Active) { introductionCompleted=true; glitch=0; ReleaseIntroductionInput(); }
    }
    void HandleCombatInput(int direction, bool confirm)
    {
        if(!battle || busy || lost || IntroductionActive || (combatHack!=null && combatHack.Active)) return;
        if(direction!=0) { int count=CombatLabels().Length; selectedCombatAction=(selectedCombatAction+(direction>0 ? 1 : -1)+count)%count; return; }
        if(confirm) Action(selectedCombatAction);
    }
    void ReleaseIntroductionInput()
    {
        if (enabledInteract) interact.Disable();
        if (enabledSubmit) submit.Disable();
        enabledInteract = enabledSubmit = false;
    }
    void OnDisable() { ReleaseIntroductionInput(); }
    void OnDestroy() { if (floorWallDistance != null) Destroy(floorWallDistance); }
    void RestartNewGame()
    {
        GameStartContext.PrepareNewGame();
        var inventory=inventorySettings;var door=exitDoorSettings;
        Destroy(gameObject);
        var fresh=new GameObject("Dungeon Prototype").AddComponent<DungeonPrototype>();
        fresh.inventorySettings=inventory;fresh.exitDoorSettings=door;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterSceneLaunch()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Launch();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Launch()
    {
        if (FindAnyObjectByType<DungeonPrototype>() == null)
            new GameObject("Dungeon Prototype").AddComponent<DungeonPrototype>();
    }
    void Start()
    {
        explored=new bool[map[0].Length,map.Length];
        RevealVisitedCell(position);
        foreach (Camera camera in FindObjectsByType<Camera>()) camera.gameObject.SetActive(false);
        foreach (Light light in FindObjectsByType<Light>()) light.enabled = false;
        wall = Surface("Textures/textura azul luffy2", .55f, .8f);
        wall.SetFloat("_ContinuousWallGrid",1);
        wall.SetVector("_FloorGridRegion",new Vector4(266f/1254f,260f/1254f,144f/1254f,144f/1254f));
        wall.SetFloat("_FloorGridSize",Cell/9f);
        floor = Surface("Textures/textura rosa luffy2", .55f, .7f);
        // Repeat one complete grid cell from the artwork in world coordinates.
        // Its edges meet on the neon line centers, independently of floor blocks.
        floor.SetFloat("_ContinuousFloorGrid",1);
        floor.SetVector("_FloorGridRegion",new Vector4(266f/1254f,850f/1254f,146f/1254f,143f/1254f));
        floor.SetFloat("_FloorGridSize",Cell/9f);
        ceiling = Surface("Textures/textura rosa luffy2", .35f, .6f);
        ConfigureFloorSheen();
        pink = Glow(new Color(1,.015f,.5f)); blue = Glow(new Color(.05f,.45f,1));
        for (int z=0; z<map.Length; z++) for(int x=0;x<map[z].Length;x++)
        {
            Vector3 p = new Vector3(x*Cell,0,z*Cell);
            if (map[z][x]=='#') { Box("Wall",p+Vector3.up*1.5f,new Vector3(Cell,3,Cell),wall); continue; }
            Box("Floor",p+Vector3.down*.1f,new Vector3(Cell,.2f,Cell),floor);
            Box("Ceiling",p+Vector3.up*3.1f,new Vector3(Cell,.2f,Cell),ceiling);
        }
        AddEnemy(new Vector2Int(5,3)); AddEnemy(new Vector2Int(7,7)); AddEnemy(new Vector2Int(1,5));
        CreateExitDoor();
        view = new GameObject("Player Camera").AddComponent<Camera>();
        view.transform.SetParent(transform,false);
        view.fieldOfView = 68; view.nearClipPlane = .05f;
        view.backgroundColor = new Color(.005f,.003f,.02f); view.clearFlags = CameraClearFlags.SolidColor;
        view.transform.position = World(position); view.transform.rotation = Quaternion.Euler(0,heading*90,0);
        view.gameObject.AddComponent<AudioListener>();
        view.gameObject.AddComponent<CombatImpactFeedback>();
        view.gameObject.AddComponent<DungeonCorridorLighting>();
        RenderSettings.ambientLight = new Color(.22f,.16f,.36f);
        RenderSettings.fog = true; RenderSettings.fogColor = view.backgroundColor; RenderSettings.fogDensity = .035f;
        Light lamp = new GameObject("Player light").AddComponent<Light>();
        lamp.transform.SetParent(view.transform,false); lamp.type=LightType.Point; lamp.range=12; lamp.intensity=2; lamp.color=new Color(.45f,.4f,1);
        lamp.shadows = LightShadows.Hard;
        lamp.shadowBias = .03f; lamp.shadowNormalBias = .15f;
        Light key = new GameObject("Anime key light").AddComponent<Light>();
        key.transform.SetParent(transform, false);
        key.type = LightType.Directional; key.color = new Color(.7f,.78f,1);
        key.intensity = .35f; key.shadows = LightShadows.Hard;
        key.transform.rotation = Quaternion.Euler(35,-30,0);
        view.gameObject.AddComponent<DungeonFloorReflection>().Initialize(view, floor);
        view.gameObject.AddComponent<DungeonFloorReflection>().Initialize(view, ceiling, 3f);
        combatHack = GetComponent<EnemyJumpHack>();
        if(combatHack==null) combatHack=gameObject.AddComponent<EnemyJumpHack>();
        inventorySettings=inventorySettings!=null ? inventorySettings : Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");
        if(inventorySettings==null) inventorySettings=ScriptableObject.CreateInstance<CombatInventorySettings>();
        itemCounts=new int[inventorySettings.items.Length];
        for(int i=0;i<itemCounts.Length;i++) itemCounts[i]=inventorySettings.items[i]==null ? 0 : Mathf.Max(0,inventorySettings.items[i].startingQuantity);
        hp=Mathf.Min(hp,MaximumHP);
        StartIntroduction();
    }
    void RevealVisitedCell(Vector2Int cell)
    {
        explored[cell.x,cell.y]=true;
        foreach(Vector2Int direction in directions)
        {
            Vector2Int adjacent=cell+direction;
            if(adjacent.x>=0&&adjacent.x<map[0].Length&&adjacent.y>=0&&adjacent.y<map.Length&&map[adjacent.y][adjacent.x]=='#')
                explored[adjacent.x,adjacent.y]=true;
        }
    }
    void CreateExitDoor()
    {
        var root=new GameObject("Level exit door");root.transform.SetParent(transform,false);
        root.transform.position=new Vector3(9*Cell,0,7.5f*Cell);
        exitDoor=root.AddComponent<DungeonDoor>();
        exitDoorSettings=exitDoorSettings!=null ? exitDoorSettings : Resources.Load<DungeonDoorSettings>("Exploration/ExitDoorSettings");
        exitDoor.settings=exitDoorSettings;
        var panel=Box("Exit door panel",root.transform.position+Vector3.up*1.5f,new Vector3(2.85f,3,.18f),wall);
        panel.transform.SetParent(root.transform,true);exitDoor.panel=panel.transform;exitDoor.blocker=panel.GetComponent<Collider>();
        for(int side=-1;side<=1;side+=2)
        {
            var frame=Box("Exit door frame",root.transform.position+new Vector3(side*1.46f,1.5f,0),new Vector3(.08f,3,.22f),blue);frame.transform.SetParent(root.transform,true);
        }
        var stripe=Box("Exit door neon",root.transform.position+new Vector3(0,1.4f,-.11f),new Vector3(2.7f,.06f,.025f),pink);stripe.transform.SetParent(panel.transform,true);
        exitDoor.Initialize();exitDoor.Passed+=OnExitPassed;
    }
    void OnExitPassed()
    { if(exitDoor.settings.completesLevel && !levelComplete) StartCoroutine(CompleteLevel()); }
    IEnumerator CompleteLevel()
    {
        levelComplete=true;levelFade=0;completionSelection=0;glitch=0;
        float duration=Mathf.Max(.1f,exitDoor.settings.fadeDuration);
        while(levelFade<1) { levelFade=Mathf.Min(1,levelFade+Time.unscaledDeltaTime/duration);yield return null; }
    }
    void UpdateCompletionInput()
    {
        if(Keyboard.current==null || levelFade<1) return;
        var keys=Keyboard.current;
        if(!showMainMenu && (keys.upArrowKey.wasPressedThisFrame || keys.downArrowKey.wasPressedThisFrame)) completionSelection=1-completionSelection;
        if(keys.enterKey.wasPressedThisFrame || keys.spaceKey.wasPressedThisFrame)
        { if(showMainMenu || completionSelection==0) RestartNewGame();else showMainMenu=true; }
    }
    void DrawCompletion(float width,float height)
    {
        GUI.enabled=true;GUI.color=new Color(0,0,0,levelFade);GUI.DrawTexture(new Rect(0,0,width,height),Texture2D.whiteTexture);GUI.color=Color.white;
        if(levelFade<1) return;
        GUI.contentColor=Color.white;GUI.backgroundColor=new Color(.12f,.05f,.3f,1);
        var title=new GUIStyle(GUI.skin.label){fontSize=34,alignment=TextAnchor.MiddleCenter};
        GUI.Label(new Rect(0,height*.35f,width,70),showMainMenu ? "VIRTUAL DUNGEON" : "NIVEL COMPLETADO",title);
        if(showMainMenu) { if(GUI.Button(new Rect(width/2-140,height*.55f,280,50),"JUGAR")) RestartNewGame(); }
        else
        {
            if(GUI.Button(new Rect(width/2-150,height*.55f,300,50),completionSelection==0 ? "> Reintentar <" : "Reintentar")) RestartNewGame();
            if(GUI.Button(new Rect(width/2-150,height*.55f+65,300,50),completionSelection==1 ? "> Volver al menú <" : "Volver al menú")) showMainMenu=true;
        }
    }
    void ConfigureFloorSheen()
    {
        int width = map[0].Length * 4, height = map.Length * 4;
        floorWallDistance = new Texture2D(width,height,TextureFormat.RFloat,false,true);
        floorWallDistance.name = "Floor distance to walls";
        floorWallDistance.filterMode = FilterMode.Bilinear;
        floorWallDistance.wrapMode = TextureWrapMode.Clamp;
        // Sample distance to the actual wall rectangles, so wider rooms fade too.
        for (int z=0;z<height;z++) for (int x=0;x<width;x++)
        {
            float px = (x+.5f)*Cell/4-Cell*.5f, pz = (z+.5f)*Cell/4-Cell*.5f;
            float distance = float.MaxValue;
            for (int wz=0;wz<map.Length;wz++) for (int wx=0;wx<map[wz].Length;wx++)
            {
                if (map[wz][wx]!='#') continue;
                float dx = Mathf.Max(Mathf.Abs(px-wx*Cell)-Cell*.5f,0);
                float dz = Mathf.Max(Mathf.Abs(pz-wz*Cell)-Cell*.5f,0);
                distance = Mathf.Min(distance,Mathf.Sqrt(dx*dx+dz*dz));
            }
            floorWallDistance.SetPixel(x,z,new Color(distance,0,0,1));
        }
        floorWallDistance.Apply(false,true);
        ceiling.SetTexture("_FloorWallDistance",floorWallDistance);
        ceiling.SetVector("_FloorDistanceBounds",new Vector4(-Cell*.5f,-Cell*.5f,map[0].Length*Cell,map.Length*Cell));
        floor.SetTexture("_FloorWallDistance",floorWallDistance);
        floor.SetVector("_FloorDistanceBounds",new Vector4(-Cell*.5f,-Cell*.5f,map[0].Length*Cell,map.Length*Cell));
    }
    Material Surface(string path, float metallic, float smoothness)
    {
        Shader surfaceShader = Resources.Load<Shader>("Shaders/DungeonMetalToon");
        if (surfaceShader == null) surfaceShader = Shader.Find("Universal Render Pipeline/Lit");
        Material m = new Material(surfaceShader);
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture == null) Debug.LogError("No se encontró la textura: " + path, this);
        m.SetTexture("_BaseMap",texture); m.SetColor("_BaseColor",Color.white);
        // Preserve the artwork's colors and keep its neon visible in dark corridors.
        if (m.HasProperty("_EmissionMap"))
        {
            m.SetTexture("_EmissionMap",texture);
            m.SetColor("_EmissionColor",Color.white * .15f);
            m.EnableKeyword("_EMISSION");
        }
        m.SetFloat("_Metallic",metallic); m.SetFloat("_Smoothness",smoothness);
        if (m.HasProperty("_EmissionStrength")) m.SetFloat("_EmissionStrength",.15f);
        return m;
    }
    Material Glow(Color color)
    {
        Material m = new Material(Resources.Load<Shader>("Shaders/DungeonNeon")); m.SetColor("_BaseColor",color); return m;
    }
    GameObject Box(string name,Vector3 p,Vector3 size,Material material)
    {
        GameObject o=GameObject.CreatePrimitive(PrimitiveType.Cube); o.name=name; o.transform.SetParent(transform);
        o.transform.position=p; o.transform.localScale=size; o.GetComponent<Renderer>().sharedMaterial=material; return o;
    }
    void AddEnemy(Vector2Int cell)
    {
        enemies.Add(cell);
        GameObject root = new GameObject("Sentinela"); root.transform.position = World(cell)-Vector3.up*1.5f; root.transform.SetParent(transform);
        GameObject body=Box("Enemy body",root.transform.position+Vector3.up*.9f,new Vector3(.7f,1.1f,.4f),pink); body.transform.SetParent(root.transform);
        GameObject head=Box("Enemy visor",root.transform.position+Vector3.up*1.6f,new Vector3(.5f,.35f,.45f),blue); head.transform.SetParent(root.transform);
        enemyObjects.Add(root);
    }
    bool Open(Vector2Int p) { return p.y>=0&&p.y<map.Length&&p.x>=0&&p.x<map[0].Length&&map[p.y][p.x]!='#'; }
    Vector3 World(Vector2Int p) { return new Vector3(p.x*Cell,1.5f,p.y*Cell); }
    void Update()
    {
        glitch=Mathf.Max(0,glitch-Time.deltaTime);
        if(view==null) return;
        if(levelComplete) { UpdateCompletionInput();return; }
        foreach(GameObject enemy in enemyObjects) if(enemy!=null) enemy.transform.LookAt(new Vector3(view.transform.position.x,enemy.transform.position.y,view.transform.position.z));
        if (IntroductionActive)
        {
            bool skip=Keyboard.current!=null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame);
            ProcessIntroductionInput(ConfirmPressed(),skip,Time.unscaledDeltaTime);
            return; // The last confirmation cannot also move or act in the same frame.
        }
        if(battle)
        {
            var keys=Keyboard.current;
            if(keys!=null)
            {
                if(keys.escapeKey.wasPressedThisFrame || keys.backspaceKey.wasPressedThisFrame) { BackToCombatMenu(); return; }
                int direction=(keys.downArrowKey.wasPressedThisFrame || keys.rightArrowKey.wasPressedThisFrame) ? 1 :
                    ((keys.upArrowKey.wasPressedThisFrame || keys.leftArrowKey.wasPressedThisFrame) ? -1 : 0);
                HandleCombatInput(direction,keys.enterKey.wasPressedThisFrame || keys.numpadEnterKey.wasPressedThisFrame || keys.spaceKey.wasPressedThisFrame);
            }
            return;
        }
        if(moving||lost||Keyboard.current==null) return;
        var k=Keyboard.current;
        if(exitDoor!=null && exitDoor.CanInteract(view.transform.position) && k.eKey.wasPressedThisFrame) { exitDoor.OpenDoor();return; }
        if(k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,-1));
        else if(k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,1));
        else if(k.wKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame) StartCoroutine(Move(1,0));
        else if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame) StartCoroutine(Move(-1,0));
    }
    IEnumerator Move(int step,int turn)
    {
        if (IntroductionActive || levelComplete) yield break;
        Vector2Int next=position+directions[heading]*step;
        if(step!=0&&!Open(next)) yield break;
        if(step!=0 && exitDoor!=null && exitDoor.Blocks(World(position),World(next))) { message="[E] Abrir puerta";yield break; }
        moving=true; heading=(heading+turn+4)%4;
        Vector3 from=view.transform.position,to=World(next); Quaternion rotation=view.transform.rotation,target=Quaternion.Euler(0,heading*90,0);
        for(float t=0;t<1;t+=Time.deltaTime/.18f) { view.transform.position=Vector3.Lerp(from,to,t); view.transform.rotation=Quaternion.Slerp(rotation,target,t); yield return null; }
        position=next; RevealVisitedCell(position); view.transform.position=to; view.transform.rotation=target; moving=false;
        if(exitDoor!=null) exitDoor.CheckPassage(from,to);
        if(levelComplete) yield break;
        for(int i=0;i<enemies.Count;i++) if(enemyObjects[i]!=null&&(enemies[i]-position).sqrMagnitude<=1)
        { activeEnemy=i; enemyHP=20; battle=true; combatMenu=CombatMenu.Main; selectedCombatAction=0; view.transform.LookAt(World(enemies[i])); glitch=.45f; message="¡Apareció un sentinela! Elige una acción."; break; }
    }
    string[] CombatLabels()
    {
        if(combatMenu==CombatMenu.Main) return new[]{"ATTACK","HACK","ITEM","HUIR"};
        if(combatMenu==CombatMenu.Hacks)
        {
            int count=availableHacks==null ? 0 : availableHacks.Length;
            var labels=new string[count+1];
            for(int i=0;i<count;i++) labels[i]=availableHacks[i].DisplayName;
            labels[count]="VOLVER"; return labels;
        }
        var items=inventorySettings.items;
        var itemLabels=new string[items.Length+1];
        for(int i=0;i<items.Length;i++) itemLabels[i]=items[i]==null ? "Objeto no disponible" : items[i].displayName+" x "+Mathf.Max(0,itemCounts[i]);
        itemLabels[items.Length]="VOLVER";return itemLabels;
    }
    void BackToCombatMenu()
    {
        if(busy || lost || (combatHack!=null && combatHack.Active) || combatMenu==CombatMenu.Main) return;
        selectedCombatAction=combatMenu==CombatMenu.Hacks ? 1 : 2;
        combatMenu=CombatMenu.Main;
    }
    void Action(int action)
    {
        if(IntroductionActive || busy || lost || !battle || (combatHack!=null && combatHack.Active)) return;
        if(action<0 || action>=CombatLabels().Length) return;
        if(combatMenu==CombatMenu.Hacks)
        {
            if(action==availableHacks.Length) { BackToCombatMenu(); return; }
            combatHack=availableHacks[action]; combatMenu=CombatMenu.Main; selectedCombatAction=1;
            busy=true;
            combatHack.Begin(damage=>StartCoroutine(ResolveCombatDamage(damage,"HACK")));
            if(!combatHack.Active) busy=false;
            return;
        }
        if(combatMenu==CombatMenu.Items)
        {
            if(action==inventorySettings.items.Length) { BackToCombatMenu(); return; }
            var item=inventorySettings.items[action];
            if(item==null || itemCounts[action]<=0) { message="No quedan unidades de ese objeto."; return; }
            if(hp>=MaximumHP) { message="Ya tienes el HP completo."; return; }
            int healed=Mathf.Min(Mathf.Max(0,item.healHP),MaximumHP-hp);
            itemCounts[action]--; hp+=healed; busy=true;
            message=item.displayName+": recuperas "+healed+" HP.";
            combatMenu=CombatMenu.Main;selectedCombatAction=2;
            StartCoroutine(ResolveCombatDamage(0,"ITEM")); return;
        }
        if(action==0) { busy=true;StartCoroutine(ResolveCombatDamage(7,"ATTACK"));return; }
        if(action==1) { availableHacks=GetComponents<CombatHack>();combatMenu=CombatMenu.Hacks;selectedCombatAction=0;return; }
        if(action==2) { combatMenu=CombatMenu.Items;selectedCombatAction=0;return; }
        battle=false;busy=false;combatMenu=CombatMenu.Main;selectedCombatAction=0;activeEnemy=-1;glitch=0;
        view.transform.rotation=Quaternion.Euler(0,heading*90,0);
        message="Escapaste. Retrocede o busca otra ruta.";
    }
    IEnumerator ResolveCombatDamage(int damage, string actionName)
    {
        enemyHP-=damage;
        if(actionName=="ATTACK" && damage>0) view.GetComponent<CombatImpactFeedback>().Play(false);
        if(actionName!="ITEM") message=actionName+": "+damage+" de daño confirmado.";
        string resultMessage=message;
        yield return new WaitForSeconds(.6f);
        if(enemyHP<=0)
        {
            Destroy(enemyObjects[activeEnemy]); victories++; battle=false;
            view.transform.rotation=Quaternion.Euler(0,heading*90,0);
            message= victories==enemies.Count ? "¡Mazmorra despejada!" : "Victoria. Sigue explorando.";
        }
        else
        {
            hp=Mathf.Max(0,hp-5);
            if(actionName=="ATTACK") view.GetComponent<CombatImpactFeedback>().Play(true);
            message=resultMessage+" El sentinela responde: pierdes 5 HP.";
            if(hp==0) { lost=true; message="Has caído. Pulsa Reiniciar."; }
        }
        busy=false;
    }
    void OnGUI()
    {
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f); GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));
        float width=Screen.width/scale,height=Screen.height/scale;
        GUI.skin.box.fontSize=20; GUI.skin.label.fontSize=18; GUI.skin.button.fontSize=20;
        GUI.backgroundColor=new Color(.12f,.05f,.3f,.95f); GUI.contentColor=new Color(.65f,.85f,1);
        GUI.Box(new Rect(20,20,270,95),"LV 1    EXPLORADOR\nHP "+hp+" / "+MaximumHP+"\nObjetos: "+TotalItems+"   Victorias: "+victories);
        float mx=width-240; GUI.Box(new Rect(mx,20,220,200),"MAPA");
        GUI.color=Color.black;GUI.DrawTexture(new Rect(mx+12,55,187,153),Texture2D.whiteTexture);
        for(int z=0;z<map.Length;z++) for(int x=0;x<map[z].Length;x++)
        {
            if(explored==null || !explored[x,z]) continue;
            GUI.color=map[z][x]=='#'?new Color(.12f,.13f,.32f):new Color(.03f,.03f,.1f);
            GUI.DrawTexture(new Rect(mx+12+x*17,55+(map.Length-1-z)*17,16,16),Texture2D.whiteTexture);
        }
        GUI.color=Color.magenta;
        for(int i=0;i<enemies.Count;i++) if(enemyObjects[i]!=null && explored[enemies[i].x,enemies[i].y]) GUI.DrawTexture(new Rect(mx+16+enemies[i].x*17,59+(map.Length-1-enemies[i].y)*17,8,8),Texture2D.whiteTexture);
        GUI.color=Color.white; GUI.Label(new Rect(mx+12+position.x*17,51+(map.Length-1-position.y)*17,25,25),new[]{"▲","▶","▼","◀"}[heading]);
        // Reuse the existing message rectangle, colors, font and box style.
        if (IntroductionActive)
        {
            IntroLine line = introduction.Current;
            GUI.Box(new Rect(310,height-100,width-330,80), line == null ? "..." : line.Speaker + "\n" + line.Text + "\n[ESPACIO: siguiente / ENTER: saltar intro]");
        }
        else if (!introductionCompleted || battle || lost || victories > 0)
            GUI.Box(new Rect(310,height-100,width-330,80),message);
        if(battle && (combatHack==null || !combatHack.Active))
        {
            GUI.Box(new Rect(width/2-160,25,320,65),"SENTINELA\nHP "+Mathf.Max(0,enemyHP)+" / 20");
            GUI.enabled=!IntroductionActive&&!busy&&!lost;
            string[] labels=CombatLabels();
            for(int i=0;i<labels.Length;i++)
            {
                string caption=i==selectedCombatAction ? "> "+labels[i]+" <" : labels[i];
                GUI.enabled=!IntroductionActive&&!busy&&!lost && (combatMenu!=CombatMenu.Items || i==inventorySettings.items.Length || (inventorySettings.items[i]!=null && itemCounts[i]>0));
                if(GUI.Button(new Rect(20,height-33-labels.Length*48+i*48,280,42),caption)) { selectedCombatAction=i; Action(i); }
            }
            GUI.Label(new Rect(20,height-81-labels.Length*48,290,42),"Flechas: elegir / ENTER: usar\nESC: volver");
            GUI.enabled=true;
        }
        else GUI.Box(new Rect(20,height-100,270,80),"W/S: avanzar / retroceder\nA/D: girar\nTambién: flechas");
        if(lost&&!IntroductionActive&&GUI.Button(new Rect(width/2-100,height/2,200,50),"Reiniciar")) RestartNewGame();
        GUI.color=new Color(0,0,0,.12f);
        for(int y=0;y<height;y+=4) GUI.DrawTexture(new Rect(0,y,width,1),Texture2D.whiteTexture);
        if(glitch>0) { GUI.color=new Color(1,0,.5f,.25f); for(int i=0;i<12;i++) GUI.DrawTexture(new Rect(0,(i*73+Time.frameCount*13)%height,width,7),Texture2D.whiteTexture); }
        GUI.color=Color.white;
        if(!levelComplete && !IntroductionActive && !battle && !lost && exitDoor!=null && exitDoor.CanInteract(view.transform.position))
            GUI.Box(new Rect(width/2-140,height-155,280,45),"[E] Abrir puerta");
        if(levelComplete) DrawCompletion(width,height);
    }
}
