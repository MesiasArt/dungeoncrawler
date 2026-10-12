using UnityEngine;
using System.Collections.Generic;
public sealed class MakongEnemyVisual:GhostEnemyVisual {
 public override int Kind {get{return EnemyRoster.Makong;}}
 protected override string FramesPath {get{return "Enemies/Makong/";}}
 public static readonly Vector2 FullCanvas=new Vector2(600,350)*(1.8f/190);
 public static readonly Vector2 FootOffset=new Vector2(0,-150)*(1.8f/190);
 protected override Vector2 SpriteSize {get{return FullCanvas;}}
 protected override Vector2 SpriteOffset {get{return FootOffset;}}
 readonly Dictionary<string,Texture> poses=new Dictionary<string,Texture>();
 readonly List<GameObject> effects=new List<GameObject>();
 Texture special;int turns;bool flip;
 public MakongTactics Tactics {get;private set;}=new MakongTactics();
 public void ResetCombatTactics(){Tactics=new MakongTactics();}
 protected override void Awake(){base.Awake();foreach(string key in new[]{"attack_0","attack_1","death_0","death_1","jump_0","jump_1","jump_2","jump_3","jump_4","swallow_prepare_0","swallow_prepare_1","swallow_tongue_0","swallow_tongue_1","swallow_tongue_2","swallow_eat_0","swallow_eat_1"})poses[key]=Load(key);deathSeconds=.8f;}
 public override bool NextAttackIsSpecial(){return false;}
 protected override Texture AttackFrame(float elapsed){Texture t;return poses.TryGetValue(elapsed<MakongSpecialSettings.Current.normalFrameSeconds?"attack_0":"attack_1",out t)?t:base.AttackFrame(elapsed);}
 protected override Texture DeathFrame(float elapsed){Texture t;return poses.TryGetValue(elapsed<.32f?"death_0":"death_1",out t)?t:base.DeathFrame(elapsed);}
 protected override Texture SpecialFrame(float elapsed){return special??base.SpecialFrame(elapsed);}
 protected override float Duration(Pose pose){return pose==Pose.Special?10000:pose==Pose.Attack?MakongSpecialSettings.Current.normalFrameSeconds*2:base.Duration(pose);}
 public void Hold(string frame){special=poses[frame];Play(Pose.Special);}
 public void FaceCompanion(Vector3 companion,Vector3 cameraRight){flip=Vector3.Dot(companion-transform.position,cameraRight)<0;}
 public void ShowBody(bool visible){if(SpriteTransform!=null)SpriteTransform.GetComponent<Renderer>().enabled=visible;}
 public void OwnEffect(GameObject effect){effects.Add(effect);}
 public void EndSpecial(){special=null;flip=false;ShowBody(true);if(CurrentPose!=Pose.Death)ResetToIdle();foreach(var effect in effects)if(effect!=null)Destroy(effect);effects.Clear();}
 protected override void Update(){base.Update();if(flip&&SpriteTransform!=null){var scale=SpriteTransform.localScale;scale.x=-Mathf.Abs(scale.x);SpriteTransform.localScale=scale;}}
 protected override void OnDestroy(){EndSpecial();base.OnDestroy();}
}
