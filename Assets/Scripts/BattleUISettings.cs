using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Battle UI")]
public sealed class BattleUISettings:ScriptableObject {
 public bool useNewArtwork=true;
 [Header("1280 x 720 reference screen")]
 public Rect player=new Rect(20,12,380,140);
 [Tooltip("Offset from top-center")] public Rect enemy=new Rect(-180,18,360,120);
 [Tooltip("Bottom-left anchored command stack")] public Vector2 commandOffset=new Vector2(20,-120);
 public Vector2 commandSize=new Vector2(220,78);
 public float commandGap=3;
 [Tooltip("Bottom-left anchored SYSTEM MESSAGE")] public Rect message=new Rect(20,-102,780,88);
 [Range(12,28)] public int commandFontSize=21;
 [Range(10,22)] public int descriptionFontSize=13;
 public static BattleUISettings Current {get{return Resources.Load<BattleUISettings>("UI/BattleUI/Settings");}}
}
