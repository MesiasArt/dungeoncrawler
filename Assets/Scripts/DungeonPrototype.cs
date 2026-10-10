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
        "###.#.#.#.#", "#...#.#...#", "#.###.###.#", "#.........#", "###########" };
    readonly List<Vector2Int> enemies = new List<Vector2Int>();
    readonly List<GameObject> enemyObjects = new List<GameObject>();
    Vector2Int position = new Vector2Int(1,1);
    readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    int heading, hp = 30, enemyHP, activeEnemy = -1, potions = 3, victories;
    bool moving, battle, busy, lost;
    Camera view;
    string message = "Explora. Los enemigos están en el mapa: puedes evitarlos.";
    float glitch;
    Material wall, floor, pink, blue;
    IntroSequence introduction;
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
        if ((interact != null && interact.WasPressedThisFrame()) ||
            (submit != null && submit.WasPressedThisFrame())) return true;
        // Fallback only when the project's action asset is unavailable.
        if (interact == null && submit == null)
            return (Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) ||
                (Gamepad.current != null && (Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame));
        return false;
    }
    void ReleaseIntroductionInput()
    {
        if (enabledInteract) interact.Disable();
        if (enabledSubmit) submit.Disable();
        enabledInteract = enabledSubmit = false;
    }
    void OnDisable() { ReleaseIntroductionInput(); }
    void RestartNewGame()
    {
        GameStartContext.PrepareNewGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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
        foreach (Camera camera in FindObjectsByType<Camera>()) camera.gameObject.SetActive(false);
        foreach (Light light in FindObjectsByType<Light>()) light.enabled = false;
        wall = Surface("Textures/textura azul luffy2", new Color(.35f,.45f,.9f));
        floor = Surface("Textures/textura rosa luffy2", new Color(.65f,.2f,.55f));
        pink = Glow(new Color(1,.015f,.5f)); blue = Glow(new Color(.05f,.45f,1));
        for (int z=0; z<map.Length; z++) for(int x=0;x<map[z].Length;x++)
        {
            Vector3 p = new Vector3(x*Cell,0,z*Cell);
            if (map[z][x]=='#') { Box("Wall",p+Vector3.up*1.5f,new Vector3(Cell,3,Cell),wall); continue; }
            Box("Floor",p+Vector3.down*.1f,new Vector3(Cell,.2f,Cell),floor);
            Box("Ceiling",p+Vector3.up*3.1f,new Vector3(Cell,.2f,Cell),wall);
            Box("Floor neon",p+new Vector3(-1.48f,.015f,0),new Vector3(.025f,.025f,Cell),pink);
            Box("Floor neon",p+new Vector3(0,.015f,-1.48f),new Vector3(Cell,.025f,.025f),pink);
            if ((x+z)%3==0) Box("Ceiling light",p+Vector3.up*2.96f,new Vector3(1,.025f,.45f),blue);
            for(int d=0;d<4;d++)
            {
                Vector2Int n = new Vector2Int(x,z)+directions[d];
                if(!Open(n))
                {
                    Vector3 edge = p+new Vector3(directions[d].x,0,directions[d].y)*1.49f;
                    for(int h=1;h<4;h++) Box("Wall stripe",edge+Vector3.up*h*.75f,
                        d%2==0?new Vector3(Cell,.018f,.025f):new Vector3(.025f,.018f,Cell),pink);
                    Box("Blue edge",edge+new Vector3(d%2==0?-1.45f:0,1.5f,d%2==0?0:-1.45f),new Vector3(.025f,3,.025f),blue);
                }
            }
        }
        AddEnemy(new Vector2Int(5,3)); AddEnemy(new Vector2Int(7,7)); AddEnemy(new Vector2Int(1,5));
        view = new GameObject("Player Camera").AddComponent<Camera>();
        view.fieldOfView = 68; view.nearClipPlane = .05f;
        view.backgroundColor = new Color(.005f,.003f,.02f); view.clearFlags = CameraClearFlags.SolidColor;
        view.transform.position = World(position); view.transform.rotation = Quaternion.Euler(0,heading*90,0);
        view.gameObject.AddComponent<AudioListener>();
        RenderSettings.ambientLight = new Color(.22f,.16f,.36f);
        RenderSettings.fog = true; RenderSettings.fogColor = view.backgroundColor; RenderSettings.fogDensity = .035f;
        Light lamp = new GameObject("Player light").AddComponent<Light>();
        lamp.transform.SetParent(view.transform,false); lamp.type=LightType.Point; lamp.range=12; lamp.intensity=2; lamp.color=new Color(.45f,.4f,1);
        StartIntroduction();
    }
    Material Surface(string path,Color tint)
    {
        Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Texture2D texture = Resources.Load<Texture2D>(path);
        m.SetTexture("_BaseMap",texture); m.SetColor("_BaseColor",tint);
        m.SetFloat("_Smoothness",.35f); return m;
    }
    Material Glow(Color color)
    {
        Material m = new Material(Shader.Find("Universal Render Pipeline/Unlit")); m.SetColor("_BaseColor",color); return m;
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
        foreach(GameObject enemy in enemyObjects) if(enemy!=null) enemy.transform.LookAt(new Vector3(view.transform.position.x,enemy.transform.position.y,view.transform.position.z));
        if (IntroductionActive)
        {
            introduction.Tick(Time.unscaledDeltaTime, ConfirmPressed());
            if (introduction.Interference) glitch = .1f;
            if (!introduction.Active) { introductionCompleted = true; ReleaseIntroductionInput(); }
            return; // The last confirmation cannot also move or act in the same frame.
        }
        if(moving||battle||lost||Keyboard.current==null) return;
        var k=Keyboard.current;
        if(k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,-1));
        else if(k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,1));
        else if(k.wKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame) StartCoroutine(Move(1,0));
        else if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame) StartCoroutine(Move(-1,0));
    }
    IEnumerator Move(int step,int turn)
    {
        if (IntroductionActive) yield break;
        Vector2Int next=position+directions[heading]*step;
        if(step!=0&&!Open(next)) yield break;
        moving=true; heading=(heading+turn+4)%4;
        Vector3 from=view.transform.position,to=World(next); Quaternion rotation=view.transform.rotation,target=Quaternion.Euler(0,heading*90,0);
        for(float t=0;t<1;t+=Time.deltaTime/.18f) { view.transform.position=Vector3.Lerp(from,to,t); view.transform.rotation=Quaternion.Slerp(rotation,target,t); yield return null; }
        position=next; view.transform.position=to; view.transform.rotation=target; moving=false;
        for(int i=0;i<enemies.Count;i++) if(enemyObjects[i]!=null&&(enemies[i]-position).sqrMagnitude<=1)
        { activeEnemy=i; enemyHP=20; battle=true; view.transform.LookAt(World(enemies[i])); glitch=.45f; message="¡Apareció un sentinela! Elige una acción."; break; }
    }
    void Action(int action) { if(!IntroductionActive&&!busy&&!lost) StartCoroutine(Turn(action)); }
    IEnumerator Turn(int action)
    {
        busy=true;
        if(action==3) { battle=false; view.transform.rotation=Quaternion.Euler(0,heading*90,0); message="Escapaste. Retrocede o busca otra ruta."; busy=false; yield break; }
        if(action==2) { if(potions==0) { message="No quedan pociones."; busy=false; yield break; } potions--; hp=Mathf.Min(30,hp+12); message="Recuperaste 12 HP."; }
        else { int damage=action==1?10:7; enemyHP-=damage; message="Infliges "+damage+" de daño."; }
        yield return new WaitForSeconds(.6f);
        if(enemyHP<=0) { Destroy(enemyObjects[activeEnemy]); victories++; battle=false; view.transform.rotation=Quaternion.Euler(0,heading*90,0); message= victories==enemies.Count?"¡Mazmorra despejada! Has completado el prototipo.":"Victoria. Sigue explorando."; }
        else { hp=Mathf.Max(0,hp-5); message="El sentinela responde: pierdes 5 HP."; if(hp==0) { lost=true; message="Has caído. Pulsa Reiniciar."; } }
        busy=false;
    }
    void OnGUI()
    {
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f); GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));
        float width=Screen.width/scale,height=Screen.height/scale;
        GUI.skin.box.fontSize=20; GUI.skin.label.fontSize=18; GUI.skin.button.fontSize=20;
        GUI.backgroundColor=new Color(.12f,.05f,.3f,.95f); GUI.contentColor=new Color(.65f,.85f,1);
        GUI.Box(new Rect(20,20,270,95),"LV 1    EXPLORADOR\nHP "+hp+" / 30\nPociones: "+potions+"   Victorias: "+victories);
        float mx=width-240; GUI.Box(new Rect(mx,20,220,200),"MAPA");
        for(int z=0;z<map.Length;z++) for(int x=0;x<map[z].Length;x++)
        {
            GUI.color=map[z][x]=='#'?new Color(.12f,.13f,.32f):new Color(.03f,.03f,.1f);
            GUI.DrawTexture(new Rect(mx+12+x*17,55+(map.Length-1-z)*17,16,16),Texture2D.whiteTexture);
        }
        GUI.color=Color.magenta;
        for(int i=0;i<enemies.Count;i++) if(enemyObjects[i]!=null) GUI.DrawTexture(new Rect(mx+16+enemies[i].x*17,59+(map.Length-1-enemies[i].y)*17,8,8),Texture2D.whiteTexture);
        GUI.color=Color.white; GUI.Label(new Rect(mx+12+position.x*17,51+(map.Length-1-position.y)*17,25,25),new[]{"▲","▶","▼","◀"}[heading]);
        // Reuse the existing message rectangle, colors, font and box style.
        if (IntroductionActive)
        {
            IntroLine line = introduction.Current;
            GUI.Box(new Rect(310,height-100,width-330,80), line == null ? "..." : line.Speaker + "\n" + line.Text);
        }
        else if (!introductionCompleted || battle || lost || victories > 0)
            GUI.Box(new Rect(310,height-100,width-330,80),message);
        if(battle)
        {
            GUI.Box(new Rect(width/2-160,25,320,65),"SENTINELA\nHP "+Mathf.Max(0,enemyHP)+" / 20");
            GUI.enabled=!IntroductionActive&&!busy&&!lost;
            string[] labels={"ATACAR","HABILIDAD","POCIÓN","HUIR"};
            for(int i=0;i<4;i++) if(GUI.Button(new Rect(20,height-225+i*48,260,42),labels[i])) Action(i);
            GUI.enabled=true;
        }
        else GUI.Box(new Rect(20,height-100,270,80),"W/S: avanzar / retroceder\nA/D: girar\nTambién: flechas");
        if(lost&&!IntroductionActive&&GUI.Button(new Rect(width/2-100,height/2,200,50),"Reiniciar")) RestartNewGame();
        GUI.color=new Color(0,0,0,.12f);
        for(int y=0;y<height;y+=4) GUI.DrawTexture(new Rect(0,y,width,1),Texture2D.whiteTexture);
        if(glitch>0) { GUI.color=new Color(1,0,.5f,.25f); for(int i=0;i<12;i++) GUI.DrawTexture(new Rect(0,(i*73+Time.frameCount*13)%height,width,7),Texture2D.whiteTexture); }
        GUI.color=Color.white;
    }
}
