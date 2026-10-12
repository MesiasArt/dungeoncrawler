using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/UI Layout")]
public class DungeonUILayout : ScriptableObject
{
 [Header("Positions and sizes in a 1280 x 720 reference screen")]
 [Tooltip("Position from the top-left corner.")] public Rect playerHealth=new Rect(170.1f,20,329.9f,132);
 [Tooltip("Independent HUD portrait position and size from the top-left.")] public Rect playerAvatar=new Rect(20,20,150.1f,132);
 [Tooltip("Position from the top-right corner. Use a negative X.")] public Rect map=new Rect(-240,20,220,200);
 [Tooltip("Position from the bottom-right corner. Use negative X and Y.")] public Rect message=new Rect(-510,-118,490,98);
 [Tooltip("Position from the bottom-left corner. X is positive; Y is negative.")] public Rect dialogue=new Rect(190,-225,680,207);
 [Tooltip("GamerBoy portrait position from the bottom-left.")] public Rect dialogueAvatar=new Rect(20,-200,160,141);
 [Header("Text appearance")]
 [Tooltip("Zero shows complete text immediately.")] [Min(0)] public float lettersPerSecond=35;
 [Min(12)] public int textFontSize=18;
 [Min(.1f)] public float continueBlinkSeconds=.45f;
 [Tooltip("Position from the top-center.")] public Rect enemyHealth=new Rect(-180,15,360,114);
 [Tooltip("Bottom-left offset for the combat button stack.")] public Vector2 combatOffset=new Vector2(20,-33);
 public Vector2 combatButtonSize=new Vector2(280,58);
 [Min(0)] public float combatButtonGap=4;
 [Tooltip("Preserve the original aspect of dialogue and message artwork.")] public bool preservePanelAspect=true;
 [Header("Discovered map tiles")]
 public Color discoveredFloor=new Color(0,.55f,.8f,.85f);
 public Color discoveredWall=new Color(.3f,.22f,.55f,.95f);
 public Rect Resolve(Rect rect,float anchorX,float anchorY) { return new Rect(anchorX+rect.x,anchorY+rect.y,Mathf.Max(1,rect.width),Mathf.Max(1,rect.height)); }
}
