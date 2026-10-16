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
 [Range(12,48)] public int commandFontSize=32;
 [Range(10,22)] public int descriptionFontSize=13;
 [Header("Command text — tilt follows the artwork")]
 public Vector2 commandTextAnchor=new Vector2(.48f,.50f);
 [Range(.2f,.6f)] public float commandTextWidth=.46f;
 public Vector4 commandTextVerticalOffsets=new Vector4(0,-.04f,-.07f,-.07f);
 public float attackTextAngle=8, hackTextAngle=2, itemTextAngle=-6, fleeTextAngle=-11;
 [Range(-.4f,.4f)] public float commandTextShear=.08f;
 [Range(.6f,1.2f)] public float commandTextHeight=.95f;
 [Header("SYSTEM MESSAGE — inner margins in reference pixels")]
 public Rect systemTitle=new Rect(38,17,700,18);
 public Rect systemBody=new Rect(38,39,700,34);
 public Rect systemIcon=new Rect(34,39,24,24);
 public int systemTitleFontSize=16,systemBodyFontSize=19;
 [Header("Rectangular HACK menu")]
 public Vector2 hackButtonSize=new Vector2(230,66);
 [Range(16,40)] public int hackButtonFontSize=27;
 [Range(.08f,.6f)] public float menuSlideSeconds=.28f;
 [Tooltip("Extra distance beyond the left edge in reference pixels")]
 [Min(0)] public float menuSlidePadding=24;
 public static BattleUISettings Current {get{return Resources.Load<BattleUISettings>("UI/BattleUI/Settings");}}
}
