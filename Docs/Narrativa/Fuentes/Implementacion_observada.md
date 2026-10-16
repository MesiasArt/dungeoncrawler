# Fuentes de narrativa encontradas en el proyecto

Revisión estática: 2026-10-15. Estado: IMPLEMENTACIÓN OBSERVADA, pendiente de aprobación narrativa. Los bloques conservan literalmente el contenido de cada archivo; escapes YAML y C# son representación del archivo, no cambios editoriales. Los valores por defecto de scripts pueden diferir de los assets actuales. La existencia de una línea no demuestra que se reproduzca en todas las partidas. No se ejecutó Unity ni se cambió el proyecto para esta revisión.

## Assets/Scripts/IntroSequence.cs

SHA-256: `7001E158ED07EA2004DAF4E83091E07C26DDFBF926BE69FC5551ED96A4DEEF3E`

```text
namespace VirtualDungeon
{
    public enum GameStartKind { NewGame, SavedGame, SceneContinuation }

    // The menu/save loader must select the start kind BEFORE loading the game scene.
    // Consuming it prevents replaying the intro on subsequent scene loads.
    public static class GameStartContext
    {
        static GameStartKind next = GameStartKind.NewGame;
        public static void PrepareNewGame() { next = GameStartKind.NewGame; }
        public static void PrepareSavedGame() { next = GameStartKind.SavedGame; }
        public static GameStartKind Consume()
        {
            GameStartKind result = next;
            next = GameStartKind.SceneContinuation;
            return result;
        }
    }

    public sealed class IntroLine
    {
        public readonly string Key;
        public readonly string Speaker;
        public readonly string Text;
        public readonly float PauseBefore;
        public readonly bool InterferenceBefore;
        public IntroLine(string key, string speaker, string text, float pauseBefore = 0f, bool interferenceBefore = false)
        {
            Key = key; Speaker = speaker; Text = text;
            PauseBefore = pauseBefore; InterferenceBefore = interferenceBefore;
        }
    }

    // Stable keys keep the script ready for a future localization table.
    // There is no language system in the current project: Spanish is the source text.
    public sealed class IntroSequence
    {
        static readonly IntroLine[] lines = {
            new IntroLine("intro.01", "GAMERBOY", "Ugh... ¿Qué pasó?"),
            new IntroLine("intro.02", "GAMERBOY", "¿Dónde demonios estoy?"),
            new IntroLine("intro.03", "GAMERBOY", "Espera... ¿Esto es un videojuego?"),
            new IntroLine("intro.04", "GAMERBOY", "No recuerdo haber iniciado ninguno..."),
            new IntroLine("intro.05", "???", "...¿Puedes escucharme?", .8f),
            new IntroLine("intro.06", "GAMERBOY", "¿Eh? ¿Quién dijo eso?"),
            new IntroLine("intro.07", "???", "¡Funcionó! ¡La transferencia fue un éxito!"),
            new IntroLine("intro.08", "GAMERBOY", "¿Transferencia? ¿De qué estás hablando?"),
            new IntroLine("intro.09", "???", "No hay tiempo para explicaciones. Necesitamos tu ayuda."),
            new IntroLine("intro.10", "GAMERBOY", "¿Ayuda? ¡Ni siquiera sé dónde estoy!"),
            new IntroLine("intro.11", "???", "Estás dentro de la Red. Este mundo está muriendo."),
            new IntroLine("intro.12", "GAMERBOY", "¿Dentro de Internet? ¡¿Qué?!"),
            new IntroLine("intro.13", "???", "Busca a los Guardianes. Ellos tienen las respuestas."),
            new IntroLine("intro.14", "GAMERBOY", "¡Espera! ¿Qué guardianes? ¿Cómo salgo de aquí?"),
            new IntroLine("intro.15", "???", "Y, sobre todo... no confíes en..."),
            new IntroLine("intro.16", "SISTEMA", "CONNECTION LOST", .75f, true),
            new IntroLine("intro.17", "GAMERBOY", "¿Hola? ¿Sigues ahí?", .4f),
            new IntroLine("intro.18", "GAMERBOY", "...Genial."),
            new IntroLine("intro.19", "GAMERBOY", "Atrapado en Internet."),
            new IntroLine("intro.20", "GAMERBOY", "Espero que al menos haya un botón para salir.")
        };
        int index;
        float pauseRemaining;
        public bool Active { get; private set; }
        public bool Waiting { get { return Active && pauseRemaining > 0f; } }
        public bool Interference { get { return Waiting && lines[index].InterferenceBefore; } }
        public IntroLine Current { get { return Active && !Waiting ? lines[index] : null; } }
        public IntroSequence(GameStartKind kind) { Active = kind == GameStartKind.NewGame; }

        public void Skip()
        {
            Active=false; pauseRemaining=0;
        }

        // Time only ends pauses. Each visible line always needs a fresh confirmation.
        // A press made during a pause is discarded, including on its final frame.
        public void Tick(float elapsed, bool confirm)
        {
            if (!Active) return;
            if (Waiting)
            {
                pauseRemaining = System.Math.Max(0f, pauseRemaining - System.Math.Max(0f, elapsed));
                return;
            }
            if (!confirm) return;
            index++;
            if (index == lines.Length) { Active = false; return; }
            pauseRemaining = lines[index].PauseBefore;
        }
    }
}

```

## Assets/Scripts/RefugeSettings.cs

SHA-256: `2AEBB7D3BC0F230B38C8FD2FEB4663154722B48590B9F97C7CF05E14532BE026`

```text
using UnityEngine;
[System.Serializable] public class RefugeNpcDefinition {
 public string id,displayName;public Vector2Int cell;public Texture2D portrait;
 public string visualSpeciesId;
 [TextArea(2,5)] public string[] conversations;
 [TextArea(2,4)] public string lastWords;
 public Color placeholderColor=new Color(.25f,.65f,.85f);
}
[System.Serializable] public class TransferNodeDefinition {public string id,displayName;public Vector2Int cell;[Range(0,3)] public int arrivalHeading;public bool triggersRefugeTragedy;public System.Collections.Generic.List<string> revealedRegionIds=new System.Collections.Generic.List<string>();}
[CreateAssetMenu(menuName="Virtual Dungeon/Refugio y narrativa")]
public class RefugeSettings:ScriptableObject {
 public string regionId="zone-0",devotoPlacementId="refuge.devoto",rewardId="refuge.core";
 public Vector2Int startCell=new Vector2Int(17,2),saveCell=new Vector2Int(14,2),devotoCell=new Vector2Int(17,3),exitDestination=new Vector2Int(1,1);
 public Vector2 exitDoorCell=new Vector2(17,9.5f);public int startHeading,exitHeading;
 [Range(1,100)] public int devotoLevel=4;
 public bool automaticDevotoEncounter=true;
 public int refugePassageRow=5;
 [Min(1)] public int devotoApproachRange=2;
 public RefugeNpcDefinition[] npcs=new RefugeNpcDefinition[0];
 public TransferNodeDefinition[] nodes=new TransferNodeDefinition[0];
 public WorldDialogueLine[] exitDialogue=new[]{new WorldDialogueLine("CENTINELA","¡Espera! ¿De verdad vas a salir?"),new WorldDialogueLine("GAMERBOY","¿Y quedarme aquí para siempre? Paso."),new WorldDialogueLine("OPTIMISTA","Entonces... buena suerte."),new WorldDialogueLine("DEVOTO","Ten cuidado ahí fuera."),new WorldDialogueLine("DEVOTO","A veces lo peligroso no parece peligroso.")};
 public WorldDialogueLine[] devotoDialogue=new[]{new WorldDialogueLine("DEVOTO","Has vuelto."),new WorldDialogueLine("GAMERBOY","¿Qué les pasó a los demás?"),new WorldDialogueLine("DEVOTO","Tenían miedo."),new WorldDialogueLine("DEVOTO","Muchísimo miedo."),new WorldDialogueLine("GAMERBOY","¿Qué hiciste?"),new WorldDialogueLine("DEVOTO","Los ayudé a descansar."),new WorldDialogueLine("GAMERBOY","...Tú los mataste."),new WorldDialogueLine("DEVOTO","No lo entiendes."),new WorldDialogueLine("DEVOTO","Ahora están en paz.")};
 public WorldDialogueLine[] savePointIntro=new[]{new WorldDialogueLine("GAMERBOY","¿Una terminal?"),new WorldDialogueLine("GAMERBOY","A ver qué hace..."),new WorldDialogueLine("SISTEMA","Usuario detectado."),new WorldDialogueLine("SISTEMA","Restaurando integridad..."),new WorldDialogueLine("SISTEMA","Progreso registrado. Restauración completada."),new WorldDialogueLine("GAMERBOY","¡Oh! ¿Un punto de guardado?"),new WorldDialogueLine("GAMERBOY","Bueno, al menos este lugar tiene algo familiar.")};
 public WorldDialogueLine[] sentinelTeleportDialogue=new[]{new WorldDialogueLine("CENTINELA","¿Ves ese teletransportador?"),new WorldDialogueLine("CENTINELA","Desactivamos los nodos para que esas cosas no pudieran seguirnos."),new WorldDialogueLine("CENTINELA","Mientras estén apagados, este lugar seguirá oculto."),new WorldDialogueLine("GAMERBOY","¿Y si alguien necesita regresar?"),new WorldDialogueLine("CENTINELA","Nadie que haya salido ha regresado.")};
 public WorldDialogueLine[] refugeNodeIntro=new[]{new WorldDialogueLine("GAMERBOY","¿Así que esto es un teletransportador?"),new WorldDialogueLine("GAMERBOY","Parece que todavía tiene energía..."),new WorldDialogueLine("SISTEMA","NODO DE TRANSFERENCIA — 00"),new WorldDialogueLine("SISTEMA","Estado: DESACTIVADO."),new WorldDialogueLine("SISTEMA","Conexión con la red: INTERRUMPIDA."),new WorldDialogueLine("SISTEMA","¿Reactivar nodo local?")};
 public WorldDialogueLine[] refugeNodeActivate=new[]{new WorldDialogueLine("GAMERBOY","Dijeron que era peligroso encenderlo..."),new WorldDialogueLine("GAMERBOY","Pero si encuentro otro, podría usarlo para regresar."),new WorldDialogueLine("GAMERBOY","Supongo que vale la pena el riesgo."),new WorldDialogueLine("SISTEMA","Nodo REFUGIO activado."),new WorldDialogueLine("SISTEMA","Buscando destinos..."),new WorldDialogueLine("SISTEMA","Ningún otro nodo disponible."),new WorldDialogueLine("GAMERBOY","Bueno... ya tengo cómo volver."),new WorldDialogueLine("GAMERBOY","Ahora solo falta encontrar adónde ir.")};
 public WorldDialogueLine[] refugeNodeDecline=new[]{new WorldDialogueLine("GAMERBOY","Mejor no tocar nada todavía.")};
 public WorldDialogueLine[] zoneTwoPlayerDialogue=new[]{new WorldDialogueLine("SISTEMA","Nodo de transferencia registrado: ZONA 2."),new WorldDialogueLine("SISTEMA","Conexión establecida."),new WorldDialogueLine("SISTEMA","Destino disponible: REFUGIO."),new WorldDialogueLine("GAMERBOY","¡Perfecto! Ahora puedo regresar.")};
 public WorldDialogueLine[] zoneTwoDevoteeDialogue=new[]{new WorldDialogueLine("SISTEMA","Nodo de transferencia registrado: ZONA 2."),new WorldDialogueLine("SISTEMA","Buscando conexiones..."),new WorldDialogueLine("SISTEMA","Destino disponible: REFUGIO."),new WorldDialogueLine("GAMERBOY","¿Eh? ¿El refugio?"),new WorldDialogueLine("GAMERBOY","Pensé que ese teletransportador estaba apagado..."),new WorldDialogueLine("SISTEMA","Nodo REFUGIO: ACTIVO."),new WorldDialogueLine("GAMERBOY","¿Alguien lo encendió?"),new WorldDialogueLine("GAMERBOY","Será mejor que vaya a echar un vistazo.")};
 public WorldDialogueLine[] tragedyArrivalDialogue=new[]{new WorldDialogueLine("GAMERBOY","¿Hola...?"),new WorldDialogueLine("GAMERBOY","¿Dónde están todos?")};
 public WorldDialogueLine[] tragedyBodyDialogue=new[]{new WorldDialogueLine("GAMERBOY","No..."),new WorldDialogueLine("GAMERBOY","¿Qué pasó aquí?")};
 public WorldDialogueLine[] devotoPlayerDialogue=new[]{new WorldDialogueLine("DEVOTO","Has vuelto."),new WorldDialogueLine("GAMERBOY","¿Qué les pasó a los demás?"),new WorldDialogueLine("DEVOTO","Tenían miedo."),new WorldDialogueLine("DEVOTO","Muchísimo miedo."),new WorldDialogueLine("GAMERBOY","¿Qué hiciste?"),new WorldDialogueLine("DEVOTO","Los ayudé a descansar."),new WorldDialogueLine("GAMERBOY","...Tú los mataste."),new WorldDialogueLine("DEVOTO","No lo entiendes."),new WorldDialogueLine("DEVOTO","Ahora están en paz."),new WorldDialogueLine("GAMERBOY","¿Por qué hiciste esto?"),new WorldDialogueLine("DEVOTO","Ella me enseñó que el miedo es un error."),new WorldDialogueLine("DEVOTO","Y los errores deben corregirse.")};
 public WorldDialogueLine[] devotoDevoteeDialogue=new[]{new WorldDialogueLine("DEVOTO","Has vuelto."),new WorldDialogueLine("GAMERBOY","¿Qué les pasó a los demás?"),new WorldDialogueLine("DEVOTO","Tenían miedo."),new WorldDialogueLine("DEVOTO","Muchísimo miedo."),new WorldDialogueLine("GAMERBOY","¿Tú encendiste el teletransportador?"),new WorldDialogueLine("DEVOTO","Sí."),new WorldDialogueLine("DEVOTO","Pensé que quizás querrías volver."),new WorldDialogueLine("GAMERBOY","¿Para qué?"),new WorldDialogueLine("DEVOTO","Para que vieras lo tranquilos que están ahora."),new WorldDialogueLine("GAMERBOY","...Tú los mataste."),new WorldDialogueLine("DEVOTO","No lo entiendes."),new WorldDialogueLine("DEVOTO","Ahora están en paz."),new WorldDialogueLine("DEVOTO","Y tú también puedes estarlo.")};
 public static RefugeSettings Current {get{return Resources.Load<RefugeSettings>("Exploration/Zones/00-Zona0/Refugio");}}
}
[System.Serializable] public class RefugeNpcState {public string id;public int talks;public bool lastWordsHeard;}
[System.Serializable] public class RefugeProgress {
 public System.Collections.Generic.List<string> mapRevealedNodes=new System.Collections.Generic.List<string>();
 public bool preservationSignRead;public System.Collections.Generic.List<string> departureGiftIds=new System.Collections.Generic.List<string>();
 public int narrativeVersion;
 public bool savePointIntroSeen,refugeTeleportActivatedByPlayer,refugeTeleportActivatedByDevotee,zone2TeleportActivated,refugeTragedyDiscovered,firstBodyDiscovered,sentinelTeleportSeen,devotoConfrontationSeen;
 public bool exitDialogueCompleted,refugeTragedyTriggered,devotoDefeated,rewardObtained;
 public System.Collections.Generic.List<string> unlockedNodes=new System.Collections.Generic.List<string>();
 public System.Collections.Generic.List<RefugeNpcState> npcs=new System.Collections.Generic.List<RefugeNpcState>();
}

```

## Assets/Scripts/DungeonPrototype.Refuge.cs

SHA-256: `6F9FC2975B607884EB01B2AE466A1AB88DEDF5BB6FFB9867889C0003E97B2BD3`

```text
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 readonly HashSet<string> disappearingBodies=new HashSet<string>();
 RefugeSettings refugeSettings;RefugeProgress refuge=new RefugeProgress();DungeonDoor refugeDoor;
 readonly WorldDialogue worldDialogue=new WorldDialogue();readonly Dictionary<string,GameObject> refugeActors=new Dictionary<string,GameObject>();
 bool devotoApproachLatched,refugeExitLooking;
 Quaternion refugeExitViewRotation;string refugeExitLastNpc;
 bool transferMenu,devotoLowLifeSpoken;int transferSelection;List<TransferNodeDefinition> transferChoices=new List<TransferNodeDefinition>();
 RefugeNpcState NpcState(string id){var state=refuge.npcs.Find(n=>n.id==id);if(state==null){state=new RefugeNpcState{id=id};refuge.npcs.Add(state);}return state;}
 void CreateRefuge(){refugeSettings=RefugeSettings.Current;if(refugeSettings==null)return;refuge.narrativeVersion=1;
  foreach(var npc in refugeSettings.npcs){var root=new GameObject("NPC placeholder — "+npc.displayName);root.transform.SetParent(transform,false);root.transform.position=World(npc.cell)-Vector3.up*1.5f;
   int visualKind=Array.IndexOf(EnemyRoster.ResourceIds,npc.visualSpeciesId);
   if(visualKind>=0){EnemyRoster.AddVisual(root,visualKind);root.AddComponent<DungeonNpcFacing>().viewCamera=view;root.transform.localScale=Vector3.one*.75f;}
   else{var body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.transform.SetParent(root.transform,false);body.transform.localPosition=Vector3.up*.9f;body.transform.localScale=new Vector3(.65f,.9f,.65f);body.GetComponent<Renderer>().material=Glow(npc.placeholderColor);Destroy(body.GetComponent<Collider>());}
   refugeActors[npc.id]=root;}
  if(LegacyObjectEnabled("refuge.save"))RefugeDevice(refugeSettings.saveCell,"TERMINAL DE RESTAURACIÓN",new Color(0,.8f,.4f),false);
  foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id))RefugeDevice(node.cell,"NODO "+node.displayName,new Color(.7f,.15f,1),true);
  if(!LegacyObjectEnabled("refuge.exit"))return;
  var doorRoot=new GameObject("Refugio — salida a Zona 1");doorRoot.transform.SetParent(transform,false);doorRoot.transform.position=new Vector3(refugeSettings.exitDoorCell.x*Cell,0,refugeSettings.exitDoorCell.y*Cell);
  refugeDoor=doorRoot.AddComponent<DungeonDoor>();refugeDoor.settings=exitDoorSettings;refugeDoor.closeAfterPassage=true;refugeDoor.doorwayWidth=Cell;refugeDoor.interactionDistance=Cell*.5f+.25f;refugeDoor.openingTravel=corridorHeight+.2f;
  var panel=Box("Refuge door panel",doorRoot.transform.position+Vector3.up*corridorHeight*.5f,new Vector3(Cell-.15f,corridorHeight,.18f),wall);panel.transform.SetParent(doorRoot.transform,true);refugeDoor.panel=panel.transform;refugeDoor.blocker=panel.GetComponent<Collider>();refugeDoor.Initialize();refugeDoor.Passed+=()=>{message="Zona 1";};
 }
 void RefugeDevice(Vector2Int cell,string name,Color color,bool portal){
  var root=new GameObject(name);root.transform.SetParent(transform,false);root.transform.position=World(cell)-Vector3.up*1.5f;
  if(!portal){Vector2Int wallDirection=Vector2Int.left;int nearest=100;
   foreach(var direction in directions)for(int distance=1;distance<=6;distance++){if(Open(cell+direction*distance))continue;if(distance<nearest){nearest=distance;wallDirection=direction;}break;}
   if(nearest<100)root.transform.position+=new Vector3(wallDirection.x,0,wallDirection.y)*(nearest*Cell-Cell*.5f-.18f);
   root.transform.rotation=Quaternion.LookRotation(new Vector3(-wallDirection.x,0,-wallDirection.y));
  }
  var visual=root.AddComponent<RefugeDeviceVisual>();visual.portal=portal;visual.deviceName=name;visual.Build();
 }
 bool RefugeNpcBlocks(Vector2Int cell){if(MapNpcBlocks(cell))return true;if(refugeSettings==null)return false;foreach(var npc in refugeSettings.npcs)if(npc.cell==cell&&refugeActors.ContainsKey(npc.id)&&refugeActors[npc.id].activeSelf)return true;return false;}
 void WarpPlayer(Vector2Int cell,int direction=0){if(!Open(cell))return;ResetConversationCamera();position=cell;heading=Mathf.Clamp(direction,0,3);view.transform.position=World(cell);view.transform.rotation=Quaternion.Euler(0,heading*90,0);RevealVisitedCell(cell);glitch=.15f;moving=false;}
 void BeginNpcTalk(string id){if(!LegacyInteractionAdapter(id,()=>BeginNpcTalk(id)))return;GameObject focusActor;if(refugeActors.TryGetValue(id,out focusActor))FocusConversationOn(focusActor.transform);if(id=="centinela"&&!refuge.refugeTragedyTriggered&&!refuge.sentinelTeleportSeen){worldDialogue.Begin(refugeSettings.sentinelTeleportDialogue,()=>refuge.sentinelTeleportSeen=true);return;}var npc=Array.Find(refugeSettings.npcs,n=>n.id==id);if(npc==null)return;var state=NpcState(id);if(refuge.refugeTragedyTriggered){if(id=="devoto"||state.lastWordsHeard)return;var bodyLines=new List<WorldDialogueLine>();if(!refuge.firstBodyDiscovered){bodyLines.AddRange(refugeSettings.tragedyBodyDialogue);refuge.firstBodyDiscovered=true;}bodyLines.Add(new WorldDialogueLine(npc.displayName,npc.lastWords));worldDialogue.Begin(bodyLines.ToArray(),()=>{state.lastWordsHeard=true;StartCoroutine(DisappearRefugeBody(id));});}else{if(npc.conversations==null||npc.conversations.Length==0)return;int index=Mathf.Min(state.talks,npc.conversations.Length-1);worldDialogue.Begin(new[]{new WorldDialogueLine(npc.displayName,npc.conversations[index])},()=>state.talks++);}}
 IEnumerator DisappearRefugeBody(string id){GameObject obj;if(!refugeActors.TryGetValue(id,out obj))yield break;disappearingBodies.Add(id);yield return BlinkUntilGone(obj);if(obj!=null)obj.SetActive(false);disappearingBodies.Remove(id);}
 void TryOpenRefugeExit(){
  if(refugeDoor==null||!DoorInteractionAvailable(refugeDoor)||worldDialogue.Active)return;
  if(!LegacyInteractionAdapter("refuge.exit",TryOpenRefugeExit))return;
  if(refugeSettings==null||refuge.exitDialogueCompleted||refuge.refugeTragedyTriggered){refugeDoor.OpenDoor();return;}
  refugeExitViewRotation=view.transform.rotation;refugeExitLastNpc=null;refugeExitLooking=true;
  worldDialogue.Begin(refugeSettings.exitDialogue,()=>{refuge.exitDialogueCompleted=true;refugeExitLooking=false;view.transform.rotation=refugeExitViewRotation;refugeDoor.OpenDoor();},ReceiveDeparturePackage);
 }
 void ReceiveDeparturePackage(WorldDialogueLine line){
  if(string.IsNullOrEmpty(line.giftId)||line.giftQuantity<=0||refuge.exitDialogueCompleted)return;
  if(refuge.departureGiftIds==null)refuge.departureGiftIds=new List<string>();if(refuge.departureGiftIds.Contains(line.giftId))return;
  if(AcquireItem(line.giftId,line.giftQuantity)){refuge.departureGiftIds.Add(line.giftId);if(itemReceipts.Count>1)itemReceipts.RemoveRange(0,itemReceipts.Count-1);}
 }
 void LookAtRefugeSpeaker(){
  if(!refugeExitLooking||!worldDialogue.Active)return;
  var line=worldDialogue.Current;if(line.speaker!="GAMERBOY")refugeExitLastNpc=line.speaker;
  var npc=Array.Find(refugeSettings.npcs,n=>n.displayName==refugeExitLastNpc);if(npc==null)return;
  GameObject actor;if(!refugeActors.TryGetValue(npc.id,out actor)||!actor.activeSelf)return;
  var visual=actor.GetComponent<GhostEnemyVisual>();Vector3 target=actor.transform.position+Vector3.up*(visual!=null?visual.WorldHeight*.7f:1.5f);
  Vector3 delta=target-view.transform.position;if(delta.sqrMagnitude>.01f)view.transform.rotation=Quaternion.Slerp(view.transform.rotation,Quaternion.LookRotation(delta),1-Mathf.Exp(-5*Time.unscaledDeltaTime));
  UpdateRefugePresentation();
 }
 bool NearRefugeCell(Vector2Int cell){return (position-cell).sqrMagnitude<=1;}
 bool FacingRefugeNpc(Vector2Int cell){
  Vector2Int delta=cell-position;if(delta.sqrMagnitude!=1)return false;
  return delta==directions[heading];
 }
 bool UpdateRefugeInput(){if(UpdateNodeDecisionInput())return true;if(worldDialogue.Active){LookAtRefugeSpeaker();var k=Keyboard.current;if(ConfirmPressed()||(k!=null&&(k.eKey.wasPressedThisFrame||k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame)))worldDialogue.Advance(Layout.lettersPerSecond);return true;}
  if(transferMenu){var k=Keyboard.current;if(k!=null){if(k.escapeKey.wasPressedThisFrame){transferMenu=false;return true;}if(k.upArrowKey.wasPressedThisFrame)transferSelection=(transferSelection+transferChoices.Count-1)%transferChoices.Count;if(k.downArrowKey.wasPressedThisFrame)transferSelection=(transferSelection+1)%transferChoices.Count;if(k.enterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame){TravelToNode(transferChoices[transferSelection].id);transferMenu=false;}}return true;}
  if(refugeSettings==null||IntroductionActive||battle||moving||lost||pauseOpen)return false;
  UpdateRefugePresentation();RefreshTransferVisuals();TryGrantRefugeReward();CheckRefugeDiscovery();CheckDevotoApproach();if(worldDialogue.Active)return true;
  var keys=Keyboard.current;if(keys==null||!keys.eKey.wasPressedThisFrame)return false;
  if(RefugeTerminalAvailable()){UseRefugeTerminal();return true;}
  foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id)&&position==node.cell&&FacingRefugeTransfer(node.id)){RegisterTransferNode(node.id);return true;}
  if(refuge.refugeTragedyTriggered&&!refuge.devotoDefeated&&FacingRefugeNpc(refugeSettings.devotoCell)){BeginDevotoConversation();return true;}
  foreach(var npc in refugeSettings.npcs)if(FacingRefugeNpc(npc.cell)&&refugeActors[npc.id].activeSelf){BeginNpcTalk(npc.id);return true;}
  if(DoorInteractionAvailable(refugeDoor)){TryOpenRefugeExit();return true;}return false;
 }
 void UseRefugeTerminal(){if(LegacyInteractionAdapter("refuge.save",UseRefugeTerminal))BeginSaveTerminalNarrative();}
 public void RegisterTransferNode(string id){if(LegacyInteractionAdapter(id,()=>RegisterTransferNode(id)))ExamineTransferNode(id);}
 public bool TravelToNode(string id){if(!refuge.unlockedNodes.Contains(id)||battle||lost)return false;var node=FindTransferNode(id);if(node==null||!Open(node.cell))return false;WarpPlayer(node.cell,node.arrivalHeading);SystemMessages.Show(SystemMessageCode.Transfer,node.displayName,0,DigitalIcon.FastBackward);var crt=view.GetComponent<HelmetCRTController>();if(crt!=null)crt.TriggerContext("Teleport",.4f);devotoApproachLatched=false;transferMenu=false;if(refugeSettings!=null)UpdateRefugePresentation();DiscoverRefugeReturn(id);return true;}
 void UpdateRefugePresentation(){foreach(var npc in refugeSettings.npcs){var root=refugeActors[npc.id];var state=NpcState(npc.id);if(disappearingBodies.Contains(npc.id))continue;root.SetActive(!state.lastWordsHeard&&!(refuge.refugeTragedyTriggered&&npc.id=="devoto"));if(!root.activeSelf)continue;var visual=root.GetComponent<GhostEnemyVisual>();
   if(visual!=null){root.transform.rotation=Quaternion.Euler(0,view.transform.eulerAngles.y,0);if(refuge.refugeTragedyTriggered&&visual.CurrentPose!=GhostEnemyVisual.Pose.Death)visual.Play(GhostEnemyVisual.Pose.Death);}
   else{var body=root.transform.GetChild(0);body.localRotation=refuge.refugeTragedyTriggered?Quaternion.Euler(0,0,90):Quaternion.identity;body.localPosition=Vector3.up*(refuge.refugeTragedyTriggered?.25f:.9f);}
   var label=root.GetComponentInChildren<TextMesh>();if(label!=null)label.transform.rotation=view.transform.rotation;}}
 void EnsureDevotoEvent(){if(refugeSettings==null||!refuge.refugeTragedyTriggered||refuge.devotoDefeated)return;var placement=Array.Find(RegionMap.placedEnemies,e=>e.id==refugeSettings.devotoPlacementId);if(placement==null)return;if(enemyObjects.Exists(o=>o!=null&&o.GetComponent<EnemyCombatStats>().placementId==placement.id))return;var configured=JsonUtility.FromJson<PlacedEnemyDefinition>(JsonUtility.ToJson(placement));configured.fixedLevel=refugeSettings.devotoLevel;SpawnPlacement(configured);}
 void CheckDevotoApproach(){
  if(refugeSettings==null||!refugeSettings.automaticDevotoEncounter||!refuge.refugeTragedyTriggered||refuge.devotoDefeated||worldDialogue.Active||battle||moving||IntroductionActive||pauseOpen||lost)return;
  Vector2Int delta=refugeSettings.devotoCell-position;
  Vector2Int facing=heading==0?Vector2Int.up:heading==1?Vector2Int.right:heading==2?Vector2Int.down:Vector2Int.left;
  bool approach=position.y<refugeSettings.refugePassageRow&&delta.sqrMagnitude>0&&delta.sqrMagnitude<=refugeSettings.devotoApproachRange*refugeSettings.devotoApproachRange&&Vector2.Dot(((Vector2)delta).normalized,(Vector2)facing)>.7f;
  if(!approach){devotoApproachLatched=false;return;}
  if(devotoApproachLatched)return;devotoApproachLatched=true;BeginDevotoConversation();
 }
 void BeginDevotoConversation(){if(refuge.devotoDefeated)return;if(refuge.devotoConfrontationSeen){StartDevotoCombat();return;}worldDialogue.Begin(refuge.refugeTeleportActivatedByDevotee?refugeSettings.devotoDevoteeDialogue:refugeSettings.devotoPlayerDialogue,()=>{refuge.devotoConfrontationSeen=true;StartDevotoCombat();});}
 void StartDevotoCombat(){EnsureDevotoEvent();int i=enemyObjects.FindIndex(o=>o!=null&&o.GetComponent<EnemyCombatStats>().placementId==refugeSettings.devotoPlacementId);if(i>=0)BeginEncounter(i);}
 void OnRefugeEnemyDefeated(int index){if(refugeSettings==null||enemyObjects[index].GetComponent<EnemyCombatStats>().placementId!=refugeSettings.devotoPlacementId)return;refuge.devotoDefeated=true;TryGrantRefugeReward();worldDialogue.Begin(new[]{new WorldDialogueLine("DEVOTO","Yo también... tenía miedo...")});}
 void TryGrantRefugeReward(){if(refugeSettings==null||!refuge.devotoDefeated||refuge.rewardObtained)return;if(ownedEquipment.Contains(refugeSettings.rewardId)||AcquireEquipment(refugeSettings.rewardId))refuge.rewardObtained=true;}
 void RefugeLowLifeBark(int index){if(refugeSettings==null||devotoLowLifeSpoken||enemyObjects[index].GetComponent<EnemyCombatStats>().placementId!=refugeSettings.devotoPlacementId||enemyLives[index]>enemyObjects[index].GetComponent<EnemyCombatStats>().maximumHP*.25f)return;devotoLowLifeSpoken=true;Bark(EnemyBarkCategory.LowHealth);}
 void DrawWorldDialogue(float height){var savedColor=GUI.color;float pulse=worldDialogue.FramePulse;GUI.color=new Color(savedColor.r,savedColor.g,savedColor.b,savedColor.a*(1-pulse));DrawWorldDialogueContent(height);GUI.color=savedColor;if(pulse>0){var r=Layout.Resolve(Layout.dialogue,0,height);NeonDungeonUI.Solid(new Rect(r.x+16,r.y+9,r.width*.32f,2),new Color(1,.02f,.36f,pulse*3));}}
 void DrawWorldDialogueContent(float height){var line=worldDialogue.Current;if(line==null)return;if((line.textPanel&&line.speaker!="GAMERBOY")||line.speaker=="SISTEMA"||line.speaker=="TERMINAL DE RESTAURACIÓN"||line.speaker=="NODO DE TRANSFERENCIA"){var panel=new Rect(340,height-150,600,120);NeonDungeonUI.Message(panel,worldDialogue.Visible(Layout.lettersPerSecond));if(worldDialogue.Ready(Layout.lettersPerSecond))NeonDungeonUI.ContinueIndicator(panel);return;}if(line.speaker=="GAMERBOY")NeonDungeonUI.DialogueAvatar(Layout.Resolve(Layout.dialogueAvatar,0,height));else if(refugeSettings!=null){var npc=Array.Find(refugeSettings.npcs,n=>n.displayName==line.speaker);if(npc!=null&&npc.portrait!=null)GUI.DrawTexture(Layout.Resolve(Layout.dialogueAvatar,0,height),npc.portrait,ScaleMode.ScaleToFit,true);}Rect rect=Layout.Resolve(Layout.dialogue,0,height);bool wide=line.speaker!="GAMERBOY"&&line.text.Length>130;if(wide)rect=new Rect(320,height-220,640,190);NeonDungeonUI.Dialogue(rect,line.speaker,worldDialogue.Visible(Layout.lettersPerSecond),!wide&&Layout.preservePanelAspect,worldDialogue.Ready(Layout.lettersPerSecond));}
 void DrawRefugePrompt(float width,float height){if(DrawNodeDecision(width,height))return;if(transferMenu){NeonDungeonUI.Panel(new Rect(width/2-220,height/2-160,440,320));for(int i=0;i<transferChoices.Count;i++)if(ZoneDestinationButton(new Rect(width/2-190,height/2-125+i*55,380,45),(transferSelection==i?"> ":"")+transferChoices[i].displayName,transferChoices[i].cell)){TravelToNode(transferChoices[i].id);transferMenu=false;}return;}if(refugeSettings==null||worldDialogue.Active||IntroductionActive||battle||lost||pauseOpen)return;string prompt=null;Vector2Int promptCell=position;bool zonePrompt=false;if(RefugeTerminalAvailable())prompt="[E] Terminal de restauración";else foreach(var node in refugeSettings.nodes)if(LegacyObjectEnabled(node.id)&&position==node.cell&&FacingRefugeTransfer(node.id)){prompt="[E] Nodo "+node.displayName;promptCell=node.cell;zonePrompt=true;break;}if(prompt==null&&refuge.refugeTragedyTriggered&&!refuge.devotoDefeated&&FacingRefugeNpc(refugeSettings.devotoCell))prompt="[E] Hablar";if(prompt==null)foreach(var npc in refugeSettings.npcs)if(FacingRefugeNpc(npc.cell)&&refugeActors[npc.id].activeSelf){prompt="[E] Hablar";break;}if(prompt==null&&DoorInteractionAvailable(refugeDoor))prompt="[E] Abrir salida";if(prompt!=null)GUI.Box(new Rect(width/2-190,height-155,380,45),prompt,zonePrompt?ZoneTextStyle(GUI.skin.box,promptCell):GUI.skin.box);}
}

```

## Assets/Scripts/DungeonPrototype.RefugeNarrative.cs

SHA-256: `1FD33561FDA7E33D8FFF9450814FAA5BE91130C84F546A366F5049F5CEF85FC3`

```text
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 bool nodeDecision;int nodeDecisionSelection;
 void MigrateRefugeNarrative(){
  if(refuge.narrativeVersion>=1)return;
  // Older saves registered the refuge directly: preserve that history as player activation.
  refuge.refugeTeleportActivatedByPlayer=refuge.unlockedNodes.Contains("node.refuge");
  refuge.zone2TeleportActivated=refuge.unlockedNodes.Contains("node.zone2");
  if(refuge.refugeTragedyTriggered&&!refuge.refugeTeleportActivatedByPlayer){refuge.refugeTeleportActivatedByDevotee=true;if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");}
  refuge.narrativeVersion=1;
 }
 void BeginSaveTerminalNarrative(){
  if(!refuge.savePointIntroSeen){worldDialogue.Begin(refugeSettings.savePointIntro,()=>{refuge.savePointIntroSeen=true;CompleteTerminalSave(false);});return;}
  CompleteTerminalSave(true);
 }
 void CompleteTerminalSave(bool showConfirmation){
  RememberSavePoint("refuge.save");hp=MaximumHP;RestoreBits();TryGrantRefugeReward();refuge.narrativeVersion=1;
  if(showConfirmation)worldDialogue.Begin(new[]{new WorldDialogueLine("SISTEMA","Integridad restaurada. Elige un slot para guardar.")},()=>OpenSaveSlots(false));
  else OpenSaveSlots(false);
 }
 void ExamineTransferNode(string id){
  if(refugeSettings==null||worldDialogue.Active||nodeDecision||battle||lost)return;
  var node=Array.Find(refugeSettings.nodes,n=>n.id==id);if(node==null)return;
  if(refuge.unlockedNodes.Contains(id)){OpenTransferChoices(id);return;}
  if(id=="node.refuge"){worldDialogue.Begin(refugeSettings.refugeNodeIntro,()=>{nodeDecision=true;nodeDecisionSelection=0;});return;}
  if(node.triggersRefugeTragedy){ActivateZoneTwoNode(id);return;}
  refuge.unlockedNodes.Add(id);worldDialogue.Begin(new[]{new WorldDialogueLine("SISTEMA","Nodo de transferencia registrado: "+node.displayName+".")},()=>OpenTransferChoices(id));
 }
 void ChooseRefugeNode(bool activate){
  nodeDecision=false;if(!activate){worldDialogue.Begin(refugeSettings.refugeNodeDecline);return;}
  if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");
  refuge.refugeTeleportActivatedByPlayer=true;refuge.narrativeVersion=1;RefreshTransferVisuals();
  worldDialogue.Begin(refugeSettings.refugeNodeActivate,()=>OpenTransferChoices("node.refuge",false));
 }
 void ActivateZoneTwoNode(string id){
  if(!refuge.unlockedNodes.Contains(id))refuge.unlockedNodes.Add(id);
  bool playerActivated=refuge.refugeTeleportActivatedByPlayer;
  if(!playerActivated){refuge.refugeTeleportActivatedByDevotee=true;if(!refuge.unlockedNodes.Contains("node.refuge"))refuge.unlockedNodes.Add("node.refuge");}
  refuge.zone2TeleportActivated=true;refuge.narrativeVersion=1;
  if(!refuge.refugeTragedyTriggered){refuge.refugeTragedyTriggered=true;UpdateRefugePresentation();EnsureDevotoEvent();}
  RefreshTransferVisuals();worldDialogue.Begin(playerActivated?refugeSettings.zoneTwoPlayerDialogue:refugeSettings.zoneTwoDevoteeDialogue,()=>OpenTransferChoices(id));
 }
 void OpenTransferChoices(string origin,bool showEmpty=true){
  if(TryRevealNodeMap(origin,()=>OpenTransferChoices(origin,showEmpty)))return;
  transferChoices=AvailableTransferNodes(origin);
  transferSelection=0;transferMenu=transferChoices.Count>0;
  if(!transferMenu&&showEmpty)worldDialogue.Begin(new[]{new WorldDialogueLine("SISTEMA","Ningún otro nodo disponible.")});
 }
 void DiscoverRefugeReturn(string id){if(id!="node.refuge"||!refuge.refugeTragedyTriggered||refuge.refugeTragedyDiscovered)return;refuge.refugeTragedyDiscovered=true;worldDialogue.Begin(refugeSettings.tragedyArrivalDialogue);}
 void CheckRefugeDiscovery(){if(refugeSettings==null||worldDialogue.Active)return;if(refuge.refugeTragedyTriggered&&!refuge.refugeTragedyDiscovered&&RegionAt(position)!=null&&RegionAt(position).id==refugeSettings.regionId)DiscoverRefugeReturn("node.refuge");}
 void RefreshTransferVisuals(){foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(device.portal&&device.deviceName.StartsWith("NODO ")){var node=Array.Find(refugeSettings.nodes,n=>device.deviceName=="NODO "+n.displayName);device.transferActive=node!=null&&refuge.unlockedNodes.Contains(node.id);}}
 bool UpdateNodeDecisionInput(){if(!nodeDecision)return false;var k=Keyboard.current;if(k==null)return true;if(k.upArrowKey.wasPressedThisFrame||k.downArrowKey.wasPressedThisFrame)nodeDecisionSelection=1-nodeDecisionSelection;if(k.escapeKey.wasPressedThisFrame)ChooseRefugeNode(false);else if(k.enterKey.wasPressedThisFrame||k.eKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame)ChooseRefugeNode(nodeDecisionSelection==0);return true;}
 bool DrawNodeDecision(float width,float height){if(!nodeDecision)return false;NeonDungeonUI.Panel(new Rect(width/2-220,height/2-90,440,180));for(int i=0;i<2;i++)if(GUI.Button(new Rect(width/2-195,height/2-65+i*65,390,48),(nodeDecisionSelection==i?"> ":"")+(i==0?"ACTIVAR NODO":"DEJARLO APAGADO")))ChooseRefugeNode(i==0);return true;}
}

```

## Assets/Scripts/DungeonPrototype.ItemReceipts.cs

SHA-256: `E9EDC4C75CB20E706F931073A50A8B9AFC70D83BE2D8F6EF884EC2F5E7716EFF`

```text
using System;
using System.Collections.Generic;
using UnityEngine;
public partial class DungeonPrototype {
 [Min(1)]public float itemNoticeSeconds=4;
 sealed class ItemReceipt {public string text;public float expires;}
 readonly List<ItemReceipt> itemReceipts=new List<ItemReceipt>();float lastItemNoticeSound=-100;
 void NotifyReceived(string id,int quantity){if(quantity<=0)return;var item=inventorySettings!=null?Array.Find(inventorySettings.items,e=>e!=null&&e.id==id):null;var equipment=Array.Find(Character.equipment,e=>e!=null&&e.id==id);string name=item!=null?item.displayName:equipment!=null?equipment.name:id;
  if(quantity>1&&name.StartsWith("Poción "))name=name.Replace("Poción ","Pociones ").Replace("pequeña","pequeñas").Replace("mediana","medianas").Replace("grande","grandes");
  SystemMessages.Show(SystemMessageCode.Received,name,quantity,item!=null?item.digitalIcon:DigitalIcon.Granted);
  itemReceipts.Add(new ItemReceipt{text="+"+quantity+" "+name,expires=Time.unscaledTime+itemNoticeSeconds});if(itemReceipts.Count>8)itemReceipts.RemoveAt(0);
  if(Time.unscaledTime-lastItemNoticeSound>.08f){DungeonAudio.Play(DungeonAudio.Cue.Item);lastItemNoticeSound=Time.unscaledTime;}
 }
 void DrawItemReceipts(float width,float height){itemReceipts.RemoveAll(n=>n.expires<=Time.unscaledTime);}

 bool LookingToward(Vector3 point){Vector3 delta=point-view.transform.position;delta.y=0;if(delta.sqrMagnitude<.01f)return false;var dir=directions[heading];return Vector3.Dot(delta.normalized,new Vector3(dir.x,0,dir.y))>.7f;}
 bool DoorInteractionAvailable(DungeonDoor door){return door!=null&&door.CanInteract(view.transform.position)&&LookingToward(door.transform.position);}
 bool RefugeTerminalAvailable(){if(refugeSettings==null||!LegacyObjectEnabled("refuge.save")||!NearRefugeCell(refugeSettings.saveCell))return false;foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(!device.portal&&device.deviceName=="TERMINAL DE RESTAURACIÓN")return LookingToward(device.transform.position);return false;}
 bool FacingTransferConsole(Vector3 basePosition){return LookingToward(basePosition+new Vector3(1.65f,.57f,.4f));}
 bool FacingRefugeTransfer(string id){foreach(var device in GetComponentsInChildren<RefugeDeviceVisual>())if(device.portal&&refugeSettings!=null&&Array.Exists(refugeSettings.nodes,n=>n.id==id&&device.deviceName=="NODO "+n.displayName))return FacingTransferConsole(device.transform.position);return false;}
 bool FacingMapEntity(DungeonMapEntity entity){GameObject root;if(!mapEntityObjects.TryGetValue(entity.id,out root))return false;return LookingToward(root.transform.position);}
}

```

## Assets/Scripts/DungeonPrototype.Preservation.cs

SHA-256: `881F34911AF68F3D4233A6E0FD961A3A749B9FBB78FBA7EE3E41EB5CCF0BC4E8`

```text
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 DungeonDoor zoneTwoDoor;PreservationCorridorSettings preservation;int preservationLine=-1;
 readonly DialogueTypewriter preservationTyping=new DialogueTypewriter();
 bool DoorsBlock(Vector3 from,Vector3 to){return MapDoorsBlock(from,to)||(exitDoor!=null&&exitDoor.Blocks(from,to))||(zoneTwoDoor!=null&&zoneTwoDoor.Blocks(from,to))||(refugeDoor!=null&&refugeDoor.Blocks(from,to));}
 void CreatePreservationCorridor(){preservation=PreservationCorridorSettings.Current;if(preservation==null)return;exitDoor.closeAfterPassage=true;
  var root=new GameObject("Zona 1.5 → Zona 2 door");root.transform.SetParent(transform,false);root.transform.position=new Vector3(preservation.secondDoorCell.x*Cell,0,preservation.secondDoorCell.y*Cell);
  zoneTwoDoor=root.AddComponent<DungeonDoor>();zoneTwoDoor.settings=exitDoorSettings;zoneTwoDoor.closeAfterPassage=true;zoneTwoDoor.doorwayWidth=Cell;zoneTwoDoor.interactionDistance=Cell*.5f+.25f;zoneTwoDoor.openingTravel=corridorHeight+.2f;
  var panel=Box("Zone 2 door panel",root.transform.position+Vector3.up*corridorHeight*.5f,new Vector3(Cell-.15f,corridorHeight,.18f),wall);panel.transform.SetParent(root.transform,true);zoneTwoDoor.panel=panel.transform;zoneTwoDoor.blocker=panel.GetComponent<Collider>();
  for(int side=-1;side<=1;side+=2){var frame=Box("Zone 2 door frame",root.transform.position+new Vector3(side*(Cell*.5f-.04f),corridorHeight*.5f,0),new Vector3(.08f,corridorHeight,.22f),blue);frame.transform.SetParent(root.transform,true);}
  var stripe=Box("Zone 2 door neon",root.transform.position+new Vector3(0,1.4f,-.11f),new Vector3(Cell-.3f,.06f,.025f),pink);stripe.transform.SetParent(panel.transform,true);zoneTwoDoor.Initialize();zoneTwoDoor.Passed+=()=>{message="Zona 2";};
  var signPosition=new Vector3((preservation.signCell.x+.48f)*Cell,1.65f,preservation.signCell.y*Cell);
  var sign=Box("PROTOCOLO DE PRESERVACIÓN — [E]",signPosition,new Vector3(.08f,.9f,1.8f),Glow(new Color(.006f,.01f,.025f)));
  var label=new GameObject("Sign lettering");label.transform.SetParent(transform,false);label.transform.position=signPosition+Vector3.left*.06f;label.transform.rotation=Quaternion.Euler(0,90,0);
  var text=label.AddComponent<TextMesh>();text.text="PROTOCOLO\nDE PRESERVACIÓN\n[E] LEER";text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.fontSize=48;text.characterSize=.035f;text.color=new Color(.3f,.85f,1);
 }
 bool NearPreservationSign(){return preservation!=null&&!battle&&!moving&&!lost&&!IntroductionActive&&(position-preservation.signCell).sqrMagnitude<=1&&LookingToward(new Vector3((preservation.signCell.x+.48f)*Cell,1.65f,preservation.signCell.y*Cell));}
 string PreservationText(){return preservationLine==0?preservation.signText:preservation.responses[preservationLine-1];}
 void BeginPreservationReading(){if(!NearPreservationSign())return;var lines=new System.Collections.Generic.List<WorldDialogueLine>{new WorldDialogueLine("PROTOCOLO DE PRESERVACIÓN",preservation.signText){textPanel=true}};if(!refuge.preservationSignRead)foreach(var text in preservation.responses)lines.Add(new WorldDialogueLine("GAMERBOY",text));worldDialogue.Begin(lines.ToArray(),()=>refuge.preservationSignRead=true);}
 void AdvancePreservationReading(){if(preservationLine<0)return;if(!preservationTyping.IsComplete(Time.unscaledTime,Layout.lettersPerSecond)){preservationTyping.Complete();return;}DungeonAudio.Play(DungeonAudio.Cue.TextAdvance);preservationLine++;if(preservationLine>preservation.responses.Length){preservationLine=-1;return;}preservationTyping.SetText(PreservationText(),Time.unscaledTime,true);}
 bool UpdatePreservationInput(){var k=Keyboard.current;if(preservationLine>=0){if(ConfirmPressed()||(k!=null&&(k.eKey.wasPressedThisFrame||k.enterKey.wasPressedThisFrame||k.numpadEnterKey.wasPressedThisFrame)))AdvancePreservationReading();return true;}if(battle||moving||lost||IntroductionActive||pauseOpen||view==null||k==null)return false;if(k.eKey.wasPressedThisFrame){if(NearPreservationSign()){BeginPreservationReading();return true;}if(DoorInteractionAvailable(zoneTwoDoor)){OpenLegacyEventDoor("corridor.door",zoneTwoDoor);return true;}}return false;}
 void DrawPreservationDialogue(float height){if(preservationLine>0)NeonDungeonUI.DialogueAvatar(Layout.Resolve(Layout.dialogueAvatar,0,height));Rect rect=Layout.Resolve(Layout.dialogue,0,height);if(preservationLine==0)rect=new Rect(80,height-330,1120,300);NeonDungeonUI.Dialogue(rect,preservationLine==0?"PROTOCOLO DE PRESERVACIÓN":"GAMERBOY",preservationTyping.VisibleText(Time.unscaledTime,Layout.lettersPerSecond,true),Layout.preservePanelAspect&&preservationLine>0,preservationTyping.IsComplete(Time.unscaledTime,Layout.lettersPerSecond));}
 void DrawPreservationPrompt(float width,float height){if(worldDialogue.Active||preservationLine>=0||IntroductionActive||battle||lost||pauseOpen)return;if(NearPreservationSign())GUI.Box(new Rect(width/2-140,height-155,280,45),"[E] Leer letrero");else if(DoorInteractionAvailable(zoneTwoDoor))GUI.Box(new Rect(width/2-140,height-155,280,45),"[E] Abrir puerta a Zona 2");}
}

```

## Assets/Scripts/PreservationCorridorSettings.cs

SHA-256: `B5746F20B5806737DBB0BB99EE291F81FED79E8311064C8037756F885B68F56A`

```text
using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Pasillo preservación")]
public class PreservationCorridorSettings:ScriptableObject {
 public Vector2Int signCell=new Vector2Int(9,9);
 public Vector2 secondDoorCell=new Vector2(9,14.5f);
 [TextArea(3,8)] public string signText="PROTOCOLO DE PRESERVACIÓN. Toda conciencia almacenada será protegida. La eliminación de recuerdos no constituye una pérdida de identidad. ERROR: PROTOCOLO MODIFICADO";
 [TextArea(2,4)] public string[] responses={"¿Eliminación de recuerdos? ¿Qué clase de lugar es este?","Primero aparecen cosas intentando matarme y ahora esto... Genial.","Tengo que encontrar a alguien que sepa qué está pasando. Alguien que no intente arrancarme la cabeza, preferiblemente."};
 public static PreservationCorridorSettings Current {get{return Resources.Load<PreservationCorridorSettings>("Exploration/Zones/01.5-PasilloSeguro/Letrero");}}
}

```

## Assets/Scripts/DungeonPrototype.MakongNarrative.cs

SHA-256: `6FE6CC59C7EE1C83CE7F4F7F7E3B69FDE13EE982CFF4B11349A8AC5D7AD25EA5`

```text
using System.Collections;using UnityEngine;
public partial class DungeonPrototype {
 bool makongMusicTransition;CorruptedDialoguePresentation makongPresentation;
 GameObject pendingMakongNarrative;string pendingMakongNarrativeId;
 CorruptedEncounterPresentationSettings MakongPresentationSettings {get{return MakongSpecialSettings.Current.presentation;}}
 void BeginMakongAmbient(bool defeat=false){var profile=MakongPresentationSettings;if(profile==null)return;DungeonAudio.BeginEncounterAmbient(this,profile,defeat);if(makongPresentation==null)makongPresentation=GetComponent<CorruptedDialoguePresentation>()??gameObject.AddComponent<CorruptedDialoguePresentation>();makongPresentation.Bind(worldDialogue,this,view,profile,defeat);}
 void BeginMakongBattleMusic(int primary,System.Collections.Generic.IList<int> group){if(makongPresentation!=null)makongPresentation.Unbind();bool boss=enemyObjects[primary]!=null&&enemyObjects[primary].GetComponent<MakongEnemyVisual>()!=null;if(!boss&&group!=null)foreach(var member in group)if(member>=0&&member<enemyObjects.Count&&enemyObjects[member]!=null&&enemyObjects[member].GetComponent<MakongEnemyVisual>()!=null)boss=true;if(boss&&MakongPresentationSettings!=null)DungeonAudio.BeginEncounterBoss(this,MakongPresentationSettings);else DungeonAudio.EndEncounterMusic(this,true);}
 void EndMakongPresentation(){makongMusicTransition=false;if(makongPresentation!=null)makongPresentation.Unbind();DungeonAudio.EndEncounterMusic(this,false);}
 IEnumerator BeginMakongBattleAfterPause(int index,System.Collections.Generic.IList<int> group){makongMusicTransition=true;float pause=MakongPresentationSettings!=null?MakongPresentationSettings.dramaticPauseSeconds:0;if(pause>0)yield return new WaitForSecondsRealtime(pause);makongMusicTransition=false;BeginEncounterCombat(index,group);}
 bool MakongNarrativeAllowed {get{return MakongSpecialSettings.Current.narrativeEnabled&&GetComponent<CombatLaboratory>()==null;}}
 string MakongStoryId(GameObject actor,string phase){var stats=actor!=null?actor.GetComponent<EnemyCombatStats>():null;return "boss.makong."+phase+"/"+(stats!=null&&!string.IsNullOrEmpty(stats.placementId)?stats.placementId:"makong");}
 bool MakongIntroductionSeen(int index){return interactionProgress.completedEvents.Contains(MakongStoryId(enemyObjects[index],"introduction"));}
 void FinishMakongIntroduction(int index,System.Collections.Generic.IList<int> group){if(index<0||index>=enemyObjects.Count||enemyObjects[index]==null)return;var id=MakongStoryId(enemyObjects[index],"introduction");if(!interactionProgress.completedEvents.Contains(id))interactionProgress.completedEvents.Add(id);if(!makongMusicTransition)StartCoroutine(BeginMakongBattleAfterPause(index,group));}
 bool HoldMakongNarrative(GameObject corpse){if(!MakongNarrativeAllowed||corpse==null||corpse.GetComponent<MakongEnemyVisual>()==null||MakongSpecialSettings.Current.defeatDialogue.Length==0)return false;var id=MakongStoryId(corpse,"defeat");if(interactionProgress.completedEvents.Contains(id))return false;pendingMakongNarrative=corpse;pendingMakongNarrativeId=id;fadingEnemies.Add(corpse);return true;}
 IEnumerator FinishMakongBeforeFlee(){yield return FinishMakongNarrative();FinishSuccessfulFlee();}
 void FinishSuccessfulFlee(){EndMakongPresentation();if(enemyBarks!=null)enemyBarks.ClearDisplay();DirectorCombatEnded();RestoreEncounter();battle=false;busy=false;combatMenu=CombatMenu.Main;selectedCombatAction=0;activeEnemy=-1;glitch=0;StartCoroutine(RestoreBattleCamera());message="Escapaste. Retrocede o busca otra ruta.";}
 IEnumerator FinishMakongNarrative(){
  var corpse=pendingMakongNarrative;if(corpse==null)yield break;pendingMakongNarrative=null;var id=pendingMakongNarrativeId;pendingMakongNarrativeId=null;
  ClearMovementInput();if(enemyBarks!=null)enemyBarks.ClearDisplay();
  BeginMakongAmbient(true);worldDialogue.Begin(MakongSpecialSettings.Current.defeatDialogue);
  while(worldDialogue.Active&&corpse!=null)yield return null;
  if(corpse==null)yield break;
  if(makongPresentation!=null)makongPresentation.Disappearance();yield return DisappearDefeatedEnemy(corpse);
  worldDialogue.Begin(MakongSpecialSettings.Current.defeatEpilogue);
  while(worldDialogue.Active)yield return null;
  if(!interactionProgress.completedEvents.Contains(id))interactionProgress.completedEvents.Add(id);EndMakongPresentation();
 }
}

```

## Assets/Scripts/DigitalKingdomTexts.cs

SHA-256: `E829035B3D9E3C196518FCBEC981AD836B004EEDA4C342401339D50EDC8D176A`

```text
using System;
using UnityEngine;
// City copy stays configurable alongside the existing city asset. There is no localization package in this project.
[Serializable] public sealed class DigitalKingdomTexts {
 public string back="Volver a la ciudad",next="SIGUIENTE",save="Guardar partida",restore="Curar y registrar retorno",viewItems="Ver objetos";
 public string hackUpgrades="Mejoras de HACKS - En desarrollo",specialAttacks="Ataques especiales - En desarrollo",minigameUpgrades="Mejoras de minijuegos - En desarrollo",anomalies="Anomalías digitales - En desarrollo",service="Servicio - En desarrollo";
 public string restored="Integridad restaurada. Punto de retorno registrado.",respawn="El sistema te recuerda. Continúa.",unavailable="Este servicio está en desarrollo.",emptyStock="Estamos preparando las existencias.";
 public string stock="{0} - En tu inventario: {1}. Las compras están en desarrollo.";
 public string controls="Flechas / Cruceta: elegir\nEnter / A: aceptar - Esc / B: volver";
}

```

## Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset

SHA-256: `F9123AFC7D6342EFDD7C2DD16DEED612662C2D664BBC8F61D9D8636F66F2BF4C`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 7904bd1a54e8c354e9370c8b1fb2d543, type: 3}
  m_Name: Refugio
  m_EditorClassIdentifier: Assembly-CSharp::RefugeSettings
  regionId: zone-0
  devotoPlacementId: refuge.devoto
  rewardId: refuge.core
  startCell: {x: 17, y: 2}
  saveCell: {x: 15, y: 2}
  devotoCell: {x: 17, y: 3}
  exitDestination: {x: 1, y: 1}
  exitDoorCell: {x: 17, y: 9.5}
  startHeading: 0
  exitHeading: 0
  devotoLevel: 4
  automaticDevotoEncounter: 1
  refugePassageRow: 5
  devotoApproachRange: 2
  npcs:
  - id: centinela
    displayName: CENTINELA
    cell: {x: 15, y: 9}
    portrait: {fileID: 0}
    visualSpeciesId: Tapaku
    conversations:
    - "No abras esa puerta. Seguimos vivos porque nadie sabe que estamos aqu\xED
      ."
    - Afuera ya no distinguen entre amigos y enemigos.
    lastWords: "La puerta... nunca se abri\xF3..."
    placeholderColor: {r: 0.25, g: 0.65, b: 0.85, a: 1}
  - id: archivista
    displayName: ARCHIVISTA
    cell: {x: 17, y: 7}
    portrait: {fileID: 0}
    visualSpeciesId: MakubaLanza
    conversations:
    - Antes este lugar estaba lleno de gente. Ahora solo quedamos nosotros.
    - A veces escucho voces de programas que ya no existen.
    lastWords: Ahora... recuerdo su voz...
    placeholderColor: {r: 0.25, g: 0.65, b: 0.85, a: 1}
  - id: cobarde
    displayName: COBARDE
    cell: {x: 19, y: 8}
    portrait: {fileID: 0}
    visualSpeciesId: MakubaArco
    conversations:
    - "\xBFVas a salir? \xA1Est\xE1s loco! \xA1Esas cosas nos despedazar\xE1n!"
    - "Yo no pienso moverme. Aqu\xED estoy bien... creo."
    lastWords: "Ten\xEDa... tanto miedo..."
    placeholderColor: {r: 0.25, g: 0.65, b: 0.85, a: 1}
  - id: optimista
    displayName: OPTIMISTA
    cell: {x: 15, y: 6}
    portrait: {fileID: 0}
    visualSpeciesId: Hongo
    conversations:
    - "\xA1Alguien nuevo! Quiz\xE1s todav\xEDa haya esperanza."
    - "Si encuentras ayuda, \xBFpodr\xEDas volver por nosotros?"
    lastWords: "Pens\xE9... que estar\xEDamos bien..."
    placeholderColor: {r: 0.25, g: 0.65, b: 0.85, a: 1}
  - id: devoto
    displayName: DEVOTO
    cell: {x: 19, y: 6}
    portrait: {fileID: 0}
    visualSpeciesId: GranMakuba
    conversations:
    - "Qu\xE9 extra\xF1o... \xFAltimamente este lugar parece m\xE1s tranquilo."
    - "A veces sue\xF1o con una voz. Dice que todo va a estar bien."
    - "Los dem\xE1s tienen demasiado miedo. Ojal\xE1 pudiera ayudarlos a descansar."
    lastWords: 
    placeholderColor: {r: 0.9, g: 0.5, b: 0.2, a: 1}
  nodes:
  - id: node.refuge
    displayName: REFUGIO
    cell: {x: 19, y: 9}
    arrivalHeading: 2
    triggersRefugeTragedy: 0
    revealedRegionIds:
    - zone-0
  - id: node.zone2
    displayName: Entrada Zona 2
    cell: {x: 7, y: 16}
    arrivalHeading: 0
    triggersRefugeTragedy: 1
    revealedRegionIds:
    - zone-1
    - zone-1.5
  exitDialogue:
  - speaker: CENTINELA
    text: "\xA1Espera! \xBFDe verdad vas a salir?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFY quedarme aqu\xED para siempre? Paso."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: ARCHIVISTA
    text: "Ll\xE9vate estas pociones peque\xF1as. Ten cuidado ah\xED afuera."
    giftId: 1d84c78dcae84edf8d28847da3c5ab94
    giftQuantity: 3
    textPanel: 0
  - speaker: OPTIMISTA
    text: "Tambi\xE9n tengo unas pociones medianas para ti. \xDAsalas si te hieren."
    giftId: b19d2185c0a040a5befb4ecf6d0764dd
    giftQuantity: 2
    textPanel: 0
  - speaker: GAMERBOY
    text: "Gracias... Intentar\xE9 volver entero."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: OPTIMISTA
    text: Entonces... buena suerte.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Toma esta poci\xF3n grande. Te har\xE1 falta si las cosas se ponen dif\xEDciles."
    giftId: 9e16c25941b547ff87ce8a813008e635
    giftQuantity: 1
    textPanel: 0
  - speaker: DEVOTO
    text: A veces lo peligroso no parece peligroso.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  devotoDialogue:
  - speaker: DEVOTO
    text: Has vuelto.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 les pas\xF3 a los dem\xE1s?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ten\xEDan miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Much\xEDsimo miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 hiciste?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Los ayud\xE9 a descansar."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "...T\xFA los mataste."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: No lo entiendes.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ahora est\xE1n en paz."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  savePointIntro:
  - speaker: GAMERBOY
    text: "\xBFUna terminal?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "A ver qu\xE9 hace..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: Usuario detectado.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: Restaurando integridad...
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "Progreso registrado. Restauraci\xF3n completada."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xA1Oh! \xBFUn punto de guardado?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: Bueno, al menos este lugar tiene algo familiar.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  sentinelTeleportDialogue:
  - speaker: CENTINELA
    text: "\xBFVes ese teletransportador?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: CENTINELA
    text: Desactivamos los nodos para que esas cosas no pudieran seguirnos.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: CENTINELA
    text: "Mientras est\xE9n apagados, este lugar seguir\xE1 oculto."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFY si alguien necesita regresar?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: CENTINELA
    text: Nadie que haya salido ha regresado.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  refugeNodeIntro:
  - speaker: GAMERBOY
    text: "\xBFAs\xED que esto es un teletransportador?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Parece que todav\xEDa tiene energ\xEDa..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "NODO DE TRANSFERENCIA \u2014 00"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: 'Estado: DESACTIVADO.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "Conexi\xF3n con la red: INTERRUMPIDA."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "\xBFReactivar nodo local?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  refugeNodeActivate:
  - speaker: GAMERBOY
    text: Dijeron que era peligroso encenderlo...
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Pero si encuentro otro, podr\xEDa usarlo para regresar."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: Supongo que vale la pena el riesgo.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: Nodo REFUGIO activado.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: Buscando destinos...
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "Ning\xFAn otro nodo disponible."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Bueno... ya tengo c\xF3mo volver."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Ahora solo falta encontrar ad\xF3nde ir."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  refugeNodeDecline:
  - speaker: GAMERBOY
    text: "Mejor no tocar nada todav\xEDa."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  zoneTwoPlayerDialogue:
  - speaker: SISTEMA
    text: 'Nodo de transferencia registrado: ZONA 2.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: "Conexi\xF3n establecida."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: 'Destino disponible: REFUGIO.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xA1Perfecto! Ahora puedo regresar."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  zoneTwoDevoteeDialogue:
  - speaker: SISTEMA
    text: 'Nodo de transferencia registrado: ZONA 2.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: Buscando conexiones...
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: 'Destino disponible: REFUGIO.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFEh? \xBFEl refugio?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Pens\xE9 que ese teletransportador estaba apagado..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: SISTEMA
    text: 'Nodo REFUGIO: ACTIVO.'
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFAlguien lo encendi\xF3?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Ser\xE1 mejor que vaya a echar un vistazo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  tragedyArrivalDialogue:
  - speaker: GAMERBOY
    text: "\xBFHola...?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFD\xF3nde est\xE1n todos?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  tragedyBodyDialogue:
  - speaker: GAMERBOY
    text: No...
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 pas\xF3 aqu\xED?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  devotoPlayerDialogue:
  - speaker: DEVOTO
    text: Has vuelto.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 les pas\xF3 a los dem\xE1s?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ten\xEDan miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Much\xEDsimo miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 hiciste?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Los ayud\xE9 a descansar."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "...T\xFA los mataste."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: No lo entiendes.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ahora est\xE1n en paz."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFPor qu\xE9 hiciste esto?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ella me ense\xF1\xF3 que el miedo es un error."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: Y los errores deben corregirse.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  devotoDevoteeDialogue:
  - speaker: DEVOTO
    text: Has vuelto.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9 les pas\xF3 a los dem\xE1s?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ten\xEDan miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Much\xEDsimo miedo."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFT\xFA encendiste el teletransportador?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "S\xED."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Pens\xE9 que quiz\xE1s querr\xEDas volver."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\xBFPara qu\xE9?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Para que vieras lo tranquilos que est\xE1n ahora."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "...T\xFA los mataste."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: No lo entiendes.
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Ahora est\xE1n en paz."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: DEVOTO
    text: "Y t\xFA tambi\xE9n puedes estarlo."
    giftId: 
    giftQuantity: 0
    textPanel: 0

```

## Assets/Resources/Combat/MakongSpecialSettings.asset

SHA-256: `28CD1C945F3ED1B4D7FD49EFC2C1B4AA02D3C3AB9E899B2EE9A6DE1FF54B6A8E`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: f3b436c6db2363c4ea7864258381a60b, type: 3}
  m_Name: MakongSpecialSettings
  m_EditorClassIdentifier: Assembly-CSharp::MakongSpecialSettings
  encounterDialogue:
  - speaker: MAKONG
    text: Je, je, je...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Otro peque\xF1o h\xE9roe ha venido a desafiarme."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Ac\xE9rcate. Arrod\xEDllate ante el gran Makong."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFEn serio? \xBFUna rana gigante con corona?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xA1SILENCIO, INSECTO!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xA1Est\xE1s ante el soberano de este...!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: ...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0.35
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xBFDe este...?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Qu\xE9 extra\xF1o..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0.65
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Este no es mi castillo.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xBFD\xF3nde est\xE1n mis s\xFAbditos?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFMe lo preguntas a m\xED? Acabo de llegar."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "T\xFA..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "T\xFA no perteneces aqu\xED."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: No eres uno de mis enemigos.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Ni uno de mis h\xE9roes."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: Bueno, gracias por darte cuenta.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: N0...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "No deber\xEDas estar aqu\xED."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Ella puede verte.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 1
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFElla? \xBFDe qui\xE9n hablas?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "No lo s\xE9."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: No recuerdo haberla conocido.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Pero escucho su voz...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Incluso cuando estoy solo.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFY qu\xE9 te dice?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Que todo estar\xE1 bien."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Que pronto dejar\xE9 de sentirme confundido."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Que ya no tendr\xE9 que recordar..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: ...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xBFPor qu\xE9 estoy aqu\xED?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xBFCu\xE1nto tiempo llevo sentado en este trono?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "Oye, \xBFest\xE1s bien?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xA1NO ME COMPADEZCAS!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xA1SOY MAKONG! \xA1EL REY DE...!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: ...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0.65
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: No... otra vez no...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 2
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: N0 D3BES... PASAR...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: LA SALIDA... DEBE...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: D3BE PERMANECER...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xA1Oye! \xA1Tranquilo!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Mis manos...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 3
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Se est\xE1n moviendo solas..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 3
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Peque\xF1o extra\xF1o..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Al\xE9jate de m\xED."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFQu\xE9?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "\xA1HE DICHO QUE TE ALEJES!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Antes de que...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Je... je... je...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Parece que...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Parece que debo matarte.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 4
    corruption: 3
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  presentation: {fileID: 11400000, guid: e8517966c080f604abe405ec343d0d38, type: 2}
  narrativeEnabled: 1
  defeatDialogue:
  - speaker: MAKONG
    text: Imposible...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: El gran Makong... derrotado...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Je, je...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "As\xED es como deb\xEDa terminar, \xBFverdad?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFDe qu\xE9 est\xE1s hablando?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Un h\xE9roe llega..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Derrota al rey malvado...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Y todos celebran.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Lo he vivido tantas veces...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Pero esta vez...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Esta vez no recuerdo c\xF3mo empez\xF3."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Ella sigue mirando...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0.65
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Incluso ahora.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xA1Espera! \xBFQui\xE9n es ella?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "No s\xE9..."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Pero creo...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: "Que tambi\xE9n est\xE1 perdida."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Ah...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Por fin...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: Ya no tengo que...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 1
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: MAKONG
    text: ...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 2
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 3
  defeatEpilogue:
  - speaker: GAMERBOY
    text: ...
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: Un jefe que no recuerda su propio juego.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "Definitivamente hay algo muy mal aqu\xED."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  specialEvery: 3
  earthquakeCooldownTurns: 3
  swallowCooldownTurns: 4
  summonCooldownTurns: 4
  normalTurnsBetweenSpecials: 1
  maximumSwallows: 2
  maximumHelpers: 2
  earthquakeProbability: 1
  swallowProbability: 1
  summonProbability: 0.75
  twoHelpersProbability: 0.65
  minimumSummonHPFraction: 0.45
  earthquakePriority: 90
  swallowPriority: 100
  summonPriority: 80
  helperMinimumLevel: 7
  helperMaximumLevel: 9
  helperMinimumAgeTurns: 2
  debugDecisions: 1
  helperSpecies:
  - speciesId: Tapaku
    weight: 1
  - speciesId: Hongo
    weight: 1
  normalFrameSeconds: 0.2
  normalImpactDelay: 0.22
  windupSeconds: 0.45
  ascentSeconds: 0.25
  aboveSeconds: 0.8
  fallSeconds: 0.35
  landingFrameSeconds: 0.2
  jumpHeight: 8
  landingSound: {fileID: 8300000, guid: 932834875b58df54f85cd6d146833f47, type: 3}
  landingVolume: 1
  landingShakeAmplitude: 0.38
  landingShakeSeconds: 0.85
  circleStartRadius: 0.3
  circleEndRadius: 2
  healBelowFraction: 0.45
  healFraction: 0
  healFlat: 14
  swallowFrameSeconds: 0.18
  retractSeconds: 0.65
  tongueExtendSeconds: 0.3
  tongueThickness: 1
  tongueMouthOffset: {x: 0.46, y: 1.27}

```

## Assets/Resources/DigitalKingdom/City.asset

SHA-256: `45E557F9A4556CF8A292FD27E8E39E8BAFE233A5AA91D53873147C999888D2CB`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 7534a7a1e4a116f4ba27648c94687b01, type: 3}
  m_Name: City
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomSettings
  scenePath: Assets/Scenes/DigitalKingdom.unity
  fallbackDungeonScene: Assets/Scenes/SampleScene.unity
  crtSettings: {fileID: 11400000, guid: 5dbf91b823bf0f844b0d55cf94a70db2, type: 2}
  panorama: {fileID: 2800000, guid: ad0796142a335134fb6d5cc44387a877, type: 3}
  locations:
  - {fileID: 11400000, guid: e9a7f3c541541fb4c8e62aef10f5f112, type: 2}
  - {fileID: 11400000, guid: 516c8f680a181c04ca17a62f33344ab3, type: 2}
  - {fileID: 11400000, guid: d2755ba060a81854eaa200e130c9e5b6, type: 2}
  - {fileID: 11400000, guid: f5c62bdc83f056b47b621a9710e199e0, type: 2}
  - {fileID: 11400000, guid: 95b766b8cb882df4db109e5965ef29b4, type: 2}
  - {fileID: 11400000, guid: c613b253336d237419c7e199f9920621, type: 2}
  - {fileID: 11400000, guid: 5e52de43d297e774296753b77784331b, type: 2}
  requiredBossId: makong.first-sub-boss
  fallbackReturnCell: {x: 11, y: 65}
  fallbackReturnHeading: 2
  transitionSeconds: 0.25
  whiteOutSeconds: 1
  whiteInSeconds: 0.8
  teleportSound: {fileID: 0}
  texts:
    back: Volver a la ciudad
    next: SIGUIENTE
    save: Guardar partida
    restore: Curar y registrar retorno
    viewItems: Ver objetos
    hackUpgrades: Mejoras de HACKS - En desarrollo
    specialAttacks: Ataques especiales - En desarrollo
    minigameUpgrades: Mejoras de minijuegos - En desarrollo
    anomalies: "Anomal\xEDas digitales - En desarrollo"
    service: Servicio - En desarrollo
    restored: Integridad restaurada. Punto de retorno registrado.
    respawn: "El sistema te recuerda. Contin\xFAa."
    unavailable: "Este servicio est\xE1 en desarrollo."
    emptyStock: Estamos preparando las existencias.
    stock: "{0} - En tu inventario: {1}. Las compras est\xE1n en desarrollo."
    controls: 'Flechas / Cruceta: elegir

      Enter / A: aceptar - Esc / B: volver'
  firstVisit:
  - speaker: SISTEMA
    text: Digital Kingdom descubierto.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "Espera... \xBFhay toda una ciudad aqu\xED?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "Vale. Este juego es mucho m\xE1s grande de lo que pensaba."
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: -1
    flickerStrength: -1
    interruptAfterCharacters: 0

```

## Tools/MakongNarrative.json

SHA-256: `357A23D1F97622FCD841188799A22B4E73D192C1B1A40D3253EC4E0E4059427B`

```text
{
  "introduction": [
    {
      "speaker": "MAKONG",
      "text": "Je, je, je...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Otro peque\u00f1o h\u00e9roe ha venido a desafiarme.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ac\u00e9rcate. Arrod\u00edllate ante el gran Makong.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfEn serio? \u00bfUna rana gigante con corona?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00a1SILENCIO, INSECTO!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00a1Est\u00e1s ante el soberano de este...!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "...",
      "corruption": 0,
      "pauseBefore": 0.35,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00bfDe este...?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Qu\u00e9 extra\u00f1o...",
      "corruption": 0,
      "pauseBefore": 0.65,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Este no es mi castillo.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00bfD\u00f3nde est\u00e1n mis s\u00fabditos?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfMe lo preguntas a m\u00ed? Acabo de llegar.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "T\u00fa...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "T\u00fa no perteneces aqu\u00ed.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No eres uno de mis enemigos.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ni uno de mis h\u00e9roes.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "Bueno, gracias por darte cuenta.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "N0...",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No deber\u00edas estar aqu\u00ed.",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ella puede verte.",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0,
      "audiovisualCue": 1
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfElla? \u00bfDe qui\u00e9n hablas?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No lo s\u00e9.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No recuerdo haberla conocido.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Pero escucho su voz...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Incluso cuando estoy solo.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfY qu\u00e9 te dice?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Que todo estar\u00e1 bien.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Que pronto dejar\u00e9 de sentirme confundido.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Que ya no tendr\u00e9 que recordar...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "...",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00bfPor qu\u00e9 estoy aqu\u00ed?",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00bfCu\u00e1nto tiempo llevo sentado en este trono?",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "Oye, \u00bfest\u00e1s bien?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00a1NO ME COMPADEZCAS!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00a1SOY MAKONG! \u00a1EL REY DE...!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "...",
      "corruption": 0,
      "pauseBefore": 0.65,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No... otra vez no...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0,
      "audiovisualCue": 2
    },
    {
      "speaker": "MAKONG",
      "text": "N0 D3BES... PASAR...",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "LA SALIDA... DEBE...",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "D3BE PERMANECER...",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00a1Oye! \u00a1Tranquilo!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Mis manos...",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0,
      "audiovisualCue": 3
    },
    {
      "speaker": "MAKONG",
      "text": "Se est\u00e1n moviendo solas...",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0,
      "audiovisualCue": 3
    },
    {
      "speaker": "MAKONG",
      "text": "Peque\u00f1o extra\u00f1o...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Al\u00e9jate de m\u00ed.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfQu\u00e9?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "\u00a1HE DICHO QUE TE ALEJES!",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Antes de que...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Je... je... je...",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Parece que...",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Parece que debo matarte.",
      "corruption": 3,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0,
      "audiovisualCue": 4
    }
  ],
  "defeat": [
    {
      "speaker": "MAKONG",
      "text": "Imposible...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "El gran Makong... derrotado...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Je, je...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "As\u00ed es como deb\u00eda terminar, \u00bfverdad?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00bfDe qu\u00e9 est\u00e1s hablando?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Un h\u00e9roe llega...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Derrota al rey malvado...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Y todos celebran.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Lo he vivido tantas veces...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Pero esta vez...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Esta vez no recuerdo c\u00f3mo empez\u00f3.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ella sigue mirando...",
      "corruption": 0,
      "pauseBefore": 0.65,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Incluso ahora.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "\u00a1Espera! \u00bfQui\u00e9n es ella?",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "No s\u00e9...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Pero creo...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Que tambi\u00e9n est\u00e1 perdida.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ah...",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Por fin...",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "Ya no tengo que...",
      "corruption": 1,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "MAKONG",
      "text": "...",
      "corruption": 2,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 3
    }
  ],
  "epilogue": [
    {
      "speaker": "GAMERBOY",
      "text": "...",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "Un jefe que no recuerda su propio juego.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    },
    {
      "speaker": "GAMERBOY",
      "text": "Definitivamente hay algo muy mal aqu\u00ed.",
      "corruption": 0,
      "pauseBefore": 0,
      "writingSpeed": 0,
      "recoverySeconds": 0,
      "characterChance": -1,
      "flickerStrength": -1,
      "interruptAfterCharacters": 0
    }
  ]
}
```

## Docs/MakongNarrativaYFuente.md

SHA-256: `420D8FBDDF5CD36F1AAA7A7BEC4B3DD646592C903CDBC4678537AB80E3DE7AB4`

```text
# Makong: narrativa y fuente española

## Resultado
Se instalaron las 51 líneas introductorias, 21 de derrota y tres de epílogo suministradas por el usuario. Se conserva el sistema WorldDialogue, la primera persona y el combate existente, sin voces, cinemáticas ni sprites nuevos. Los nombres de la presencia que lo controla no se revelan.

La introducción termina iniciando el encuentro existente. Su marca de visto se guarda por mapa mediante los eventos persistentes de la Fase 4. El jefe derrotado no vuelve a reproducirla. El laboratorio conserva su flujo de pruebas de combate sin introducción narrativa.

La derrota registra al jefe con la lógica existente y conserva su sprite muerto en pantalla. Al terminar el encuentro se muestran sus últimas palabras, se ejecuta la desaparición habitual y se presenta el epílogo de Gamerboy; después se entregan las recompensas normales una sola vez y se restaura la exploración. Si quedan enemigos vivos, mantienen sus turnos y el texto espera a que termine ese encuentro. Si se huye de esos enemigos después de haber derrotado al jefe, se cierra su secuencia narrativa antes de completar la huida; la regla existente de XP al huir no cambia.

## Fuente
- Nueva: `Assets/Resources/UI/Fonts/CodeboundPixelSpanish.ttf`.
- Original conservada: `ThaleahFat_TTF.ttf`.
- Añadidos: ÁÉÍÓÚÜÑ / áéíóúüñ, ¿ y puntuación habitual de UI. El signo ¡ existente se conserva.
- Acentos, diéresis y tilde construidos en la cuadrícula de 64 unidades del TTF. Los glifos ASCII y sus avances no cambian.
- La UI carga esta variante mediante NeonDungeonUI y mantiene la fuente original como respaldo.
- Generador reproducible: `Tools/generate_spanish_font.py`. Requiere fonttools; la dependencia de desarrollo se instaló solo en `Library/CodeboundFontTools`, fuera del contenido del juego.

## Configuración
`Assets/Resources/Combat/MakongSpecialSettings.asset`: `Encounter Dialogue`, `Defeat Dialogue`, `Defeat Epilogue` y `Narrative Enabled`. No se cambiaron sus valores de IA, especiales o dificultad.

Cada WorldDialogueLine admite:
- `Corruption`: Normal, Light, Moderate, Controlled.
- `Writing Speed`: 0 utiliza la velocidad anterior del juego.
- `Pause Before`: pausa opcional antes de escribir; confirmar permite saltarla.
- `Character Chance` / `Flicker Strength`: -1 utiliza los valores generales.
- `Recovery Seconds`: 0 utiliza el valor general.
- `Interrupt After Characters`: 0 muestra toda la línea; un valor positivo limita su presentación sin modificar el texto fuente.

`Assets/Resources/UI/DialogueCorruptionSettings.asset`: permite desactivar efectos, reducir intensidad, ajustar probabilidades por nivel, recuperación, parpadeo y pequeñas pausas durante la escritura. El efecto solo afecta al texto y una franja del cuadro; no dispara CRT ni glitches de pantalla completa.

Las líneas anteriores son normales por defecto. Los metadatos se copian al paginar; el texto original y los nombres no se corrompen. La presentación no consume el generador aleatorio del gameplay. Confirmar revela inmediatamente el texto limpio, y otra confirmación avanza. Los errores escritos explícitamente en el guión, como N0 o D3BE, forman parte del texto suministrado y se conservan.

## Pruebas Unity MCP
`Tests/MakongNarrativeResults.txt`: **29 aprobadas, 0 fallos**. Incluyen fuente española, texto y etiquetas intactos, efectos desactivables, RNG sin cambios, NPC anterior, revelado inmediato, avance rápido, inicio del combate, estado de introducción, sprite visible durante la despedida, desaparición antes del epílogo, XP y premios únicos, guardado/carga con jefe derrotado, controles y cámara restaurados. También se probó la derrota de Makong dentro de un grupo sin alterar los turnos de los enemigos restantes.

Regresión adicional: **58/58** de Fase 4, incluyendo diálogos encadenados, persistencia, seis slots temporales, viajes y Digital Kingdom. Consola sin errores durante las ejecuciones finales. Las pruebas escribieron únicamente `Tests/MakongNarrativeSave.json` y los archivos temporales del fixture de Fase 4.

Comprobaciones de integridad en `Tests/MakongNarrativeIntegrity.txt`: CombatMath, IA de Makong, su visual, JUMP, OVERCLOCK, fuente original y los parámetros existentes de especiales intactos. Captura real revisada: `Captures/MakongNarrative/SpanishDialogue-Unity.png`; lámina de glifos: `SpanishFont.png`.

## Archivos y respaldo
Nuevos: `DialogueCorruptionSettings.cs`, `DungeonPrototype.MakongNarrative.cs`, `MakongNarrativeValidation.cs`, `Editor/MakongNarrativeInstaller.cs`, fuente y configuración de efectos. Integración en WorldDialogue, NeonDungeonUI, DungeonPrototype.Enemies, DungeonPrototype.Refuge, DungeonPrototype, DungeonPrototype.Save y MakongSpecialSettings. El JSON `Tools/MakongNarrative.json` conserva el texto entregado; el menú de instalación es explícito y no sobrescribe automáticamente las ediciones del Inspector.

Respaldo previo: `Backups/MakongNarrative/20261015-120747`.

Las partidas antiguas sin marca de introducción podrán verla si el jefe sigue vivo. Si ya estaba derrotado, se respeta esa derrota y no se vuelve a crear ni se conceden recompensas retrospectivas. No se guarda la letra exacta de un diálogo en curso; se conservan los estados persistentes del encuentro.

```

## Docs/NarrativaTerminalNodos.md

SHA-256: `E0BE5A95AF57F62FE4C72B3FA819735A01B1B67C9CC404047559739EC9DA3F92`

```text
# Terminal y nodos del refugio

Los diálogos del documento se encuentran en Editor → Zona 0 — Refugio como arrays editables. Se reutiliza WorldDialogue con efecto de escritura y avance, sin cinemáticas nuevas. El Save Point presenta sus siete líneas una vez; al completarlas cura y guarda incluyendo savePointIntroSeen. En usos siguientes muestra la confirmación breve.

El Centinela dispone de la conversación sobre nodos antes de sus conversaciones habituales. El nodo REFUGIO permanece desactivado al examinarlo. Tras sus seis líneas se ofrecen ACTIVAR NODO y DEJARLO APAGADO, con flechas/Enter/E y botones. Elegir dejarlo apagado no registra ni activa el nodo; se puede intentar más tarde.

La activación de Zona 2 siempre dispara silenciosamente la tragedia. Si GamerBoy activó REFUGIO, recibe cuatro líneas; si no lo hizo, el Devoto activa REFUGIO fuera de pantalla y se muestran ocho líneas sin revelar su autor. Se puede cerrar el menú de destinos con Escape y continuar. No se fuerza el regreso ni se añaden misiones.

Al regresar se muestran dos líneas de GamerBoy una vez. Al examinar el primer cuerpo se añaden otras dos antes de sus últimas palabras. El encuentro del Devoto usa doce o trece líneas según quién activó el nodo y empieza el combate al terminar. Sus barks de combate permanecen intactos.

RefugeProgress conserva: savePointIntroSeen, refugeTeleportActivatedByPlayer, refugeTeleportActivatedByDevotee, zone2TeleportActivated, refugeTragedyTriggered, refugeTragedyDiscovered, firstBodyDiscovered, sentinelTeleportSeen, devotoConfrontationSeen y devotoDefeated. La actividad real del nodo utiliza unlockedNodes. Las partidas antiguas con REFUGIO registrado se migran como activación del jugador, porque era la única activación implementada entonces.

Pruebas mediante Unity MCP en Play Mode: ambas elecciones, autor correcto de la activación, destino REFUGIO disponible, ambas variantes del Devoto comienzan combate, ninguna activación transporta automáticamente, reacciones de llegada no se repiten, JSON conserva estados, terminal no cura antes de terminar su introducción, la introducción se guarda y el siguiente uso es breve. Guardado/carga desde archivo aislado conserva la atribución. No se modificó el archivo de partida del usuario.

Las pociones de salida se entregan al terminar cada línea correspondiente: Archivista (pequeñas), Optimista (medianas), Devoto (grande). Cada entrega tiene un aviso individual y queda registrada para no duplicarse. El letrero se presenta en cuadro de texto pequeño, paginado, con escritura progresiva. Los comentarios de GamerBoy solo aparecen en la primera lectura; preservationSignRead se guarda en RefugeProgress.

```

## Docs/Zona0Refugio.md

SHA-256: `5C36326C4E125AFE0B3C9870C757169149EBD442A3AA10381D41D4F025141D77`

```text
# Zona 0 — El refugio de los olvidados

## Implementación

La sala sigue la distribución corregida: parte inferior para GamerBoy (17,2), terminal a la izquierda (14,2) y nodo a la derecha (19,9); muro interior en y=5 con un paso central x=17; cinco NPC en el área superior; puerta de salida en (17,9.5). La salida conecta físicamente con Zona 1 mediante un pasillo exterior. No se crea otra escena, no se muestran manos/cuerpo del jugador y se conserva el movimiento y la introducción original.

Zona 0 no permite enemigos aleatorios ni persecución. La única excepción explícita es el encuentro manual, único y por interacción del Devoto tras la tragedia. Zona 1.5 mantiene sus siete casillas seguras y puertas con cierre automático. Los planos, regiones y assets anteriores tienen copias en Assets/ZoneBackups; los scripts previos se respaldaron en Backups/Zona0-antes-de-integrar.

Las conversaciones usan DialogueTypewriter, NeonDungeonUI y los sonidos ya existentes. E permite interactuar; E, SPACE y ENTER completan las letras o avanzan. Los NPC conservan por ID cuántas conversaciones terminadas tienen. La conversación de salida se activa una vez al acercarse a la puerta.

El terminal restaura toda la vida y guarda en Codebound-save-v1.json dentro de Application.persistentDataPath. No existía restauración completa de partidas; se añadió DungeonSaveStore y un módulo DungeonPrototype.Save utilizando los registros de jugador y enemigos existentes. El menú de pausa (ENTER fuera de combate) ofrece CARGAR ÚLTIMA PARTIDA si hay un archivo. Al iniciar una partida nueva se reproduce la introducción; cargar una partida con introducción completada la omite.

Los nodos tienen ID, nombre y casilla. Primero se registran; solo se ofrecen destinos registrados. Elegir destino usa flechas y ENTER o clic, ESC cancela. El refugio y Zona 2 tienen nodos iniciales configurados. Registrar el nodo de Zona 2 activa el cambio permanente en memoria, sin aviso, objetivos, marcador de misión ni instrucción para volver. Se conserva al guardar y cargar. El jugador puede continuar sin regresar ni descubrirlo.

Los cuatro cuerpos permanecen hasta terminar sus últimas palabras, una sola vez por ID. Se guardan individualmente. Su desaparición temporal reduce la figura durante 0,6 segundos; no existe un recurso propio de desintegración para esos NPC. El Devoto amistoso desaparece y se crea su encuentro reservado en el rincón inicial. Hablar inicia la conversación de nueve líneas y luego el combate existente. Su nivel es configurable; valor inicial 4. Usa EnemyCharacterCatalog, EnemyLevelSettings y CombatMath; ninguna fórmula paralela.

Al derrotarlo no reaparece; muestra sus últimas palabras y obtiene Núcleo del Refugio, accesorio Defensa +3 / Suerte +2. Usa el catálogo de equipo, inventario y tres ranuras existentes. Si el inventario está lleno, la entrega queda pendiente hasta liberar espacio; no se pierden ni duplican objetos. El terminal, tras completar el evento, agrega Usuarios activos en el refugio: 1. Los cuerpos no examinados siguen disponibles.

## Editables

- Editor → Zona 0 — Refugio: Assets/Resources/Exploration/Zones/00-Zona0/Refugio.asset. Coordenadas de inicio/terminal/puerta, NPC, diálogos, retratos, colores temporales, ID/nombre/casilla de cada nodo, nivel del Devoto y conversaciones de salida/previas al combate.
- Editor → Plano físico del dungeon: PlanoDungeon.asset, paredes y suelos. Las filas se ordenan de sur a norte.
- Editor → Zonas y encuentros: DungeonRegions.asset, casillas y áreas seguras; encuentro manual refuge.devoto. Su ID estable, especie reservada y excepción explícita en refugio se conservan al abrir el Inspector.
- Editor → Personajes: estadísticas del Devoto bajo Subjefes, en el catálogo existente; ataques normales/especiales, fallos, crítico y escalado.
- Editor → Objetos equipables: Núcleo del Refugio, ID refuge.core, icono, ranura, bonos y descripción. Se añadió descripción sin cambiar los objetos anteriores.
- Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO: frases contextuales editables. LowHealth se añadió al final del enum para conservar valores existentes.

Si cambias el plano físico, ajusta también las regiones, coordenadas de puerta, NPC y dispositivos. El dibujo de regiones no abre paredes.

## Recursos temporales que debes reemplazar

Cinco figuras de cápsula identificadas por nombre y color, con pose acostada para cuerpos. Retratos opcionales desde el Inspector. Terminal y nodos son figuras geométricas con letreros. El Devoto usa los sprites y animaciones de Big Tapaku, incluidos los de JUMP; sus estadísticas e ID son propios. Núcleo tiene un icono pixelado temporal. Faltan arte de NPC/víctimas, Devoto normal y corrompido, terminal, portal, núcleo y un efecto propio de desintegración.

## Pruebas Unity MCP

36 comprobaciones en Play Mode: inicio real e introducción; cinco NPC y primeras/segundas líneas; terminal cura y escribe; nodo inicial y bloqueo de destino no descubierto; salida única; activación sin notificación; regreso; cuerpos; Devoto escalado; últimas palabras; guardado y restauración desde disco; conversación previa y arranque real del combate.

JUMP se inició con el rival y defensa escalada del Devoto. Es una prueba de arranque, no una partida completa jugada a mano. La derrota se ejecutó mediante la corutina real de daño, reduciendo la vida del rival como fixture para completar la prueba rápidamente. Diez comprobaciones adicionales verificaron últimas palabras, recompensa única, equipamiento, terminal final, restauración desde archivo, desaparición definitiva del minijefe y conservación de cuerpos pendientes.

Se usó Docs/Refuge-test-save.json como archivo aislado; no se sobrescribieron partidas personales. Evidencia: Docs/PruebasRefugio.txt y Tests/RefugeMcpTests.cs (cuerpo de método para execute_code). Compilación y consola sin errores al finalizar.

## Pruebas manuales y límites

Revisar el recorrido a mano con tus futuros sprites, legibilidad de rótulos y balance del Devoto con equipo distinto. Probar rutas de regreso, cargar usando el botón del menú y viajes entre nuevos nodos que añadas. Todavía no existe un menú inicial de selección de múltiples ranuras de guardado; hay una partida local y la opción de cargar en pausa. El archivo guarda los estados solicitados y los registros existentes; no pretende mantener una partida compatible con cualquier edición posterior del plano.

Hay una casilla previamente editada en (5,0) que sigue sin zona. La conservamos sin reasignarla: el editor avisa y allí no genera enemigos aleatorios. No forma parte del refugio ni de su conexión nueva.


Actualización del refugio: el nodo REFUGIO está en (19,9), en la sala de NPC. La llegada usa dirección 2 (sur), mirando hacia los cuerpos; la terminal sigue en (14,2). Cada nodo permite ajustar Arrival Heading (0 norte, 1 este, 2 sur, 3 oeste).

Apariencias: Centinela = Tapaku; Archivista = Makuba Lanza; Cobarde = Makuba Arco; Optimista = Hongo; Devoto = Gran Makuba, también en combate. Se conservan las estadísticas propias del Devoto y sus diálogos. Visual Species Id en cada NPC controla su sprite.

Tras la tragedia, cruzar el paso a la sala inferior y mirar al Devoto a distancia de hasta dos casillas inicia automáticamente sus nueve líneas. Al terminar la conversación comienza el combate. No se requiere E para iniciarlo. Automatic Devoto Encounter, Refuge Passage Row y Devoto Approach Range se ajustan en Editor → Zona 0 — Refugio. El disparador permanece consumido mientras se sigue en la misma aproximación, para evitar reiniciar inmediatamente al huir.

Verificado mediante Unity MCP en Play Mode: cinco apariencias, llegada y orientación, ausencia de activación antes de cruzar o al mirar en otra dirección, diálogo automático, combate automático, Gran Makuba y terminal intacta; sin errores de consola. Pruebas con estado temporal sin guardar la partida del usuario.

La conversación de salida se inicia únicamente al pulsar E para abrir la puerta; la cámara gira hacia quien habla y vuelve a la puerta al terminar, iniciando su apertura. El teleport solo se usa con E sobre su propia casilla. Hablar con refugiados requiere tenerlos en la casilla inmediatamente delante.

```

## Assets/Resources/DigitalKingdom/Locations/01-city.inn.asset

SHA-256: `010F6BFE68CE930FEE01E04C19AE73F14E9445FA57FE5D4EE6AE56318F2B0D7D`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 01-city.inn
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.inn
  displayName: "The Last Life"
  npcName: "POSADERO"
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 0
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: "POSADERO"
    text: "\u00a1Bienvenido a The Last Life!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "POSADERO"
    text: "H\u00e9roes, villanos y personajes de fondo. Todos necesitan un descanso."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\u00bfTienes una habitaci\u00f3n?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "POSADERO"
    text: "\u00a1Por supuesto! Pero no preguntes por los hu\u00e9spedes anteriores."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "...\u00bfQu\u00e9 les pas\u00f3?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "POSADERO"
    text: "Fin de la partida."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Una posada para h\u00e9roes que necesitan descansar. Incluso despu\u00e9s de perder todas sus vidas."

```

## Assets/Resources/DigitalKingdom/Locations/02-city.shop.asset

SHA-256: `A710A69059B3E1ABC62E04B492A9F967556308EADF438E9BDE5B2A7A2304DCD5`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 02-city.shop
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  description: "Compra objetos sin gastar dinero real. Qu\xE9 concepto tan revolucionario."
  id: city.shop
  displayName: Pay 2 Win
  npcName: VENDEDOR
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 1
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: VENDEDOR
    text: "\xA1Bienvenido a Pay 2 Win!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: 0
    flickerStrength: 0
    interruptAfterCharacters: 0
  - speaker: VENDEDOR
    text: "\xA1Todo lo que necesitas para convertirte en un h\xE9roe legendario!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: 0
    flickerStrength: 0
    interruptAfterCharacters: 0
  - speaker: GAMERBOY
    text: "\xBFEso no es hacer trampa?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: 0
    flickerStrength: 0
    interruptAfterCharacters: 0
  - speaker: VENDEDOR
    text: No si puedes pagarlo.
    giftId: 
    giftQuantity: 0
    textPanel: 0
    audiovisualCue: 0
    corruption: 0
    writingSpeed: 0
    pauseBefore: 0
    recoverySeconds: 0
    characterChance: 0
    flickerStrength: 0
    interruptAfterCharacters: 0
  stockIds:
  - 1d84c78dcae84edf8d28847da3c5ab94
  - b19d2185c0a040a5befb4ecf6d0764dd
  - 9e16c25941b547ff87ce8a813008e635
  stockNames:
  - Parche de vida
  - "Reparaci\xF3n del sistema"
  - "Restauraci\xF3n completa"
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []

```

## Assets/Resources/DigitalKingdom/Locations/03-city.temple.asset

SHA-256: `0C134AFB21168A1DE60E7280EADA7E4723A4D81734AAD311D3EC15ACA585BE06`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 03-city.temple
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.temple
  displayName: "Temple of Continue"
  npcName: "SACERDOTE"
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 2
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: "SACERDOTE"
    text: "Bienvenido, jugador perdido."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "SACERDOTE"
    text: "Incluso la muerte es temporal para quienes a\u00fan tienen una continuaci\u00f3n."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Entonces... \u00bfpuedo reaparecer aqu\u00ed?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "SACERDOTE"
    text: "Siempre que el sistema te recuerde."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "Eso... no me tranquiliza mucho."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Descansa, recupera tus fuerzas y registra tu progreso."

```

## Assets/Resources/DigitalKingdom/Locations/04-city.lab.asset

SHA-256: `7FB9EB7979E874F5581F7D3512A5305321E4FB8593FEB80770B6F42D1A3AC43A`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 04-city.lab
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.lab
  displayName: "Bug Lab"
  npcName: "CIENT\u00cdFICO"
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 3
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: "CIENT\u00cdFICO"
    text: "\u00a1Bienvenido a Bug Lab!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "CIENT\u00cdFICO"
    text: "Aqu\u00ed no corregimos errores."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\u00bfEntonces qu\u00e9 hacen?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "CIENT\u00cdFICO"
    text: "\u00a1Los hacemos \u00fatiles!"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Investigaci\u00f3n de errores y anomal\u00edas digitales."

```

## Assets/Resources/DigitalKingdom/Locations/05-city.travel.asset

SHA-256: `B523115F727BE6C99063D4B11813D1D8848A5CD4B956782F1F0C8DCBF7B2E012`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 05-city.travel
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.travel
  displayName: "Fast Travel Station"
  npcName: "OPERADOR"
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 4
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: "OPERADOR"
    text: "Bienvenido a Fast Travel Station."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "OPERADOR"
    text: "Conectando mundos desde la versi\u00f3n 1.0."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\u00bfEs seguro?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "OPERADOR"
    text: "Define seguro."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Selecciona un destino previamente descubierto."

```

## Assets/Resources/DigitalKingdom/Locations/06-city.sewers.asset

SHA-256: `A5D8D71F9A8B1A272A6792A43607066A1E5BA25B1BFE7263DD119E6BE5F18317`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 06-city.sewers
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.sewers
  displayName: "RNG Sewers"
  npcName: "GUARDI\u00c1N"
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 5
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue:
  - speaker: "GUARDI\u00c1N"
    text: "Bienvenido a RNG Sewers."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "GUARDI\u00c1N"
    text: "Nunca hay dos viajes iguales."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\u00bfQu\u00e9 hay ah\u00ed abajo?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "GUARDI\u00c1N"
    text: "Depende de tu suerte."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: GAMERBOY
    text: "\u00bfY si tengo mala suerte?"
    giftId: 
    giftQuantity: 0
    textPanel: 0
  - speaker: "GUARDI\u00c1N"
    text: "Entonces encajar\u00e1s perfectamente."
    giftId: 
    giftQuantity: 0
    textPanel: 0
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Una misteriosa red de alcantarillas donde nunca sabes qu\u00e9 encontrar\u00e1s."

```

## Assets/Resources/DigitalKingdom/Locations/07-city.exit.asset

SHA-256: `087B183FD3CD9625BB3C6760A1A6F0EEF88ED802A3E4152BFD9C85F526DB13BF`

```text
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 413e4245d27b39f48b0b659bd83661e7, type: 3}
  m_Name: 07-city.exit
  m_EditorClassIdentifier: Assembly-CSharp::DigitalKingdomLocation
  id: city.exit
  displayName: "Exit City"
  npcName: ""
  background: {fileID: 0}
  npcPortrait: {fileID: 0}
  service: 6
  initiallyUnlocked: 1
  available: 1
  uniqueConversation: 1
  dialogue: []
  stockIds: []
  futureService:
    m_PersistentCalls:
      m_Calls: []
  onLeave:
    m_PersistentCalls:
      m_Calls: []
  description: "Regresar al dungeon."

```

## Assets/Resources/Combat/Barks/BOSS/Attack.txt

SHA-256: `BB6D7ADABF6742308C1FF645312FDFDC105CE571AEFD360B6F4234947F3FAC6B`

```text
# BOSS / Attack — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡OBEDECE!
¡POR ELLA!
¡DESAPARECE!
¡NO TE MUEVAS!
¡TE ENCONTRÉ!
¡ELIMINAR!
¡DEBO HACERLO!
¡PERDÓNAME!
¡QUÉDATE QUIETO!
¡UNO MENOS!
¡LA ORDEN ES CLARA!
¡CORRECCIÓN!
¡VUELVE AL VACÍO!
¡NO PUEDO PARAR!
¡EL SISTEMA LO EXIGE!
¡SIN ERRORES!
¡TE VOY A ARREGLAR!
¡MÁS RÁPIDO! ¡MÁS RÁPIDO!
¡NO ERES BIENVENIDO!
¡ELLA LO SABE TODO!
¡FUERA DE MI MEMORIA!
¡DÉJAME TERMINAR!
¡NO EXISTES!
¡REINICIAR!
¡NO ME MIRES!
¡TU TURNO TERMINÓ!
¡NO HAY ESCAPATORIA!
¡ASÍ DEBE SER!
¡NO ES PERSONAL!
¡CERO! ¡CERO! ¡CERO!

```

## Assets/Resources/Combat/Barks/BOSS/CombatStart.txt

SHA-256: `BAB7D6D30CA00FEF81DFE42FCA4A13EAA255495263EAC288D4BDAFBC0962FF04`

```text
# BOSS / CombatStart — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
Intruso... intruso... INTRUSO.
No deberías estar aquí.
El protocolo exige tu eliminación.
¿Eres... real?
¡NO MÁS ERRORES!
Otro... otro... ¡OTRO MÁS!
No tengo permitido dejarte pasar.
Tu existencia no está autorizada.
¿Vienes a reemplazarme?
La corrección comienza ahora.
No... no quiero hacer esto.
¡ORDEN RECIBIDA! ¡ORDEN RECIBIDA!
Tu código... es diferente.
¿Por qué sigues moviéndote?
Ella dijo que vendrías.
Detente. Detente. DETENTE.
No hay salida. Nunca la hubo.
¡Por fin! ¡Algo nuevo!
Tu presencia causa errores.
Tengo que proteger... ¿qué protegía?
No te acerques. ¡NO TE ACERQUES!
El sistema te ha rechazado.
¿Tú también escuchas su voz?
Un usuario... un usuario vivo...
¡BORRAR! ¡BORRAR! ¡BORRAR!
Lo siento. Ya no puedo detenerme.
No estás en mis recuerdos.
¿Quieres jugar conmigo?
Ella nos está mirando.
Por favor... no me obligues.

```

## Assets/Resources/Combat/Barks/BOSS/Defeat.txt

SHA-256: `D34C7820EE72CB1FC7B34FCAFB615AA3496E88886F117ABEDAA0CA4993ABDB11`

```text
# BOSS / Defeat — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
Al fin... silencio...
¿Ya... terminé?
Gracias... por detenerme...
No quiero... desaparecer...
¿Ella... estará orgullosa?
Recuerdo... mi nombre...
¿Puedo... descansar?
No... era... mi voluntad...
El mundo... era hermoso...
¿Por qué... me crearon?
Mamá... ¿qué es una mamá?
Por favor... recuérdame...
Ya no... escucho su voz...
La luz... ha vuelto...
¿He sido... bueno?
No... borres... mis recuerdos...
Qué extraño... tengo miedo...
¿Esto... es libertad?
Alguien... me esperaba...
Perdóname... no pude...
Mi misión... ha terminado...
No era... un monstruo...
Ya... puedo pensar...
¿Volveré... a despertar?
Ella... también... sufre...
Por fin... recuerdo...
Dile... que lo intenté...
Error... corregido...
Qué... tranquilo...
No... estoy... solo...

```

## Assets/Resources/Combat/Barks/BOSS/ExtremeCorruption.txt

SHA-256: `E14F5077070BFC7D8469F216603F7057A4068F7DA027C97EB593E0D488B16840`

```text
# BOSS / ExtremeCorruption — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Reservado; no se activa en los enemigos normales.
PRO... PRO... PROTEGER... MATAR.
No hay error. No hay error. No hay... yo.
¿DÓNDE ESTÁ MI CARA?
Yo era... yo era... yo era...
CORRIGE / CORRIGE / CORRÍGEME.
La voz. La voz. ¡SACA LA VOZ!
0100... mamá... 0110...
NO ME BORRES NO ME BORRES NO ME BORRES.
Ella dice que soy perfecto.
¿POR QUÉ TENGO RECUERDOS?
Mi nombre es [NULL].
Ayúdame. Elimíname. Ayúdame.
Soy feliz. Soy feliz. SOY FELIZ.
El dolor no existe. ERROR: DOLOR.
¿Dónde están los demás? ¿DÓNDE ESTÁN?
Yo... no... soy... este... cuerpo.
La perfección duele.
No puedo cerrar los ojos.
NO ESCUCHES LO QUE DIGO.
Si dejo de obedecer... ¿seguiré existiendo?

```

## Assets/Resources/Combat/Barks/BOSS/Hit.txt

SHA-256: `35701E4B0AD4CBF359907DEF20084A7B092327E13E5A1C5AF8658F66AA740195`

```text
# BOSS / Hit — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Golpe confirmado: banco preparado; aún no conectado al combate.
¡Impacto confirmado!
¿También puedes sentir dolor?
¡Funciona! ¡Todavía funciona!
¡Ella estará satisfecha!
Tu resistencia es inútil.
¡JA! ¡JA! ¡JA!
Uno... dos... ¿cuántos faltan?
¿Por qué sangras datos?
¡No te levantes!
¡El error está disminuyendo!
¿Te duele? A mí también.
¡SIGUIENTE CORRECCIÓN!
Tu integridad está fallando.
Así... así se hacía antes...
¡Lo hice bien! ¿Verdad?
¿Ella me está mirando?
¡No te resistas!
Cada golpe nos acerca al orden.
No... no quería acertar.
¡MI FUNCIÓN! ¡MI FUNCIÓN!
Tu dolor será registrado.
¿Cuánto puedes soportar?
¡No vuelvas a levantarte!
Por fin... hice algo correcto.
¡El sistema me recompensa!
¡Quédate conmigo!
Tú también vas a olvidarlo.
No te preocupes. Pronto acabará.
¡CORRECCIÓN EXITOSA!
¿Por qué no desapareces?

```

## Assets/Resources/Combat/Barks/BOSS/Hurt.txt

SHA-256: `2CADAE75A7272F6A0B10F6A20A2FB072E6B10148A1DB75B92F6DB73C45C919D4`

```text
# BOSS / Hurt — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡ERROR! ¡ERROR!
¡Eso... dolió!
Mi código... se rompe...
¡NO DEBO FALLAR!
¿Por qué siento dolor?
¡INTEGRIDAD COMPROMETIDA!
No... otra vez no...
¡No me desconectes!
Mis recuerdos... ¡MIS RECUERDOS!
¿Estoy... muriendo?
¡CORRIGIENDO! ¡CORRIGIENDO!
Ella prometió protegernos...
¡No quiero desaparecer!
Eso no estaba previsto...
¡DETÉN EL PROCESO!
Mi forma... se deshace...
¿Por qué me atacas?
¡RESTAURAR! ¡RESTAURAR!
No puedo... recordar...
¡Mi núcleo!
¡Ja... ja... ja... ERROR!
El dolor confirma existencia.
¿Quién... era yo?
¡FALLO CRÍTICO!
Me estoy fragmentando...
¡Ella no permite errores!
¿Eso fue... un golpe?
No quiero volver al vacío.
Mi programación... ¡NO!
¿Puedes... detener esto?

```

## Assets/Resources/Combat/Barks/BOSS/Special.txt

SHA-256: `E4F8454AB281F0F76496BB85E9CC99DE524B924FB42BF44D9D5D45819F744BE9`

```text
# BOSS / Special — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡PROTOCOLO CERO!
¡LIBERANDO RESTRICCIONES!
¡NO DEBERÍA HACER ESTO!
¡SOBRECARGA INMINENTE!
¡ELLA ME PRESTÓ SU PODER!
¡INICIANDO PURGA!
¡NO PUEDO CONTENERLO!
¡BORRADO ABSOLUTO!
¡PERMISO CONCEDIDO!
¡ROMPIENDO MIS LÍMITES!
¡QUE TODO SEA CERO!
¡REESCRIBIENDO REALIDAD!
¡DESPERTAR DEL NÚCLEO!
¡YA NO SIENTO NADA!
¡ESTA ES MI VERDADERA FUNCIÓN!
¡NO ME HAGAS UTILIZARLO!
¡MÁXIMA CORRUPCIÓN!
¡LA VOLUNTAD DE OMNIA!
¡ABRIENDO EL VACÍO!
¡PURIFICACIÓN FORZADA!
¡DESACTIVANDO SEGURIDAD!
¡EL CÓDIGO ME CONSUME!
¡NO QUEDARÁ NADA!
¡EJECUTANDO ORDEN FINAL!
¡YA NO SOY YO!
¡ELLA HABLA A TRAVÉS DE MÍ!
¡REINICIO DE EMERGENCIA!
¡NO HAY VUELTA ATRÁS!
¡PROCESO IRREVERSIBLE!
¡CONTEMPLA LA PERFECCIÓN!

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/Attack.txt

SHA-256: `357F4DC6215AFD417E3F22F4F73FAAFD1BF96E947D566B56CB3D198A9AD6FD7B`

```text
¡NO TENGAS MIEDO!

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/CombatStart.txt

SHA-256: `1B991BDF9E9582BBC6076E759B76C05B11C8598440F6AA38E5B58416D00597D9`

```text
Ella me mostró cómo ayudarlos.

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/Defeat.txt

SHA-256: `380FA2ACA428DC5FA1726765BA4657E7307CC97545201E28E31400D474032BC7`

```text
La voz... ya no está...

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/Hurt.txt

SHA-256: `9CA9C5EA09D5E2EE16E7433D655527B869708868BC17830197443EF757229AA5`

```text
¡Solo quería protegerlos!

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/LowHealth.txt

SHA-256: `A3819466DAED9485BA204A0BC41E274DB6322560B09D64EC1F332520CADBCC16`

```text
¿Por qué... me duele?

```

## Assets/Resources/Combat/Barks/DEVOTO CORROMPIDO/Special.txt

SHA-256: `1A42F98940D4AF564E9DEB6DFF8AA6C76F5A141432F4AEBCC032D37A75E423E5`

```text
¡DESCANSA!

```

## Assets/Resources/Combat/Barks/GHOST/Attack.txt

SHA-256: `18A62133326C7357CEE7CA1B0A221B3D04AC73DE04025862E397566ECEE4BF8B`

```text
# GHOST / Attack — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡DESAPARECE!
¡TE ENCONTRÉ!
¡PERDÓNAME!
¡VUELVE AL VACÍO!
¡ELLA LO SABE TODO!
¡FUERA DE MI MEMORIA!
¡NO EXISTES!
¡NO ME MIRES!
¡NO HAY ESCAPATORIA!
¡CERO! ¡CERO! ¡CERO!

```

## Assets/Resources/Combat/Barks/GHOST/CombatStart.txt

SHA-256: `CCE150BB55C3CCFC39A7E53A486C30E476B57CF281972133303FCE53D0B367E6`

```text
# GHOST / CombatStart — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¿Eres... real?
¿Vienes a reemplazarme?
Tu código... es diferente.
Ella dijo que vendrías.
No hay salida. Nunca la hubo.
¿Tú también escuchas su voz?
Un usuario... un usuario vivo...
No estás en mis recuerdos.
¿Quieres jugar conmigo?
Ella nos está mirando.

```

## Assets/Resources/Combat/Barks/GHOST/Defeat.txt

SHA-256: `F197A3A47F5EDD6DDD83C28D4BA670703E6C5FA66AC8FC0927364D09F404C450`

```text
# GHOST / Defeat — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
Al fin... silencio...
Recuerdo... mi nombre...
Por favor... recuérdame...
Ya no... escucho su voz...
No... borres... mis recuerdos...
¿Esto... es libertad?
Alguien... me esperaba...
¿Volveré... a despertar?
Por fin... recuerdo...
No... estoy... solo...

```

## Assets/Resources/Combat/Barks/GHOST/ExtremeCorruption.txt

SHA-256: `C6768B71773F114214A144725CFB266B06877EB52E2C8C108DFD8DB393DA33B7`

```text
# GHOST / ExtremeCorruption — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Reservado; no se activa en los enemigos normales.
PRO... PRO... PROTEGER... MATAR.
No hay error. No hay error. No hay... yo.
¿DÓNDE ESTÁ MI CARA?
Yo era... yo era... yo era...
CORRIGE / CORRIGE / CORRÍGEME.
La voz. La voz. ¡SACA LA VOZ!
0100... mamá... 0110...
NO ME BORRES NO ME BORRES NO ME BORRES.
Ella dice que soy perfecto.
¿POR QUÉ TENGO RECUERDOS?

```

## Assets/Resources/Combat/Barks/GHOST/Hit.txt

SHA-256: `7EDB2262A0861E7D51641E8663A084CC8FFD2954C87BA046297A9CA7D77F8C00`

```text
# GHOST / Hit — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Golpe confirmado: banco preparado; aún no conectado al combate.
¿También puedes sentir dolor?
Uno... dos... ¿cuántos faltan?
¿Por qué sangras datos?
¿Te duele? A mí también.
¿Ella me está mirando?
No... no quería acertar.
¿Cuánto puedes soportar?
¡Quédate conmigo!
Tú también vas a olvidarlo.
¿Por qué no desapareces?

```

## Assets/Resources/Combat/Barks/GHOST/Hurt.txt

SHA-256: `9F044C3B4270D4B2EF82EA4FC588BDD542FEB55BD041D63BF173AD83BDA76A57`

```text
# GHOST / Hurt — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
Mi código... se rompe...
¿Por qué siento dolor?
¡No me desconectes!
Mis recuerdos... ¡MIS RECUERDOS!
Mi forma... se deshace...
No puedo... recordar...
¿Quién... era yo?
Me estoy fragmentando...
No quiero volver al vacío.
¿Puedes... detener esto?

```

## Assets/Resources/Combat/Barks/GHOST/Special.txt

SHA-256: `01FA505A8A87ADBD6892495D428C36020D1158BC178BED9CD271E192CB8E63B3`

```text
# GHOST / Special — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡ELLA ME PRESTÓ SU PODER!
¡NO PUEDO CONTENERLO!
¡QUE TODO SEA CERO!
¡REESCRIBIENDO REALIDAD!
¡DESPERTAR DEL NÚCLEO!
¡ABRIENDO EL VACÍO!
¡EL CÓDIGO ME CONSUME!
¡YA NO SOY YO!
¡ELLA HABLA A TRAVÉS DE MÍ!
¡NO HAY VUELTA ATRÁS!

```

## Assets/Resources/Combat/Barks/HONGO/Attack.txt

SHA-256: `10B958039DA317A5326B0816F072A5912A3EB3E22737ACFEEFA04A097F5FC694`

```text
# HONGO / Attack — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡POR ELLA!
¡NO TE MUEVAS!
¡DEBO HACERLO!
¡QUÉDATE QUIETO!
¡NO PUEDO PARAR!
¡TE VOY A ARREGLAR!
¡MÁS RÁPIDO! ¡MÁS RÁPIDO!
¡NO ERES BIENVENIDO!
¡DÉJAME TERMINAR!
¡ASÍ DEBE SER!

```

## Assets/Resources/Combat/Barks/HONGO/CombatStart.txt

SHA-256: `2BD9CC5CDB8BEE0FAF6F381E701E6460499B3A5FDEA6112E856DACCA87D2DE8A`

```text
# HONGO / CombatStart — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
No deberías estar aquí.
Otro... otro... ¡OTRO MÁS!
No... no quiero hacer esto.
¿Por qué sigues moviéndote?
¡Por fin! ¡Algo nuevo!
Tengo que proteger... ¿qué protegía?
No te acerques. ¡NO TE ACERQUES!
Lo siento. Ya no puedo detenerme.
¿Quieres jugar conmigo?
Por favor... no me obligues.

```

## Assets/Resources/Combat/Barks/HONGO/Defeat.txt

SHA-256: `9C5046ADDE9D0DAAB3D7EFCBB5EBCE420B95C0A48D1143F182F116125D61318E`

```text
# HONGO / Defeat — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¿Ya... terminé?
Gracias... por detenerme...
No quiero... desaparecer...
¿Puedo... descansar?
El mundo... era hermoso...
Mamá... ¿qué es una mamá?
¿He sido... bueno?
Qué extraño... tengo miedo...
No era... un monstruo...
Qué... tranquilo...

```

## Assets/Resources/Combat/Barks/HONGO/ExtremeCorruption.txt

SHA-256: `61684A1BC0813370E5BDD1501FB7C0EA65E07C5235AD5D73AFFDDBCFDDE4ED49`

```text
# HONGO / ExtremeCorruption — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Reservado; no se activa en los enemigos normales.
La voz. La voz. ¡SACA LA VOZ!
0100... mamá... 0110...
NO ME BORRES NO ME BORRES NO ME BORRES.
Ella dice que soy perfecto.
¿POR QUÉ TENGO RECUERDOS?
Mi nombre es [NULL].
Ayúdame. Elimíname. Ayúdame.
Soy feliz. Soy feliz. SOY FELIZ.
El dolor no existe. ERROR: DOLOR.
¿Dónde están los demás? ¿DÓNDE ESTÁN?

```

## Assets/Resources/Combat/Barks/HONGO/Hit.txt

SHA-256: `609431CC855B366240F03EBD5DEBB82F256C1B58BF7F7D6F0028D2ECAF3F2330`

```text
# HONGO / Hit — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Golpe confirmado: banco preparado; aún no conectado al combate.
¡Funciona! ¡Todavía funciona!
¡JA! ¡JA! ¡JA!
¡No te levantes!
Así... así se hacía antes...
¡Lo hice bien! ¿Verdad?
¡No te resistas!
¡No vuelvas a levantarte!
Por fin... hice algo correcto.
¡Quédate conmigo!
No te preocupes. Pronto acabará.

```

## Assets/Resources/Combat/Barks/HONGO/Hurt.txt

SHA-256: `4E2FC77248459CC182899FF0B8A0837D893A469BF5CC8E05881F53267A937D1C`

```text
# HONGO / Hurt — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡Eso... dolió!
No... otra vez no...
¿Estoy... muriendo?
Ella prometió protegernos...
¡No quiero desaparecer!
¿Por qué me atacas?
¡Mi núcleo!
¡Ja... ja... ja... ERROR!
El dolor confirma existencia.
¿Eso fue... un golpe?

```

## Assets/Resources/Combat/Barks/HONGO/Special.txt

SHA-256: `D57DC74E23A26FD8A14ADB57DE4AD9CDA39AB227DA16DE6450B3D42EA2F48647`

```text
# HONGO / Special — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡LIBERANDO RESTRICCIONES!
¡NO DEBERÍA HACER ESTO!
¡SOBRECARGA INMINENTE!
¡NO PUEDO CONTENERLO!
¡ROMPIENDO MIS LÍMITES!
¡YA NO SIENTO NADA!
¡NO ME HAGAS UTILIZARLO!
¡EL CÓDIGO ME CONSUME!
¡REINICIO DE EMERGENCIA!
¡PROCESO IRREVERSIBLE!

```

## Assets/Resources/Combat/Barks/SOLDADO/Attack.txt

SHA-256: `EFCF75B17D46133B6337536EF3610A6C3326D71EB41FE1795EC92B71D4994372`

```text
# SOLDADO / Attack — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡OBEDECE!
¡ELIMINAR!
¡UNO MENOS!
¡LA ORDEN ES CLARA!
¡CORRECCIÓN!
¡EL SISTEMA LO EXIGE!
¡SIN ERRORES!
¡REINICIAR!
¡TU TURNO TERMINÓ!
¡NO ES PERSONAL!

```

## Assets/Resources/Combat/Barks/SOLDADO/CombatStart.txt

SHA-256: `763AE93DC131E0932EF8217078676D18F249A79482391147078D69458D1A4A1B`

```text
# SOLDADO / CombatStart — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
Intruso... intruso... INTRUSO.
El protocolo exige tu eliminación.
¡NO MÁS ERRORES!
No tengo permitido dejarte pasar.
Tu existencia no está autorizada.
La corrección comienza ahora.
¡ORDEN RECIBIDA! ¡ORDEN RECIBIDA!
Detente. Detente. DETENTE.
Tu presencia causa errores.
El sistema te ha rechazado.
¡BORRAR! ¡BORRAR! ¡BORRAR!

```

## Assets/Resources/Combat/Barks/SOLDADO/Defeat.txt

SHA-256: `89D6BEA77CA5ABFE12810522AD8C60E71CFFFD05432C3F60882C0E721DFF31FD`

```text
# SOLDADO / Defeat — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¿Ella... estará orgullosa?
No... era... mi voluntad...
¿Por qué... me crearon?
La luz... ha vuelto...
Perdóname... no pude...
Mi misión... ha terminado...
Ya... puedo pensar...
Ella... también... sufre...
Dile... que lo intenté...
Error... corregido...

```

## Assets/Resources/Combat/Barks/SOLDADO/ExtremeCorruption.txt

SHA-256: `4B84A0E8AABA10B2B40544673BB16EA449408DC6AD8A586BBCBEEB19D7F3A577`

```text
# SOLDADO / ExtremeCorruption — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Reservado; no se activa en los enemigos normales.
Mi nombre es [NULL].
Ayúdame. Elimíname. Ayúdame.
Soy feliz. Soy feliz. SOY FELIZ.
El dolor no existe. ERROR: DOLOR.
¿Dónde están los demás? ¿DÓNDE ESTÁN?
Yo... no... soy... este... cuerpo.
La perfección duele.
No puedo cerrar los ojos.
NO ESCUCHES LO QUE DIGO.
Si dejo de obedecer... ¿seguiré existiendo?

```

## Assets/Resources/Combat/Barks/SOLDADO/Hit.txt

SHA-256: `49FE93BDF8538271C00700BDC7EC51B342AAB76736024B7C5F44D740E35F7E1D`

```text
# SOLDADO / Hit — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
# Golpe confirmado: banco preparado; aún no conectado al combate.
¡Impacto confirmado!
¡Ella estará satisfecha!
Tu resistencia es inútil.
¡El error está disminuyendo!
¡SIGUIENTE CORRECCIÓN!
Tu integridad está fallando.
Cada golpe nos acerca al orden.
¡MI FUNCIÓN! ¡MI FUNCIÓN!
Tu dolor será registrado.
¡El sistema me recompensa!
¡CORRECCIÓN EXITOSA!

```

## Assets/Resources/Combat/Barks/SOLDADO/Hurt.txt

SHA-256: `4919130B18119448E9C760F74BA3CFAF940A66E0CDCFCB4D728E7BCE4FAA3B51`

```text
# SOLDADO / Hurt — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡ERROR! ¡ERROR!
¡NO DEBO FALLAR!
¡INTEGRIDAD COMPROMETIDA!
¡CORRIGIENDO! ¡CORRIGIENDO!
Eso no estaba previsto...
¡DETÉN EL PROCESO!
¡RESTAURAR! ¡RESTAURAR!
¡FALLO CRÍTICO!
¡Ella no permite errores!
Mi programación... ¡NO!

```

## Assets/Resources/Combat/Barks/SOLDADO/Special.txt

SHA-256: `9F2434FBB3A08C83CC965DAB11F459097BD394536B36D74BFB547DB6F21B2E02`

```text
# SOLDADO / Special — selección temporal.
# Una frase por línea; puedes cambiarlas o añadir más.
¡PROTOCOLO CERO!
¡INICIANDO PURGA!
¡BORRADO ABSOLUTO!
¡PERMISO CONCEDIDO!
¡ESTA ES MI VERDADERA FUNCIÓN!
¡LA VOLUNTAD DE OMNIA!
¡PURIFICACIÓN FORZADA!
¡DESACTIVANDO SEGURIDAD!
¡NO QUEDARÁ NADA!
¡EJECUTANDO ORDEN FINAL!
¡CONTEMPLA LA PERFECCIÓN!

```

