using UnityEngine;
public sealed class BigTapakuEnemyVisual : GhostEnemyVisual
{
 protected override string FramesPath { get { return "Enemies/BigTapaku/"; } }
 protected override Vector2 SpriteSize { get { return new Vector2(3.19f,1.8f); } }
 Texture2D[] spinning;int attacks;
 [Min(.05f)] public float specialFrameSeconds=.12f;
 [Min(1)] public int specialEveryAttacks=3;
 public override bool NextAttackIsSpecial(){return ++attacks%Mathf.Max(1,specialEveryAttacks)==0;}
 protected override void Awake(){spinning=new Texture2D[6];for(int i=0;i<6;i++)spinning[i]=Load("special_"+i);base.Awake();}
 protected override float Duration(Pose pose){return pose==Pose.Special?Mathf.Max(minimumSpecialSeconds,6*Mathf.Max(.05f,specialFrameSeconds)):base.Duration(pose);}
 protected override Texture SpecialFrame(float elapsed){return spinning[Mathf.Clamp(Mathf.FloorToInt(elapsed/Mathf.Max(.05f,specialFrameSeconds)),0,5)];}
}
