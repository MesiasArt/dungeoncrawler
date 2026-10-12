using UnityEngine;
[System.Serializable] public class RefugeNpcDefinition {
 public string id,displayName;public Vector2Int cell;public Texture2D portrait;
 public string visualSpeciesId;
 [TextArea(2,5)] public string[] conversations;
 [TextArea(2,4)] public string lastWords;
 public Color placeholderColor=new Color(.25f,.65f,.85f);
}
[System.Serializable] public class TransferNodeDefinition {public string id,displayName;public Vector2Int cell;[Range(0,3)] public int arrivalHeading;public bool triggersRefugeTragedy;}
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
 public bool preservationSignRead;public System.Collections.Generic.List<string> departureGiftIds=new System.Collections.Generic.List<string>();
 public int narrativeVersion;
 public bool savePointIntroSeen,refugeTeleportActivatedByPlayer,refugeTeleportActivatedByDevotee,zone2TeleportActivated,refugeTragedyDiscovered,firstBodyDiscovered,sentinelTeleportSeen,devotoConfrontationSeen;
 public bool exitDialogueCompleted,refugeTragedyTriggered,devotoDefeated,rewardObtained;
 public System.Collections.Generic.List<string> unlockedNodes=new System.Collections.Generic.List<string>();
 public System.Collections.Generic.List<RefugeNpcState> npcs=new System.Collections.Generic.List<RefugeNpcState>();
}
