using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Digital Kingdom/Establecimiento")]
public class DigitalKingdomLocation:ScriptableObject {
 public string id,displayName,npcName;public Texture2D background,npcPortrait;
 public DigitalKingdomService service;public bool initiallyUnlocked=true,available=true,uniqueConversation=true;
 public WorldDialogueLine[] dialogue=new WorldDialogueLine[0];
 [Tooltip("IDs del inventario existente; catálogo provisional sin compras todavía.")]public string[] stockIds=new string[0];
 public UnityEngine.Events.UnityEvent futureService,onLeave;
}
