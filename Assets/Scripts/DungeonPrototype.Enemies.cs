using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class DungeonPrototype
{
    public EnemyEncounterSettings enemyEncounterSettings;
    EnemyEncounterSettings EnemySettings { get { if(enemyEncounterSettings==null)enemyEncounterSettings=Resources.Load<EnemyEncounterSettings>("Exploration/EnemyEncounterSettings");if(enemyEncounterSettings==null)enemyEncounterSettings=ScriptableObject.CreateInstance<EnemyEncounterSettings>();return enemyEncounterSettings; } }
    readonly List<int> enemyLives=new List<int>();
    readonly List<bool> alerted=new List<bool>();
    readonly List<float> respawnAt=new List<float>();
    readonly List<int> encounter=new List<int>();
    readonly Dictionary<int,Vector3> encounterHomes=new Dictionary<int,Vector3>();
    bool Occupied(Vector2Int cell,int except=-1)
    { for(int i=0;i<enemies.Count;i++)if(i!=except&&enemyObjects[i]!=null&&enemies[i]==cell)return true;return false; }
    bool CanEnemyMove(Vector2Int a,Vector2Int b,int index=-1)
    { return EnemySpacingAllows(b,index)&&Open(b)&&EnemyMayEnter(b)&&b!=position&&!Occupied(b)&&!DoorsBlock(World(a),World(b)); }
    bool SeesPlayer(Vector2Int cell)
    {
        Vector2Int delta=position-cell;
        if(Mathf.Abs(delta.x)+Mathf.Abs(delta.y)>EnemySettings.detectionSteps)return false;
        if(delta.x!=0&&delta.y!=0)return false;
        Vector2Int direction=new Vector2Int(System.Math.Sign(delta.x),System.Math.Sign(delta.y));
        while(cell!=position){Vector2Int next=cell+direction;if(!Open(next)||DoorsBlock(World(cell),World(next)))return false;cell=next;}
        return true;
    }
    Vector2Int PursuitStep(int index){return EnemyPathStep(index,position);}
    Vector2Int EnemyPathStep(int index,Vector2Int goal)
    {
        var queue=new Queue<Vector2Int>();var first=new Dictionary<Vector2Int,Vector2Int>();
        Vector2Int origin=enemies[index];if(origin==goal)return origin;queue.Enqueue(origin);first[origin]=origin;
        while(queue.Count>0)
        {
            Vector2Int at=queue.Dequeue();
            foreach(var direction in directions)
            {
                Vector2Int next=at+direction;
                if(!ExplorationDomainAllows(index,next)||!Open(next)||!EnemyMayEnter(next)||first.ContainsKey(next)||Occupied(next,index)||DoorsBlock(World(at),World(next)))continue;
                first[next]=at==origin?next:first[at];
                if(next==goal)return first[next]==position?origin:first[next];
                queue.Enqueue(next);
            }
        }
        return origin;
    }
    IEnumerator EnemyTurn()
    {
        var starts=new Dictionary<int,Vector3>();var targets=new Dictionary<int,Vector3>();
        for(int i=0;i<enemies.Count;i++)
        {
            if(enemyObjects[i]==null||slots[i].placement!=null&&slots[i].placement.holdPosition)continue;
            Vector2Int next;
            if(ExplorationProfile(i)==EnemyExplorationProfile.Legacy){
                if(SeesPlayer(enemies[i]))alerted[i]=true;
                next=enemies[i];
                if(alerted[i])next=PursuitStep(i);
                else {
                    int start=Random.Range(0,4);
                    for(int j=0;j<4;j++){var candidate=enemies[i]+directions[(start+j)%4];if(CanEnemyMove(enemies[i],candidate,i)){next=candidate;break;}}
                }
            }else next=ExplorationStep(i);
            if(next==enemies[i]||!CanEnemyMove(enemies[i],next,i))continue;
            starts[i]=enemyObjects[i].transform.position;targets[i]=World(next)-Vector3.up*1.5f;enemies[i]=next;
        }
        if(starts.Count>0)DungeonAudio.Play(DungeonAudio.Cue.Step);
        float elapsed=0;
        while(elapsed<EnemySettings.moveSeconds)
        {
            elapsed+=Time.deltaTime;
            foreach(var pair in starts)if(enemyObjects[pair.Key]!=null)enemyObjects[pair.Key].transform.position=Vector3.Lerp(pair.Value,targets[pair.Key],Mathf.Clamp01(elapsed/EnemySettings.moveSeconds));
            yield return null;
        }
    }
    bool FindSpawn(out Vector2Int result,bool nearby=false,DungeonRegion origin=null)
    {
        if(EnemySettings.encounterDirectorEnabled)return TryDirectedSpawn(out result,origin,nearby);
        var cells=new List<Vector2Int>();
        for(int z=1;z<map.Length-1;z++)for(int x=1;x<map[0].Length-1;x++)
        {
            var cell=new Vector2Int(x,z);
            if((origin==null?RegionAt(cell)!=null&&RegionAt(cell).CanSpawn:RegionAt(cell)==origin)&&!ReservedCell(cell)&&EnemySpacingAllows(cell)&&Open(cell)&&!Occupied(cell)&&(cell-position).sqrMagnitude>=9&&(!nearby||(cell-position).sqrMagnitude<=36))cells.Add(cell);
        }
        if(cells.Count==0){result=position;return false;}result=cells[Random.Range(0,cells.Count)];return true;
    }
    public EnemyZoneSettings enemyZoneSettings;
    EnemyZoneSettings Zone { get { return enemyZoneSettings!=null?enemyZoneSettings:EnemyZoneSettings.Current; } }
    int LiveEnemyCount {get{int count=0;foreach(var obj in enemyObjects)if(obj!=null)count++;return count;}}
    bool KindEnabled(int kind){return !(kind==3&&!EnemySettings.spawnBigTapaku||kind>=4&&!EnemySettings.spawnMakubas||kind==6&&!EnemySettings.spawnGranMakuba);}
    bool SpawnRandom(Vector2Int cell,DungeonRegion origin=null) {origin=origin??RegionAt(cell);if(origin==null||!origin.CanSpawn)return false;int before=enemyObjects.Count;int kind=origin.enemies.SelectKind(Random.value);if(kind<0)return false;AddEnemy(cell,visualKind:kind,origin:origin);return enemyObjects.Count>before;}
    int RandomEnemyKind(){return Zone.SelectKind(Random.value);}
    float nextRespawnScan;
    void UpdateEnemyRespawns()
    {
        if(battle||moving||pauseOpen||IntroductionActive||preservationLine>=0||worldDialogue.Active||levelComplete||lost||!DirectorAllowsRespawn())return;
        if(EnemySettings.encounterDirectorEnabled&&Time.unscaledTime<nextRespawnScan)return;nextRespawnScan=Time.unscaledTime+.5f;
        for(int i=0;i<respawnAt.Count;i++)
        {
            if(respawnAt[i]<0||Time.unscaledTime<respawnAt[i]||enemyObjects[i]!=null)continue;
            var slot=slots[i];bool customTimed=slot.placement!=null&&slot.placement.exploration!=null&&slot.placement.exploration.persistence==EnemyExplorationPersistence.TimedRespawn;if(!slot.CanRespawn||!slot.origin.CanSpawn||((!customTimed||slot.counts)&&Population(slot.origin.id)>=slot.origin.enemies.maximumEnemies))continue;
            Vector2Int cell;if(slot.placement!=null){cell=slot.placement.cell;if(Occupied(cell)||cell==position||EnemySettings.encounterDirectorEnabled&&!(customTimed?ExplorationPlacementRespawnValid(cell,slot.origin):DirectedSpawnCellValid(cell,slot.origin)))continue;}else if(!FindSpawn(out cell,false,slot.origin))continue;
            int before=enemyObjects.Count;if(slot.placement!=null)AddEnemy(cell,visualKind:ArrayIndex(slot.placement.speciesId),origin:slot.origin,placement:slot.placement);else SpawnRandom(cell,slot.origin);
            if(enemyObjects.Count==before)continue;defeatedEnemies.Remove(i);int last=enemyObjects.Count-1;enemyObjects[last].GetComponent<EnemyCombatStats>().instanceId=slot.instanceId;slots[last].instanceId=slot.instanceId;
            enemies[i]=enemies[last];enemyObjects[i]=enemyObjects[last];enemyLives[i]=enemyLives[last];alerted[i]=false;respawnAt[i]=-1;slots[i]=slots[last];
            enemies.RemoveAt(last);enemyObjects.RemoveAt(last);enemyLives.RemoveAt(last);alerted.RemoveAt(last);respawnAt.RemoveAt(last);slots.RemoveAt(last);DirectorRespawnOccurred();if(EnemySettings.encounterDirectorEnabled)return;
        }
    }
    bool TryEncounter()
    {
        var region=RegionAt(position);if(region!=null&&region.safeArea)return false;
        for(int i=0;i<enemies.Count;i++)if(enemyObjects[i]!=null&&(enemies[i]-position).sqrMagnitude<=1)
        { BeginEncounter(i);return true; }return false;
    }
    void BeginEncounter(int primary,IList<int> forcedGroup=null)
    {
        if(primary<0||primary>=enemyObjects.Count||enemyObjects[primary]==null||worldDialogue.Active)return;
        if(enemyObjects[primary].GetComponent<MakongEnemyVisual>()!=null&&MakongNarrativeAllowed&&!MakongIntroductionSeen(primary)&&MakongSpecialSettings.Current.encounterDialogue!=null&&MakongSpecialSettings.Current.encounterDialogue.Length>0){
            SystemMessages.Show(SystemMessageCode.BossWarning,"",0,DigitalIcon.Warning);ClearMovementInput();FocusConversationOn(enemyObjects[primary].transform);
            BeginMakongAmbient();worldDialogue.Begin(MakongSpecialSettings.Current.encounterDialogue,()=>FinishMakongIntroduction(primary,forcedGroup));return;
        }
        BeginEncounterCombat(primary,forcedGroup);
    }
    void BeginEncounterCombat(int primary,IList<int> forcedGroup=null)
    {
        if(primary<0||primary>=enemyObjects.Count||enemyObjects[primary]==null)return;
        BeginMakongBattleMusic(primary,forcedGroup);ResetConversationCamera();
        DirectorEncounterStarted();BeginEncounterXP();encounterDefeats=0;encounterCoinBonus=0;combatDropsAwarded=false;guaranteedEncounterDrops.Clear();encounter.Clear();encounterHomes.Clear();encounter.Add(primary);
        if(forcedGroup!=null)foreach(int member in forcedGroup)if(member!=primary&&member>=0&&member<enemyObjects.Count&&enemyObjects[member]!=null&&!encounter.Contains(member))encounter.Add(member);
        int wanted=forcedGroup!=null?encounter.Count:slots[primary].placement!=null?1:Random.Range(Mathf.Clamp(EnemySettings.minimumEnemies,1,3),Mathf.Clamp(EnemySettings.maximumEnemies,Mathf.Clamp(EnemySettings.minimumEnemies,1,3),3)+1);
        // Nearby map enemies join first. Additional varied opponents become normal map agents.
        for(int i=0;i<enemies.Count&&encounter.Count<wanted;i++)if(i!=primary&&enemyObjects[i]!=null&&EligibleJoiner(i,primary)&&(enemies[i]-enemies[primary]).sqrMagnitude<=16)encounter.Add(i);
        for(int i=0;i<enemies.Count&&encounter.Count<wanted;i++)if(enemyObjects[i]!=null&&EligibleJoiner(i,primary)&&!encounter.Contains(i))encounter.Add(i);
        var encounterZone=slots[primary].origin;
        while(encounter.Count<wanted&&Population(encounterZone.id)<encounterZone.enemies.maximumEnemies)
        { Vector2Int cell;if(!FindSpawn(out cell,true,encounterZone)&&!FindSpawn(out cell,false,encounterZone))break;if(!SpawnRandom(cell,encounterZone))break;encounter.Add(enemyObjects.Count-1); }
        foreach(int i in encounter){encounterHomes[i]=enemyObjects[i].transform.position;var makong=enemyObjects[i].GetComponent<MakongEnemyVisual>();if(makong!=null)makong.ResetCombatTactics();}
        activeEnemy=primary;enemyHP=enemyLives[primary];battle=true;combatMenu=CombatMenu.Main;selectedCombatAction=0;
        battleCameraHome=World(position);battleCameraFov=view.fieldOfView;
        formationCenter=BattleStageOrigin;
        Vector3 forward=Vector3.forward;
        if(forward.sqrMagnitude<.01f)forward=Quaternion.Euler(0,heading*90,0)*Vector3.forward;
        formationForward=forward.normalized;formationRight=Vector3.Cross(Vector3.up,formationForward);
        view.transform.rotation=Quaternion.LookRotation(formationForward);
        busy=true;StartCoroutine(EnterEncounter());
        DungeonAudio.Play(DungeonAudio.Cue.Encounter);glitch=.45f;message="Â¡Enemigos a la vista!";Bark(EnemyBarkCategory.CombatStart);
    }
    Vector3 battleCameraHome,formationCenter,formationForward,formationRight;
    float battleCameraFov;
    bool selectingEnemy;
    CombatHack pendingTargetHack;
    Texture2D targetArrow;
    readonly Dictionary<int,Vector3> encounterScales=new Dictionary<int,Vector3>();
    int CentralMakongSlot {get{for(int i=0;i<encounter.Count;i++){var obj=enemyObjects[encounter[i]];if(obj!=null&&obj.GetComponent<MakongEnemyVisual>()!=null)return i;}return -1;}}
    float FormationOffset(int n)
    {
        int center=CentralMakongSlot;
        if(center>=0){
            if(n==center)return 0;
            int order=0;for(int i=0;i<n;i++)if(i!=center)order++;
            int side=order%2==0?-1:1;float distance=FormationWidth(center)*.5f+.4f;
            for(int i=0,other=0;i<n;i++){if(i==center)continue;if(other%2==order%2)distance+=FormationWidth(i)+.4f;other++;}
            return side*(distance+FormationWidth(n)*.5f);
        }
        float total=0;for(int i=0;i<encounter.Count;i++)total+=FormationWidth(i);total+=Mathf.Max(0,encounter.Count-1)*.4f;
        float x=-total*.5f;for(int i=0;i<n;i++)x+=FormationWidth(i)+.4f;return x+FormationWidth(n)*.5f;
    }
    Vector3 FormationPosition(int n){return formationCenter+formationRight*FormationOffset(n);}
    float FormationWidth(int n){var obj=enemyObjects[encounter[n]];return obj!=null?EnemySpaceLayout.Width(obj.GetComponent<GhostEnemyVisual>().Kind,EnemySizeSettings.Current):2.5f;}
    float BattleWidth {get{if(CentralMakongSlot>=0){float edge=0;for(int i=0;i<encounter.Count;i++)edge=Mathf.Max(edge,Mathf.Abs(FormationOffset(i))+FormationWidth(i)*.5f);return Mathf.Max(9,edge*2+1.5f);}float width=1.5f;for(int i=0;i<encounter.Count;i++)width+=FormationWidth(i);return Mathf.Max(9,width+Mathf.Max(0,encounter.Count-1)*.4f);}}
    void SetCeilingReflection(float height){foreach(var reflection in view.GetComponents<DungeonFloorReflection>())if(reflection.IsCeiling)reflection.SetPlaneHeight(height);}

    float FormationScale { get { return 1f; } }
    IEnumerator EnterEncounter()
    {
        selectingSpeedrun=false;selectingEnemy=false;pendingTargetHack=null;encounterScales.Clear();
        EnsureBattleStage();battleStage.SetActive(true);
        foreach(int i in encounter){encounterScales[i]=enemyObjects[i].transform.localScale;enemyObjects[i].transform.localScale=Vector3.one*FormationScale;}
        glitch=.85f;
        // Switch spaces under the glitch, then animate the camera within the wider corridor.
        yield return new WaitForSecondsRealtime(.1f);
        view.transform.rotation=Quaternion.identity;
        float distance=Mathf.Max(4.8f,BattleWidth*.5f/(Mathf.Tan(34*Mathf.Deg2Rad)*view.aspect)+.8f);
        Vector3 from=BattleStageOrigin+new Vector3(0,1.5f,-distance-1.2f);
        Vector3 to=BattleStageOrigin+new Vector3(0,1.5f,-distance);
        SetCeilingReflection(EnemySpaceLayout.CombatHeight(DungeonVisualSettings.Current,EnemySizeSettings.Current));
        float fov=68;
        view.transform.position=from;
        for(int n=0;n<encounter.Count;n++)enemyObjects[encounter[n]].transform.position=formationCenter+formationForward*(n==0?0:.6f);
        for(float elapsed=0;elapsed<.65f;elapsed+=Time.unscaledDeltaTime)
        {
            float k=Mathf.SmoothStep(0,1,elapsed/.65f);view.transform.position=Vector3.Lerp(from,to,k);view.fieldOfView=Mathf.Lerp(battleCameraFov,fov,k);
            for(int n=0;n<encounter.Count;n++){
                float spread=Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-.12f)/.5f));
                var enemy=enemyObjects[encounter[n]];if(enemy!=null)enemy.transform.position=Vector3.Lerp(formationCenter+formationForward*(n==0?0:.6f),FormationPosition(n),spread);
            }
            yield return null;
        }
        view.transform.position=to;view.fieldOfView=fov;for(int n=0;n<encounter.Count;n++)if(enemyObjects[encounter[n]]!=null)enemyObjects[encounter[n]].transform.position=FormationPosition(n);busy=false;
    }
    void ArrangeEncounter()
    {
        // Keep the original formation slots when a member dies; no instant regrouping.
        if(encounterScales.Count>0)return;
        for(int n=0;n<encounter.Count;n++)if(enemyObjects[encounter[n]]!=null){enemyObjects[encounter[n]].transform.position=FormationPosition(n);enemyObjects[encounter[n]].transform.localScale=Vector3.one*FormationScale;}
    }
    void RestoreEncounter()
    {
        selectingSpeedrun=false;selectingEnemy=false;pendingTargetHack=null;ClearCombatSummons();
        foreach(var pair in encounterHomes)if(enemyObjects[pair.Key]!=null){enemyObjects[pair.Key].transform.position=pair.Value;Vector3 scale;if(encounterScales.TryGetValue(pair.Key,out scale))enemyObjects[pair.Key].transform.localScale=scale;}
        encounter.Clear();encounterHomes.Clear();encounterScales.Clear();
    }
    IEnumerator RestoreBattleCamera()
    {
        moving=true;Vector3 from=view.transform.position;glitch=.5f;
        for(float elapsed=0;elapsed<.18f;elapsed+=Time.unscaledDeltaTime){while(pauseOpen)yield return null;view.transform.position=from-Vector3.forward*Mathf.SmoothStep(0,.4f,elapsed/.18f);yield return null;}
        view.transform.position=battleCameraHome;view.transform.rotation=Quaternion.Euler(0,heading*90,0);view.fieldOfView=battleCameraFov;
        if(battleStage!=null)battleStage.SetActive(false);SetCeilingReflection(corridorHeight);moving=false;
    }
    void ConfirmEnemyTarget()
    { if(!selectingEnemy||busy||lost)return;if(selectingSpeedrun){TryExecuteSpeedrun(activeEnemy);return;}selectingEnemy=false;
        var hack=pendingTargetHack;pendingTargetHack=null;
        if(hack!=null)BeginTargetHack(hack);else{busy=true;StartCoroutine(ResolveCombatDamage(PlayerStrength,"ATTACK"));}
    }
    void BeginTargetHack(CombatHack hack)
    {
        if(busy||(combatHack!=null&&combatHack.Active))return;
        if(hack==null||!CanExecuteHack(hack)){busy=false;message="HACK no disponible.";return;}
        int target=activeEnemy;
        if(!battle||target<0||target>=enemyObjects.Count||enemyObjects[target]==null||enemyLives[target]<=0){message="Objetivo no disponible.";return;}
        int bitsCost=HackBitsCost(hack);if(CurrentBits<bitsCost){busy=false;message="Bits insuficientes: necesitas "+bitsCost+".";return;}
        var opponent=enemyObjects[target];
        combatHack=hack;busy=true;
        var jump=hack as EnemyJumpHack;if(jump!=null){jump.SetOpponent(opponent);jump.ConfigureAttack(PlayerStrength,opponent.GetComponent<EnemyCombatStats>().defense);}
        var overclock=hack as OverclockHack;if(overclock!=null)overclock.SetOpponent(opponent);
        bool resolved=false;BeginHackSystemSession(hack);
        hack.Begin(damage=>{
            if(resolved)return;resolved=true;
            if(!battle||target>=enemyObjects.Count||enemyObjects[target]!=opponent||opponent==null||enemyLives[target]<=0){CompleteHackSystemSession(HackExecutionOutcome.Interrupted);busy=false;return;}
            CompleteHackSystemSession(hack.LastOutcome);activeEnemy=target;
            StartCoroutine(ResolveCombatDamage(damage,"HACK"));
        });
        if(hack.Active||resolved)playerBits=CurrentBits-bitsCost;
        if(!hack.Active){if(!resolved)CompleteHackSystemSession(HackExecutionOutcome.Interrupted);busy=false;}
    }
    static readonly Vector3 BattleStageOrigin=new Vector3(1000,0,1000);
    GameObject battleStage;
    Material battleCeiling;
    void EnsureBattleStage()
    {
        if(battleStage==null){
            battleStage=new GameObject("Wide combat corridor");battleStage.transform.SetParent(transform,false);
            battleCeiling=new Material(ceiling);battleCeiling.SetFloat("_ContinuousFloorGrid",1);battleCeiling.SetVector("_FloorGridRegion",floor.GetVector("_FloorGridRegion"));battleCeiling.SetFloat("_FloorGridSize",floor.GetFloat("_FloorGridSize"));
            for(int n=0;n<3;n++)for(int side=-1;side<=1;side+=2){var lamp=new GameObject("Battle torch");lamp.transform.SetParent(battleStage.transform,false);lamp.transform.position=BattleStageOrigin+new Vector3(side*4.4f,0,2+n*7);lamp.transform.rotation=Quaternion.LookRotation(new Vector3(-side,0,0));lamp.AddComponent<DungeonWallLamp>();}
        }
        float width=BattleWidth,height=EnemySpaceLayout.CombatHeight(DungeonVisualSettings.Current,EnemySizeSettings.Current);
        ResizeStageBox("Battle floor",new Vector3(0,-.1f,8),new Vector3(width,.2f,30),floor);
        ResizeStageBox("Battle ceiling",new Vector3(0,height+.1f,8),new Vector3(width,.2f,30),battleCeiling);
        ResizeStageBox("Battle left wall",new Vector3(-width*.5f-.1f,height*.5f,8),new Vector3(.2f,height,30),wall);
        ResizeStageBox("Battle right wall",new Vector3(width*.5f+.1f,height*.5f,8),new Vector3(.2f,height,30),wall);
        ResizeStageBox("Battle far wall",new Vector3(0,height*.5f,23),new Vector3(width,height,.2f),wall);
        int torch=0;foreach(Transform child in battleStage.transform)if(child.name=="Battle torch"){int side=torch%2==0?-1:1;child.position=BattleStageOrigin+new Vector3(side*(width*.5f-.1f),0,2+(torch/2)*7);torch++;}
        var retro=GetComponent<RetroDungeonWallComposer>();if(retro!=null)retro.ApplyBattleStage(battleStage,BattleStageOrigin,width,height);
    }
    void ResizeStageBox(string name,Vector3 offset,Vector3 size,Material material){var existing=battleStage.transform.Find(name);if(existing==null)StageBox(name,offset,size,material);else{existing.position=BattleStageOrigin+offset;existing.localScale=size;}}

    void StageBox(string name,Vector3 offset,Vector3 size,Material material)
    { var box=Box(name,BattleStageOrigin+offset,size,material);box.transform.SetParent(battleStage.transform,true); }
    void SelectEnemy(int direction)
    { var choices=selectingSpeedrun?SpeedrunTargets():encounter;if(choices.Count==0)return;int n=choices.IndexOf(activeEnemy);activeEnemy=choices[(n+direction+choices.Count)%choices.Count];enemyHP=enemyLives[activeEnemy]; }
    void DrawEnemyTargets(float scale)
    {
        var ui=BattleUISettings.Current;if(ui!=null&&ui.useNewArtwork)targetArrow=BattleUIArt.Texture("TargetArrow");
        if(targetArrow==null)targetArrow=Resources.Load<Texture2D>("UI/MenuUI/TargetArrow");
        foreach(int index in encounter)
        {
            if(enemyObjects[index]==null)continue;
            Vector3 point=view.WorldToScreenPoint(enemyObjects[index].transform.position+Vector3.up*(enemyObjects[index].GetComponent<GhostEnemyVisual>().WorldHeight+.2f));
            if(point.z<=0)continue;
            Rect rect=new Rect(point.x/scale-85,(Screen.height-point.y)/scale-20,170,30);
            if(index==activeEnemy&&targetArrow!=null&&!busy&&(!selectingSpeedrun||SpeedrunEligible(index))){GUI.color=Color.white;GUI.DrawTexture(new Rect(rect.center.x-14,rect.y-24+Mathf.Sin(Time.unscaledTime*5)*3,28,20),targetArrow,ScaleMode.ScaleToFit,true);}
            GUI.enabled=!busy&&!lost&&(!selectingSpeedrun||SpeedrunEligible(index));
            if(GUI.Button(rect,GUIContent.none,GUIStyle.none)){activeEnemy=index;enemyHP=enemyLives[index];}
        }
        GUI.enabled=true;
    }
}
