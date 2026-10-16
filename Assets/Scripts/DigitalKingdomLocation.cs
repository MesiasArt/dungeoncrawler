using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Digital Kingdom/Establecimiento")]
public class DigitalKingdomLocation:ScriptableObject {
 [TextArea(2,5)] public string description;
 [TextArea(2,5)] public string personality;
 public string welcomeGiftId; [Min(1)] public int welcomeGiftQuantity=1;
 [TextArea] public string welcomeGiftDialogue;
 public string id,displayName,npcName;public Texture2D background,npcPortrait;
 public DigitalKingdomService service;public bool initiallyUnlocked=true,available=true,uniqueConversation=true;
 public WorldDialogueLine[] dialogue=new WorldDialogueLine[0];
 [Tooltip("IDs del inventario existente; catálogo provisional sin compras todavía.")]public string[] stockIds=new string[0];
 [Tooltip("Nombres traducidos opcionales para los objetos del catalogo, en el mismo orden que Stock IDs.")]public string[] stockNames=new string[0];
 public UnityEngine.Events.UnityEvent futureService,onLeave;
}
