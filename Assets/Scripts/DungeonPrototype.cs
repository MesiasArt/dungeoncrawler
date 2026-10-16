using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using VirtualDungeon;

// The first playable slice. Launch any scene to explore the prototype.
public partial class DungeonPrototype : MonoBehaviour
{
    public DungeonUILayout uiLayout;
    DungeonUILayout Layout { get { if(uiLayout==null) uiLayout=Resources.Load<DungeonUILayout>("UI/DungeonUILayout");if(uiLayout==null) uiLayout=ScriptableObject.CreateInstance<DungeonUILayout>();return uiLayout; } }
    EnemyCombatBarks enemyBarks;
    int barkEnemy=-1;
    Transform barkSpeaker;
    void Bark(EnemyBarkCategory category) { if(enemyBarks==null)enemyBarks=GetComponent<EnemyCombatBarks>()??gameObject.AddComponent<EnemyCombatBarks>();if(category==EnemyBarkCategory.CombatStart)enemyBarks.ClearDisplay();if(activeEnemy>=0&&activeEnemy<enemyObjects.Count&&enemyObjects[activeEnemy]!=null)if(enemyBarks.TrySpeak(enemyObjects[activeEnemy].name,category)){barkEnemy=activeEnemy;barkSpeaker=enemyObjects[activeEnemy].transform;} }
    float Cell=3f,corridorHeight=3f;
    static readonly string[] mapTemplate = {
        "###########", "#.........#", "#.###.###.#", "#...#...#.#",
        "###.#.#.#.#", "#...#.#...#", "#.###.###.#", "#.........#", "#########.#" };
    public static string[] DefaultMapRows {get{var plan=DungeonMapSettings.Current;return plan!=null&&plan.Valid?(string[])plan.rows.Clone():(string[])mapTemplate.Clone();}}
    string[] map=(string[])mapTemplate.Clone();
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
    int MaximumHP { get { return (inventorySettings==null ? 30 : Mathf.Max(1,inventorySettings.maximumHP))+Levels.Growth(Levels.healthGrowth,PlayerLevel)+EquipmentBonus(0); } }
    int TotalItems { get { int total=0; if(itemCounts!=null) foreach(int count in itemCounts) total+=Mathf.Max(0,count); return total; } }
    bool[,] explored,visited;
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
    readonly DialogueTypewriter introTyping=new DialogueTypewriter(),messageTyping=new DialogueTypewriter();
    IntroLine typingLine;
    void SyncIntroTyping() { var line=introduction==null?null:introduction.Current;if(!ReferenceEquals(line,typingLine)){typingLine=line;introTyping.SetText(line==null?"":line.DisplayText,Time.unscaledTime,true);} }
    IntroSequence introduction;
    [SerializeField] CombatHack combatHack;
    InputAction interact, submit;
    bool enabledInteract, enabledSubmit, introductionCompleted;
    bool IntroductionActive { get { return introduction != null && introduction.Active; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetGameStart() { GameStartContext.PrepareNewGame(); }

    void StartIntroduction()
    {
        EndIntroPresentation(false);
        introduction = new IntroSequence(GameStartContext.Consume());
        SyncIntroTyping();
        if (!introduction.Active) return;
        BeginIntroPresentation();
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
        SyncIntroTyping();
        var previousLine=introduction.Current;
        skip=false;
        if(skip) introduction.Skip();
        else {
            if(advance&&introduction.Current!=null&&!introTyping.IsComplete(Time.unscaledTime,Layout.lettersPerSecond)) { introTyping.Complete();advance=false; }
            introduction.Tick(elapsed,advance);
        }
        if(!skip&&advance&&previousLine!=null&&!ReferenceEquals(previousLine,introduction.Current))DungeonAudio.Play(DungeonAudio.Cue.TextAdvance);
        SyncIntroTyping();
        UpdateIntroPresentation();
        if(!introduction.Active) { introductionCompleted=true; glitch=0; EndIntroPresentation(true);ReleaseIntroductionInput();ClearMovementInput(); }
    }
    void HandleCombatInput(int direction, bool confirm)
    {
        if(!battle || busy || lost || IntroductionActive || (combatHack!=null && combatHack.Active)) return;
        if(selectingEnemy){if(direction!=0)SelectEnemy(direction);if(confirm)ConfirmEnemyTarget();return;}
        if(!selectingEnemy&&combatMenu==CombatMenu.Main&&SpeedrunInput())return;
        if(direction!=0) { int count=CombatLabels().Length; selectedCombatAction=(selectedCombatAction+(direction>0 ? 1 : -1)+count)%count; return; }
        if(confirm) Action(selectedCombatAction);
    }
    void ReleaseIntroductionInput()
    {
        if (enabledInteract) interact.Disable();
        if (enabledSubmit) submit.Disable();
        enabledInteract = enabledSubmit = false;
    }
    void OnDisable() { CancelRefugeWelcome();EndIntroPresentation(true);ClearMovementInput(); CancelMapReveal(); SetPause(false); ReleaseIntroductionInput(); }
    void OnDestroy() { if(voidDepthMaterial!=null)Destroy(voidDepthMaterial);EndMakongPresentation();if(battleCeiling!=null)Destroy(battleCeiling);if (floorWallDistance != null) Destroy(floorWallDistance); }
    void RestartNewGame()
    {
        GameStartContext.PrepareNewGame();
        var inventory=inventorySettings;var door=exitDoorSettings;var plan=mapSettings;var regions=dungeonRegions;
        Destroy(gameObject);
        var fresh=new GameObject("Dungeon Prototype").AddComponent<DungeonPrototype>();
        fresh.inventorySettings=inventory;fresh.exitDoorSettings=door;fresh.mapSettings=plan;fresh.dungeonRegions=regions;
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
        if(FindAnyObjectByType<CodeboundTitleScreen>()!=null)return;
        if(FindAnyObjectByType<DigitalKingdomController>()!=null)return;
        if(FindAnyObjectByType<EnemySizeGallery>()!=null)return;
        if (FindAnyObjectByType<DungeonPrototype>() == null)
            new GameObject("Dungeon Prototype").AddComponent<DungeonPrototype>();
    }
    void Start()
    {
        DungeonAudio.SetCityMusic(false);
        bool cityArrival=DigitalKingdomTransit.ReturnData!=null;if(cityArrival){if(DigitalKingdomTransit.Map!=null)mapSettings=DigitalKingdomTransit.Map;characterSettings=DigitalKingdomTransit.Character;playerLevelSettings=DigitalKingdomTransit.Levels;inventorySettings=DigitalKingdomTransit.Inventory;}
        ApplyPlayerProfileDefaults();
        map=ActiveMap!=null&&ActiveMap.Valid?CurrentMapDefinition.Rows:DefaultMapRows;
        if(mapSettings!=null&&mapSettings.regions!=null)dungeonRegions=mapSettings.regions;
        var refugeConfig=RefugeSettings.Current;if(refugeConfig!=null){position=refugeConfig.startCell;heading=refugeConfig.startHeading;}if(mapSettings!=null&&!mapSettings.includeLegacyObjects){position=mapSettings.startCell;heading=mapSettings.startHeading;}
        Vector2 space=EnemySpaceLayout.Corridor(DungeonVisualSettings.Current,EnemySizeSettings.Current);Cell=space.x;corridorHeight=space.y;
        explored=new bool[map[0].Length,map.Length];visited=new bool[map[0].Length,map.Length];
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
        ApplyNeonTextureScales();
        ConfigureFloorSheen();
        pink = Glow(new Color(1,.015f,.5f)); blue = Glow(new Color(.05f,.45f,1));
        for (int z=0; z<map.Length; z++) for(int x=0;x<map[z].Length;x++)
        {
            Vector3 p = new Vector3(x*Cell,0,z*Cell);
            if (map[z][x]=='#') { Box("Wall",p+Vector3.up*(corridorHeight*.5f),new Vector3(Cell,corridorHeight,Cell),wall); continue; }
            if(map[z][x]=='~'){BuildVoidDepth(new Vector2Int(x,z));Box("Void ceiling",p+Vector3.up*(corridorHeight+.1f),new Vector3(Cell,.2f,Cell),ceiling);continue;}
            Box("Floor",p+Vector3.down*.1f,new Vector3(Cell,.2f,Cell),floor);
            Box("Ceiling",p+Vector3.up*(corridorHeight+.1f),new Vector3(Cell,.2f,Cell),ceiling);
            if ((x+z)%3==2 && !(x==9 && z==8))
            {
                foreach(Vector2Int direction in directions)
                {
                    if(Open(new Vector2Int(x,z)+direction)) continue;
                    var wallLamp=new GameObject("Magenta wall lamp");
                    wallLamp.transform.SetParent(transform,false);
                    wallLamp.transform.position=p+new Vector3(direction.x,0,direction.y)*(Cell*.5f-.06f);
                    wallLamp.transform.rotation=Quaternion.LookRotation(new Vector3(-direction.x,0,-direction.y));
                    wallLamp.AddComponent<DungeonWallLamp>();
                    break; // One fixture per chosen corridor cell.
                }
            }
        }
        bool restoreFloor=cityArrival||(incomingMapTravel!=null&&!incomingMapFirstVisit);
        if(!restoreFloor)SpawnPlacedEncounters();
        if(!restoreFloor&&(RegionMap==null||RegionMap.legacyPlacements)){
        AddEnemy(new Vector2Int(5,3)); AddEnemy(new Vector2Int(7,7),soldier:true); AddEnemy(new Vector2Int(1,5),true);if(EnemySettings.spawnBigTapaku)AddEnemy(new Vector2Int(9,3),bigTapaku:true);
        if(EnemySettings.spawnMakubas){AddEnemy(new Vector2Int(3,3),visualKind:4);AddEnemy(new Vector2Int(5,7),visualKind:5);if(EnemySettings.spawnGranMakuba)AddEnemy(new Vector2Int(9,5),visualKind:6);}
        }
        if(ActiveMap==null||ActiveMap.includeLegacyObjects)CreateExitDoor();
        var recess=gameObject.AddComponent<DungeonExitDepth>();recess.cellSize=Cell;recess.ceilingHeight=corridorHeight;
        view = new GameObject("Player Camera").AddComponent<Camera>();
        view.transform.SetParent(transform,false);
        view.fieldOfView = 68; view.nearClipPlane = .05f;
        view.backgroundColor = new Color(.005f,.003f,.02f); view.clearFlags = CameraClearFlags.SolidColor;
        view.transform.position = World(position); view.transform.rotation = Quaternion.Euler(0,heading*90,0);
        view.gameObject.AddComponent<AudioListener>();
        view.gameObject.AddComponent<CombatImpactFeedback>();
        view.gameObject.AddComponent<HelmetCRTController>();
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
        view.gameObject.AddComponent<DungeonFloorReflection>().Initialize(view, ceiling, corridorHeight);
        combatHack = GetComponent<EnemyJumpHack>();
        if(combatHack==null) combatHack=gameObject.AddComponent<EnemyJumpHack>();
        InitializeHackAvailability();
        inventorySettings=inventorySettings!=null ? inventorySettings : Resources.Load<CombatInventorySettings>("Combat/CombatInventorySettings");
        if(inventorySettings==null) inventorySettings=ScriptableObject.CreateInstance<CombatInventorySettings>();
        itemCounts=new int[inventorySettings.items.Length];
        for(int i=0;i<itemCounts.Length;i++) itemCounts[i]=RefugeSettings.Current!=null||inventorySettings.items[i]==null ? 0 : Mathf.Max(0,inventorySettings.items[i].startingQuantity);
        hp=MaximumHP;
        if(ActiveMap==null||ActiveMap.includeLegacyObjects)CreatePreservationCorridor();
        if(ActiveMap==null||ActiveMap.includeLegacyObjects)CreateRefuge();RemoveDisabledLegacyDoors();CreateMapEntities();if(ActiveMap!=null&&ActiveMap.digitalDoorsEnabled)foreach(var legacyDoor in new[]{exitDoor,refugeDoor,zoneTwoDoor})if(legacyDoor!=null)legacyDoor.ConfigureDigital(DigitalDoorAccess.Cyan,false,true,false);ConfigureDoorZoneSigns();foreach(var text in GetComponentsInChildren<TextMesh>()){if(text.GetComponentInParent<DungeonDoorZoneSigns>()!=null)continue;var occlusion=text.GetComponent<WorldTextOcclusion>();if(occlusion==null)occlusion=text.gameObject.AddComponent<WorldTextOcclusion>();occlusion.viewCamera=view;}if(!restoreFloor)PopulateRegions();
        CreateDungeonDecorations();bool mapArrival=incomingMapTravel!=null;StartIntroduction();ApplyMapTravel();if(cityArrival){restoringCitySnapshot=true;var data=DigitalKingdomTransit.ConsumeReturn();RestoreDungeonSave(data);restoringCitySnapshot=false;var target=DigitalKingdomTransit.ArrivalNode;DigitalKingdomTransit.ArrivalNode=null;if(!string.IsNullOrEmpty(target))DiscoverRefugeReturn(target);}else if(!mapArrival){var requestedSave=CodeboundTitleScreen.TakeSavedGame();if(requestedSave!=null)RestoreDungeonSave(requestedSave);else if(CodeboundTitleScreen.TakeNewGame())StartFreshFromMenu();else ShowStartupMenu();}
    }
    void RevealVisitedCell(Vector2Int cell)
    {
        if(!Open(cell))return;
        if(visited==null)visited=new bool[map[0].Length,map.Length];visited[cell.x,cell.y]=true;
        explored[cell.x,cell.y]=true;
        foreach(Vector2Int direction in directions)
        {
            Vector2Int adjacent=cell+direction;
            if(adjacent.x>=0&&adjacent.x<map[0].Length&&adjacent.y>=0&&adjacent.y<map.Length&&map[adjacent.y][adjacent.x]!='.')
                explored[adjacent.x,adjacent.y]=true;
        }
    }
    void CreateExitDoor()
    {
        var root=new GameObject("Level exit door");root.transform.SetParent(transform,false);
        var exitCell=ActiveMap!=null?ActiveMap.legacyExitDoorCell:new Vector2(9,7.5f);root.transform.position=new Vector3(exitCell.x*Cell,0,exitCell.y*Cell);
        exitDoor=root.AddComponent<DungeonDoor>();
        exitDoorSettings=exitDoorSettings!=null ? exitDoorSettings : Resources.Load<DungeonDoorSettings>("Exploration/ExitDoorSettings");
        exitDoor.settings=exitDoorSettings;exitDoor.doorwayWidth=Cell;exitDoor.interactionDistance=Mathf.Max(exitDoor.interactionDistance,Cell*.5f+.25f);exitDoor.openingTravel=corridorHeight+.2f;
        var panel=Box("Exit door panel",root.transform.position+Vector3.up*(corridorHeight*.5f),new Vector3(Cell-.15f,corridorHeight,.18f),wall);
        panel.transform.SetParent(root.transform,true);exitDoor.panel=panel.transform;exitDoor.blocker=panel.GetComponent<Collider>();
        for(int side=-1;side<=1;side+=2)
        {
            var frame=Box("Exit door frame",root.transform.position+new Vector3(side*(Cell*.5f-.04f),corridorHeight*.5f,0),new Vector3(.08f,corridorHeight,.22f),blue);frame.transform.SetParent(root.transform,true);
        }
        var stripe=Box("Exit door neon",root.transform.position+new Vector3(0,1.4f,-.11f),new Vector3(Cell-.3f,.06f,.025f),pink);stripe.transform.SetParent(panel.transform,true);
        exitDoor.Initialize();exitDoor.Passed+=OnExitPassed;
    }
    void OnExitPassed()
    { if(PreservationCorridorSettings.Current!=null){message="Pasillo de preservación — zona segura";return;} if(exitDoor.settings.completesLevel && !levelComplete) StartCoroutine(CompleteLevel()); }
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
        var title=new GUIStyle(GUI.skin.label){fontSize=41,alignment=TextAnchor.MiddleCenter};
        GUI.Label(new Rect(0,height*.35f,width,70),showMainMenu ? "VIRTUAL DUNGEON" : "NIVEL COMPLETADO",title);
        if(showMainMenu) { if(GUI.Button(new Rect(width/2-140,height*.55f,280,50),"JUGAR")) RestartNewGame();if(GUI.Button(new Rect(width/2-140,height*.55f+65,280,50),"CARGAR PARTIDA"))OpenSaveSlots(true); }
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
    void AddEnemy(Vector2Int cell, bool hongo = false, bool soldier = false, bool bigTapaku = false, int visualKind = -1, DungeonRegion origin = null, PlacedEnemyDefinition placement = null)
    {
        if(placement==null&&ReservedCell(cell))return;
        origin=origin??RegionAt(cell);if(origin==null||(!origin.CanSpawn&&!(placement!=null&&placement.allowInSafeArea&&origin.safeArea))||!Open(cell)||Occupied(cell))return;
        var config=origin.enemies;bool counts=placement==null||placement.countsTowardLimit;
        if(counts&&Population(origin.id)>=config.maximumEnemies)return;
        int requested=visualKind>=0?Mathf.Clamp(visualKind,0,EnemyRoster.Count-1):bigTapaku?3:soldier?2:hongo?1:0;
        if(placement==null&&!config.AllowsKind(requested)){requested=config.SelectKind(Random.value);if(requested<0)return;}
        int level=placement!=null&&!placement.randomLevel?placement.fixedLevel:config.SelectLevel(Random.value);if(level<1)return;
        enemies.Add(cell);
        GameObject root = new GameObject("Sentinela"); root.transform.position = World(cell)-Vector3.up*1.5f; root.transform.SetParent(transform);
        var stats=root.AddComponent<EnemyCombatStats>();
        int kind=requested;
        root.name=EnemyRoster.Names[kind];stats.maximumHP=10;stats.damage=2;
        var visual=EnemyRoster.AddVisual(root,kind);
        var big=visual as BigTapakuEnemyVisual;if(big!=null)big.specialEveryAttacks=EnemySettings.bigTapakuSpecialEvery;
        var profile=EnemyCharacterCatalog.Current!=null?EnemyCharacterCatalog.Current.FindId(placement!=null?placement.speciesId:EnemyRoster.ResourceIds[kind]):null;
        if(profile!=null){stats.Initialize(profile,level,EnemyLevelSettings.Current);if(placement!=null&&placement.speciesId=="DevotoCorrompido")root.name=profile.character;}
        stats.zoneId=origin.id;stats.placementId=placement==null?null:placement.id;stats.category=placement==null?PlacedEnemyCategory.Normal:placement.category;
        slots.Add(new EnemySpawnSlot{origin=origin,placement=placement,counts=counts,instanceId=stats.instanceId,explorationHome=cell});
        if(placement!=null&&placement.exploration!=null&&placement.exploration.profile==EnemyExplorationProfile.Elite&&placement.exploration.showEliteMarker)root.AddComponent<EnemyExplorationMarker>().Initialize(this,visual);
        root.transform.localScale=Vector3.one*.75f;
        enemyObjects.Add(root);
        enemyLives.Add(stats.maximumHP); alerted.Add(false); respawnAt.Add(-1);
    }
    bool Open(Vector2Int p) { return p.y>=0&&p.y<map.Length&&p.x>=0&&p.x<map[0].Length&&map[p.y][p.x]=='.'; }
    Vector3 World(Vector2Int p) { return new Vector3(p.x*Cell,1.5f,p.y*Cell); }
    void Update()
    {
        UpdateHackSystemSession();
        if(TryCityRespawn())return;
        if(UpdateCityInput())return;
        if(UpdateBossEvents())return;
        if(MovementInterfaceBlocked())ClearMovementInput();
        if(UpdateMapReveal())return;
        var laboratory=GetComponent<CombatLaboratory>();if(laboratory!=null&&laboratory.MenuOpen)return;
        ClaimPendingCombatDrops();
        if(UpdateSaveSlotInput())return;
        if(UpdateRefugeWelcome())return;
        if(UpdateRefugeInput())return;
        if(makongMusicTransition)return;
        if(UpdateMapEntityInput())return;
        if(UpdatePreservationInput())return;
        if(PauseInput())return;
        if(PartySwitchInput())return;
        if(message=="[E] Abrir puerta")message="";
        messageTyping.SetText(message,Time.unscaledTime);
        if(!IntroductionActive&&(combatHack==null||!combatHack.Active)&&
           (ConfirmPressed()||(Keyboard.current!=null&&(Keyboard.current.enterKey.wasPressedThisFrame||Keyboard.current.numpadEnterKey.wasPressedThisFrame)))) {
            messageTyping.Complete();if(enemyBarks!=null)enemyBarks.CompleteText();
        }
        DungeonAudio.SetCombat(battle && !levelComplete && !lost);
        UpdateHelmetContext();
        glitch=Mathf.Max(0,glitch-Time.deltaTime);
        if(view==null) return;
        UpdateEnemyRespawns();
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
                if(!busy && (combatHack==null || !combatHack.Active)) { if(keys.qKey.wasPressedThisFrame) SelectEnemy(-1);if(keys.eKey.wasPressedThisFrame)SelectEnemy(1); }
                if(keys.escapeKey.wasPressedThisFrame || keys.backspaceKey.wasPressedThisFrame) { BackToCombatMenu(); return; }
                int direction=(keys.downArrowKey.wasPressedThisFrame || keys.rightArrowKey.wasPressedThisFrame) ? 1 :
                    ((keys.upArrowKey.wasPressedThisFrame || keys.leftArrowKey.wasPressedThisFrame) ? -1 : 0);
                HandleCombatInput(direction,keys.enterKey.wasPressedThisFrame || keys.numpadEnterKey.wasPressedThisFrame || keys.spaceKey.wasPressedThisFrame);
            }
            var pad=Gamepad.current;
            if(pad!=null){if(pad.buttonEast.wasPressedThisFrame)BackToCombatMenu();else {int direction=pad.dpad.down.wasPressedThisFrame||pad.dpad.right.wasPressedThisFrame?1:pad.dpad.up.wasPressedThisFrame||pad.dpad.left.wasPressedThisFrame?-1:0;HandleCombatInput(direction,pad.buttonSouth.wasPressedThisFrame);}}
            return;
        }
        if(ExperimentalMovement){if(!moving&&Keyboard.current!=null&&DoorInteractionAvailable(exitDoor)&&Keyboard.current.eKey.wasPressedThisFrame){ClearMovementInput();OpenLegacyEventDoor("legacy.exit",exitDoor);return;}PollMovementInput();return;}
        if(moving||lost||Keyboard.current==null) return;
        var k=Keyboard.current;
        if(DoorInteractionAvailable(exitDoor) && k.eKey.wasPressedThisFrame) { OpenLegacyEventDoor("legacy.exit",exitDoor);return; }
        if(k.aKey.wasPressedThisFrame||k.leftArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,-1));
        else if(k.dKey.wasPressedThisFrame||k.rightArrowKey.wasPressedThisFrame) StartCoroutine(Move(0,1));
        else if(k.wKey.wasPressedThisFrame||k.upArrowKey.wasPressedThisFrame) StartCoroutine(Move(1,0));
        else if(k.sKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame) StartCoroutine(Move(-1,0));
    }
    IEnumerator Move(int step,int turn)
    {
        if (IntroductionActive || levelComplete) yield break;
        Vector2Int next=position+directions[heading]*step;
        if(step!=0 && TryEnterCityPortal(World(position),World(next)))yield break;
        if(step!=0&&(!Open(next)||RefugeNpcBlocks(next))) yield break;
        if(step!=0 && DoorsBlock(World(position),World(next))) { yield break; }
        if(step!=0)for(int i=0;i<enemies.Count;i++)if(enemyObjects[i]!=null&&enemies[i]==next){moving=true;while(enemyNavigationRunning)yield return null;moving=false;ClearMovementInput();BeginEncounter(i);yield break;}
        if(step!=0) DungeonAudio.Play(DungeonAudio.Cue.Step);
        moving=true; heading=(heading+turn+4)%4;
        Vector3 from=view.transform.position,to=World(next); Quaternion rotation=view.transform.rotation,target=Quaternion.Euler(0,heading*90,0);
        if(ExperimentalMovement){float duration=Mathf.Max(.03f,step!=0?Movement.stepSeconds:Movement.turnSeconds);float started=Time.time;float elapsed=0;while(elapsed<duration){elapsed=Time.time-started;float t=DungeonMovementSettings.Interpolate(elapsed/duration,step!=0?Movement.stepInterpolation:Movement.turnInterpolation);view.transform.position=Vector3.Lerp(from,to,t);view.transform.rotation=Quaternion.Slerp(rotation,target,t);if(elapsed<duration)yield return null;}}
        else for(float t=0;t<1;t+=Time.deltaTime/.18f) { view.transform.position=Vector3.Lerp(from,to,t); view.transform.rotation=Quaternion.Slerp(rotation,target,t); yield return null; }
        position=next;if(step!=0)DirectorPlayerStep(); RevealVisitedCell(position); view.transform.position=to; view.transform.rotation=target; moving=false;
        if(exitDoor!=null) exitDoor.CheckPassage(from,to);
        if(zoneTwoDoor!=null)zoneTwoDoor.CheckPassage(from,to);
        foreach(var door in mapDoors.Values)door.CheckPassage(from,to);
        if(refugeDoor!=null)refugeDoor.CheckPassage(from,to);
        if(levelComplete||cityActive){moving=false;yield break;}
        moving=true;
        if(ExperimentalMovement&&Movement.overlapEnemyAnimation){
            // Preserve one enemy decision turn per completed player action; serialize enemy turns.
            while(enemyNavigationRunning)yield return null;
            if(!MovementInterfaceBlocked()&&!TryEncounter())StartCoroutine(AnimateEnemyNavigation());
            moving=false;ContinueNavigation();
        }else{if(!TryEncounter()) { yield return EnemyTurn();TryEncounter(); }moving=false;if(ExperimentalMovement)ContinueNavigation();}
    }

    string[] CombatLabels()
    {
        if(combatMenu==CombatMenu.Main) return new[]{"ATTACK","HACK","ITEM","HUIR"};
        if(combatMenu==CombatMenu.Hacks)
        {
            RefreshAvailableHacks();
            int count=availableHacks==null ? 0 : availableHacks.Length;
            var labels=new string[count+2];
            for(int i=0;i<count;i++) labels[i]=availableHackDefinitions[i].displayName+" ["+availableHackDefinitions[i].bitsCost+" Bits]";
            labels[count]="SPEEDRUN";labels[count+1]="VOLVER"; return labels;
        }
        var items=inventorySettings.items;
        var usable=CombatItemIndices();var itemLabels=new string[usable.Count+1];
        for(int i=0;i<usable.Count;i++) itemLabels[i]=items[usable[i]].displayName+" x "+Mathf.Max(0,itemCounts[usable[i]]);
        itemLabels[usable.Count]="VOLVER";return itemLabels;
    }
    void BackToCombatMenu()
    {
        if(selectingEnemy){selectingSpeedrun=false;selectingEnemy=false;pendingTargetHack=null;return;}
        if(busy || lost || (combatHack!=null && combatHack.Active) || combatMenu==CombatMenu.Main) return;
        selectedCombatAction=combatMenu==CombatMenu.Hacks ? 1 : 2;
        combatMenu=CombatMenu.Main;
    }
    void Action(int action)
    {
        if(IntroductionActive || busy || lost || !battle || (combatHack!=null && combatHack.Active)) return;
        if(selectingEnemy){if(selectingSpeedrun){ConfirmEnemyTarget();return;}if(action==(pendingTargetHack!=null?1:0))ConfirmEnemyTarget();return;}
        if(action<0 || action>=CombatLabels().Length) return;
        if(RoutePartyCommand(action))return;
        if(combatMenu==CombatMenu.Hacks)
        {
            if(action==availableHacks.Length+1) { BackToCombatMenu(); return; }
            if(action==availableHacks.Length){combatMenu=CombatMenu.Main;selectedCombatAction=1;BeginSpeedrunSelection();return;}
            selectedHackId=availableHackDefinitions[action].id;combatHack=availableHacks[action]; combatMenu=CombatMenu.Main; selectedCombatAction=1;
            if(encounter.Count>1){pendingTargetHack=combatHack;selectingEnemy=true;message="Elige el objetivo del HACK y confirma.";}
            else BeginTargetHack(combatHack);

            return;
        }
        if(combatMenu==CombatMenu.Items)
        {
            var usable=CombatItemIndices();if(action==usable.Count) { BackToCombatMenu(); return; }action=usable[action];
            var item=inventorySettings.items[action];
            if(item==null || itemCounts[action]<=0) { message="No quedan unidades de ese objeto."; return; }
            if(item.gainOneLevel){if(!UseLevelUpCheat(action))return;busy=true;combatMenu=CombatMenu.Main;selectedCombatAction=2;StartCoroutine(ResolveCombatDamage(0,"ITEM"));return;}
            if(hp>=MaximumHP) { message="Ya tienes el HP completo."; return; }
            int healed=Mathf.Min(Mathf.Max(0,item.healHP),MaximumHP-hp);
            DungeonAudio.Play(DungeonAudio.Cue.Item);itemCounts[action]--; hp+=healed;NotifyHealing(item,healed); busy=true;
            message=item.displayName+": recuperas "+healed+" HP.";
            combatMenu=CombatMenu.Main;selectedCombatAction=2;
            StartCoroutine(ResolveCombatDamage(0,"ITEM")); return;
        }
        if(action==0) { if(encounter.Count>1){pendingTargetHack=null;selectingEnemy=true;message="Elige el objetivo y confirma.";}else {busy=true;StartCoroutine(ResolveCombatDamage(PlayerStrength,"ATTACK"));}return; }
        if(action==1) { RefreshAvailableHacks();combatMenu=CombatMenu.Hacks;selectedCombatAction=0;return; }
        if(action==2) { combatMenu=CombatMenu.Items;selectedCombatAction=0;return; }
        if(RollAttackMiss(100-CombatAccuracySettings.EffectiveFlee(Character.fleePercent,PlayerLuck))){message="No pudiste huir.";busy=true;StartCoroutine(ResolveCombatDamage(0,"HUIR"));return;}
        if(pendingMakongNarrative!=null){busy=true;StartCoroutine(FinishMakongBeforeFlee());return;}
        FinishSuccessfulFlee();
    }
    IEnumerator ResolveCombatDamage(int damage, string actionName, bool instantClear=false)
    {
        int hit=activeEnemy;
        float missPercent=actionName=="ATTACK"?Accuracy.CharacterMissPercent("GAMERBOY",false):actionName=="HACK"&&combatHack!=null?Accuracy.HackMissPercent(combatHack.GetType().Name,combatHack.UsesAttackLuck):0;
        if(actionName=="ATTACK"||(actionName=="HACK"&&combatHack!=null&&combatHack.UsesAttackLuck))missPercent=PlayerMiss(missPercent);
        bool missed=damage>0&&RollAttackMiss(missPercent);
        bool critical=!missed&&damage>0&&actionName=="ATTACK"&&CombatMath.CriticalRoll(CombatAccuracySettings.EffectiveBonus(Character.criticalPercent,PlayerLuck),Random.value);
        float quality=actionName=="HACK"&&combatHack!=null?combatHack.AttackQuality(damage):1;
        int attack=actionName=="HACK"?PlayerStrength:damage;
        if(instantClear)damage=enemyLives[hit];
        else if(!missed)damage=CombatMath.Damage(attack,enemyObjects[hit].GetComponent<EnemyCombatStats>().defense,quality,critical,Character.criticalMultiplier,CombatBalanceSettings.Current);
        if(missed)damage=0;
        if(actionName=="HACK"&&combatHack is OverclockHack)((OverclockHack)combatHack).ShowResolvedDamage(damage);
        var ghost=enemyObjects[hit].GetComponent<GhostEnemyVisual>();
        if(actionName=="ATTACK") DungeonAudio.Play(DungeonAudio.Cue.Attack);
        enemyHP=enemyLives[hit]=Mathf.Max(0,enemyLives[hit]-damage);
        if(damage>0&&enemyHP>0){Bark(EnemyBarkCategory.Hurt);RefugeLowLifeBark(hit);}
        if(ghost!=null&&damage>0)ghost.Play(GhostEnemyVisual.Pose.Hurt);
        if(actionName=="ATTACK"&&damage>0)view.GetComponent<CombatImpactFeedback>().Play(false);
        if(!instantClear&&actionName!="ITEM"&&actionName!="HUIR")message=missed?"GAMERBOY: "+actionName+" falló.":actionName+(critical?" CRÍTICO": "")+": "+damage+" de daño confirmado.";
        bool defeated=enemyLives[hit]<=0;
        SignalBossState(hit,defeated);
        if(defeated){
            Bark(EnemyBarkCategory.Defeat);
            if(!instantClear&&ghost!=null)ghost.Play(GhostEnemyVisual.Pose.Death);
            var corpse=enemyObjects[hit];
            RecordDefeated(hit);OnRefugeEnemyDefeated(hit);
            enemyObjects[hit]=null;victories++;RecordCombatDropEnemy(corpse.GetComponent<EnemyCombatStats>());RecordEnemyXP(corpse.GetComponent<EnemyCombatStats>());respawnAt[hit]=RespawnDeadline(hit);
            encounter.Remove(hit);encounterHomes.Remove(hit);encounterScales.Remove(hit);
            if(!HoldMakongNarrative(corpse)){if(instantClear)Destroy(corpse);else StartCoroutine(DisappearDefeatedEnemy(corpse));}
        }
        PartyActionFinished();
        if(!instantClear)yield return new WaitForSeconds(.6f);
        if(defeated){
            if(encounter.Count==0){yield return FinishMakongNarrative();DirectorCombatEnded();AwardEncounterXP();AwardCombatDrops();RestoreEncounter();battle=false;activeEnemy=-1;yield return RestoreBattleCamera();view.transform.rotation=Quaternion.Euler(0,heading*90,0);message="";SystemMessages.Show(SystemMessageCode.Victory);busy=false;yield break;}
            activeEnemy=encounter[0];enemyHP=enemyLives[activeEnemy];
        }
        int selected=activeEnemy;
        // Each living opponent takes one turn; the player's selected target stays independent.
        foreach(int index in encounter.ToArray())
        {
            if(enemyObjects[index]==null||enemyLives[index]<=0||lost)continue;
            activeEnemy=index;enemyHP=enemyLives[index];
            ghost=enemyObjects[index].GetComponent<GhostEnemyVisual>();
            var soldier=ghost as SoldierEnemyVisual;
            bool grenade=soldier!=null&&soldier.NextAttackIsGrenade();
            bool spin=ghost!=null&&ghost.NextAttackIsSpecial();
            yield return ResolveEnemyAttack(index,ghost,grenade,spin);
        }
        activeEnemy=encounter.Contains(selected)&&enemyObjects[selected]!=null?selected:encounter[0];enemyHP=enemyLives[activeEnemy];busy=false;
    }
    void DrawWristHackMenu()
    {
        var savedMatrix=GUI.matrix;var savedColor=GUI.color;
        var savedBackground=GUI.backgroundColor;var savedContent=GUI.contentColor;
        bool savedEnabled=GUI.enabled;int savedDepth=GUI.depth;
        GUI.depth=-20;
        WristHackScreen.DrawFrame();WristHackScreen.ApplyContentMatrix();
        var titleStyle=new GUIStyle(GUI.skin.label){fontSize=36,alignment=TextAnchor.MiddleCenter};
        var entryStyle=new GUIStyle(GUI.skin.button){fontSize=29};
        var hintStyle=new GUIStyle(GUI.skin.label){fontSize=22,alignment=TextAnchor.MiddleCenter};
        GUI.Label(new Rect(40,15,920,50),"HACKS",titleStyle);
        string[] labels=CombatLabels();
        float row=Mathf.Min(65,340f/Mathf.Max(1,labels.Length));
        for(int i=0;i<labels.Length;i++)
        {
            GUI.enabled=!busy&&!lost;
            GUI.contentColor=i==selectedCombatAction ? new Color(1,.3f,.75f) : WristHackScreen.Text;
            string caption=i==selectedCombatAction ? "> "+labels[i]+" <" : labels[i];
            if(GUI.Button(new Rect(170,90+i*row,660,row-10),caption,entryStyle)) { selectedCombatAction=i;Action(i); }
        }
        GUI.enabled=true;GUI.contentColor=WristHackScreen.Text;
        if(selectedCombatAction>=0&&selectedCombatAction<availableHackDefinitions.Count){var descriptionStyle=new GUIStyle(GUI.skin.label){fontSize=19,wordWrap=true,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(80,462,840,44),availableHackDefinitions[selectedCombatAction].description,descriptionStyle);}
        GUI.Label(new Rect(30,515,940,40),"Flechas: elegir / ENTER: usar / ESC: volver",hintStyle);
        GUI.matrix=savedMatrix;GUI.color=savedColor;GUI.backgroundColor=savedBackground;GUI.contentColor=savedContent;GUI.enabled=savedEnabled;GUI.depth=savedDepth;
    }
    void OnGUI()
    {
        SystemMessages.SetStatus(null);
        SystemMessages.SuppressPanel=cityActive||mapRevealActive||(pauseOpen&&!(pausePage==2&&Time.unscaledTime<cheatMessageUntil))||IntroductionActive||worldDialogue.Active||preservationLine>=0;
        HelmetCRTLayerMask.BeginGUI();
        if(cityActive){if(cityDataOnly || !DungeonWhiteTransition.Active)DrawCity();return;}
        if(mapRevealActive){DrawMapReveal();return;}
        if(pauseOpen){DrawPause();return;}
        if(battle && !worldDialogue.Active && !IntroductionActive && (combatHack!=null && combatHack.Active))
        {
            return;
        }
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f); GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));
        float width=Screen.width/scale,height=Screen.height/scale;
        NeonDungeonUI.FontSize=Layout.textFontSize;NeonDungeonUI.ContinueBlinkSeconds=Layout.continueBlinkSeconds;
        GUI.skin.box.fontSize=24; GUI.skin.label.fontSize=22; GUI.skin.button.fontSize=24;
        GUI.backgroundColor=new Color(.12f,.05f,.3f,.95f); GUI.contentColor=new Color(.65f,.85f,1);
        NeonDungeonUI.ApplyFont();
        if(IntroductionActive){DrawIntroductionCinematic(width,height);return;}
        DrawActivePartyPanel();
        float mx=width-240;
        Rect mapRect=Layout.Resolve(Layout.map,width,0);
        if(ExplorationMinimapVisible){DrawLocalMinimap(mapRect);DrawZoneCaption(new Rect(mapRect.x,mapRect.yMax+4,mapRect.width,26),position);}
        // Reuse the existing message rectangle, colors, font and box style.
        if (IntroductionActive)
        {
            IntroLine line = introduction.Current;
            Rect dialogueRect = Layout.Resolve(Layout.dialogue,0,height);
            if(line!=null&&line.Speaker=="GAMERBOY")NeonDungeonUI.DialogueAvatar(Layout.Resolve(Layout.dialogueAvatar,0,height));
            NeonDungeonUI.Dialogue(dialogueRect,line == null ? "" : line.Speaker,line == null ? "..." : introTyping.VisibleText(Time.unscaledTime,Layout.lettersPerSecond,true),Layout.preservePanelAspect,line!=null&&introTyping.IsComplete(Time.unscaledTime,Layout.lettersPerSecond));
            var skipStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(width/2-160,height/2-16,320,32), "ENTER: saltar intro", skipStyle);
        }
        else if(worldDialogue.Active)DrawWorldDialogue(height);
        else if (preservationLine>=0) DrawPreservationDialogue(height);
        else if (!introductionCompleted || battle || lost || victories > 0)
            { SystemMessages.SetStatus(message); }
        if(battle && !worldDialogue.Active && (combatHack==null || !combatHack.Active))
        {
            var battleUI=BattleUISettings.Current;
            if(activeEnemy>=0&&activeEnemy<enemyObjects.Count&&enemyObjects[activeEnemy]!=null){
                var stats=enemyObjects[activeEnemy].GetComponent<EnemyCombatStats>();
                if(battleUI!=null&&battleUI.useNewArtwork){string kind=EnemyRoster.ResourceIds[EnemyRoster.KindForName(enemyObjects[activeEnemy].name)];BattleUIArt.Enemy(Layout.Resolve(battleUI.enemy,width/2,0),stats.CombatLabel,enemyHP,stats.maximumHP,Resources.Load<Texture2D>("Enemies/"+kind+"/idle_0"));}
                else NeonDungeonUI.EnemyHealth(Layout.Resolve(Layout.enemyHealth,width/2,0),enemyObjects[activeEnemy].name,enemyHP,stats.maximumHP,stats.CombatLabel);
            }
            DrawEnemyTargets(scale);
            // SPEEDRUN is selected from the in-battle HACK command list.
            GUI.enabled=!IntroductionActive&&!busy&&!lost&&!selectingEnemy;
            string[] labels=CombatLabels();
            if(battleUI!=null&&battleUI.useNewArtwork&&labels.Length>5){
                var nav=new Rect(battleUI.commandOffset.x,height+battleUI.commandOffset.y-5*(battleUI.commandSize.y+battleUI.commandGap)-26,battleUI.commandSize.x,24);
                if(GUI.Button(new Rect(nav.x,nav.y,nav.width/2-2,nav.height),"▲"))selectedCombatAction=(selectedCombatAction+labels.Length-1)%labels.Length;
                if(GUI.Button(new Rect(nav.center.x+2,nav.y,nav.width/2-2,nav.height),"▼"))selectedCombatAction=(selectedCombatAction+1)%labels.Length;
            }
            for(int i=0;i<labels.Length;i++)
            {
                string caption=i==selectedCombatAction ? "> "+labels[i]+" <" : labels[i];
                GUI.enabled=!selectingSpeedrun&&!IntroductionActive&&!busy&&!lost&&(!selectingEnemy||i==(pendingTargetHack!=null?1:0)) && (combatMenu!=CombatMenu.Items || i==CombatItemIndices().Count || itemCounts[CombatItemIndices()[i]]>0);
                if(combatMenu==CombatMenu.Hacks&&i==availableHacks.Length)GUI.enabled=GUI.enabled&&PlayerLevel>=SpeedrunUnlockLevel&&SpeedrunTargets().Count>0&&CurrentBits>=SpeedrunBitsCost;
                bool clicked;
                if(battleUI!=null&&battleUI.useNewArtwork){
                    string key=combatMenu==CombatMenu.Main?(i==0?"Attack":i==1?"Hack":i==2?"Item":"Flee"):combatMenu==CombatMenu.Hacks?(i==labels.Length-1?"Flee":"Hack"):"Item";
                    string title=labels[i],detail="";
                    if(combatMenu==CombatMenu.Main)detail=i==0?"Ataque normal":i==1?"Elegir habilidad":i==2?"Usar objeto":"Intentar escapar";
                    else if(combatMenu==CombatMenu.Hacks){if(i<availableHacks.Length){title=availableHackDefinitions[i].displayName;detail=availableHackDefinitions[i].bitsCost+" BITS";}else if(i==availableHacks.Length)detail=PlayerLevel<10?"Se desbloquea en NV 10":SpeedrunTargets().Count==0?"Sin objetivo válido":CurrentBits<5?"Faltan Bits":"5 BITS";else detail="Menú de combate";}
                    int visibleRows=Mathf.Min(5,labels.Length);int first=Mathf.Clamp(selectedCombatAction-visibleRows+1,0,Mathf.Max(0,labels.Length-visibleRows));
                    if(i<first||i>=first+visibleRows)continue;
                    var rect=new Rect(battleUI.commandOffset.x,height+battleUI.commandOffset.y-(visibleRows-(i-first))*(battleUI.commandSize.y+battleUI.commandGap),battleUI.commandSize.x,battleUI.commandSize.y);
                    clicked=BattleUIArt.Command(rect,title,detail,key,i==selectedCombatAction,battleUI);
                }else clicked=NeonDungeonUI.Action(new Rect(Layout.combatOffset.x,height+Layout.combatOffset.y-(labels.Length-i)*(Layout.combatButtonSize.y+Layout.combatButtonGap),Layout.combatButtonSize.x,Layout.combatButtonSize.y),labels[i],i==selectedCombatAction);
                if(clicked) { selectedCombatAction=i; Action(i); }
            }
            GUI.enabled=true;
        }
        if(lost&&!IntroductionActive&&GUI.Button(new Rect(width/2-100,height/2,200,50),"Reiniciar")) RestartNewGame();
        GUI.color=new Color(0,0,0,.12f);


        GUI.color=Color.white;
        if(!levelComplete && !IntroductionActive && !battle && !lost && DoorInteractionAvailable(exitDoor))
            GUI.Box(new Rect(width/2-140,height-155,280,45),"[E] Abrir puerta");
        DrawRefugePrompt(width,height);DrawMapEntityPrompt(width,height);DrawItemReceipts(width,height);
        DrawPreservationPrompt(width,height);DrawPlayerLevelNotice(width);DrawHackUnlockNotice(width);
        if(enemyBarks!=null&&!IntroductionActive&&!levelComplete) {
            if(barkSpeaker!=null) {
                var visual=barkSpeaker.GetComponent<GhostEnemyVisual>();
                Vector3 screenPoint=view.WorldToScreenPoint(barkSpeaker.position+Vector3.up*((visual!=null?visual.WorldHeight:2)+.15f));
                if(screenPoint.z>0) {
                    Vector2 anchor=new Vector2(screenPoint.x/scale,(Screen.height-screenPoint.y)/scale);
                    float left=8,right=width-8;
                    // Neighbour midpoints reserve a separate horizontal lane for each speaker.
                    foreach(var other in enemyObjects) if(other!=null && other.transform!=barkSpeaker) {
                        var otherVisual=other.GetComponent<GhostEnemyVisual>();
                        Vector3 head=view.WorldToScreenPoint(other.transform.position+Vector3.up*((otherVisual!=null?otherVisual.WorldHeight:2)+.15f));
                        if(head.z<=0)continue;
                        float x=head.x/scale;
                        if(x<anchor.x)left=Mathf.Max(left,(x+anchor.x)*.5f+8);
                        else right=Mathf.Min(right,(x+anchor.x)*.5f-8);

                    }
                    enemyBarks.Draw(width,height,anchor,true,left,right);
                }
            }
        }
        DrawCheatPulse(width,height);
        if(levelComplete) DrawCompletion(width,height);
    }
}
