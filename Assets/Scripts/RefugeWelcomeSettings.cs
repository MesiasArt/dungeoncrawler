using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Refuge welcome")]
public sealed class RefugeWelcomeSettings:ScriptableObject {
 public Vector2Int triggerCell=new Vector2Int(17,6);
 public DungeonInteractionSettings dialogue;
 [Min(.1f)]public float turnSeconds=.55f,lookHoldSeconds=.65f;
 public static RefugeWelcomeSettings Current {get{return Resources.Load<RefugeWelcomeSettings>("Exploration/RefugeWelcome");}}
}
