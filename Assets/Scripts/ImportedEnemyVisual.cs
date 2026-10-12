using UnityEngine;
// PSD frames share a canvas and a foot anchor: poses do not recenter on each frame.
public abstract class ImportedEnemyVisual : GhostEnemyVisual
{
 protected abstract int AttackFrames { get; }
 protected abstract int SpecialFrames { get; }
 protected override string FramesPath { get { return "Enemies/"+EnemyRoster.ResourceIds[Kind]+"/"; } }
 protected override Vector2 SpriteSize { get { return EnemyRoster.CanvasSize(Kind); } }
 protected override Vector2 SpriteOffset { get { var offset=EnemyRoster.CanvasOffset(Kind);return new Vector2(-offset.x,offset.y); } }
 [Min(1)] public int specialEveryAttacks=3;
 [Min(.05f)] public float poseFrameSeconds=.14f;
 Texture2D[] attackSequence,specialSequence;int attacks;
 protected override void Awake(){
  attackSequence=new Texture2D[AttackFrames];for(int i=0;i<attackSequence.Length;i++)attackSequence[i]=Load("attack_"+i);
  specialSequence=new Texture2D[SpecialFrames];for(int i=0;i<specialSequence.Length;i++)specialSequence[i]=Load("special_"+i);
  base.Awake();
 }
 public override bool NextAttackIsSpecial(){return SpecialFrames>0&&++attacks%Mathf.Max(1,specialEveryAttacks)==0;}
 protected override Texture AttackFrame(float elapsed){return attackSequence[Mathf.Clamp(Mathf.FloorToInt(elapsed/Mathf.Max(.05f,poseFrameSeconds)),0,attackSequence.Length-1)];}
 protected override Texture SpecialFrame(float elapsed){return specialSequence.Length>0?specialSequence[Mathf.Clamp(Mathf.FloorToInt(elapsed/Mathf.Max(.05f,poseFrameSeconds)),0,specialSequence.Length-1)]:AttackFrame(elapsed);}
 protected override float Duration(Pose pose){return pose==Pose.Special?Mathf.Max(minimumSpecialSeconds,SpecialFrames*poseFrameSeconds):pose==Pose.Attack?Mathf.Max(attackSeconds,AttackFrames*poseFrameSeconds):base.Duration(pose);}
}
