using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public partial class DungeonPrototype {
 public const int SpeedrunUnlockLevel=10,SpeedrunLevelGap=10,SpeedrunBitsCost=5;
 bool selectingSpeedrun,speedrunExecuting;int speedrunInputFrame=-1;
 public static bool SpeedrunLevelEligible(int player,int enemy){return player>=SpeedrunUnlockLevel&&enemy>=1&&player-enemy>=SpeedrunLevelGap;}
 bool SpeedrunTurnAvailable {get{return battle&&!busy&&!lost&&!pauseOpen&&!worldDialogue.Active&&!IntroductionActive&&(combatHack==null||!combatHack.Active)&&combatMenu==CombatMenu.Main&&!speedrunExecuting&&(partyPanels==null||!partyPanels.Animating);}}
 public bool SpeedrunEligible(int index){
  if(!battle||index<0||index>=enemyObjects.Count||enemyObjects[index]==null||enemyLives[index]<=0||!encounter.Contains(index)||combatSummons.Contains(index))return false;
  var stats=enemyObjects[index].GetComponent<EnemyCombatStats>();if(stats==null||!SpeedrunLevelEligible(PlayerLevel,stats.level)||stats.category!=PlacedEnemyCategory.Normal)return false;
  var profile=EnemyCharacterCatalog.Current!=null?EnemyCharacterCatalog.Current.FindId(stats.speciesId):null;
  if(profile==null||profile.category!=EnemyRank.Enemigo||profile.reservedForEvents||profile.protectedFromInstantDefeat)return false;
  var placement=index<slots.Count?slots[index].placement:null;
  // Scripted encounters are protected conservatively; their normal defeat still works unchanged.
  if(placement!=null&&(placement.category!=PlacedEnemyCategory.Normal||placement.protectedFromInstantDefeat||placement.unique||placement.trigger==EncounterTrigger.InteractiveEvent||placement.holdPosition||placement.exploration!=null&&placement.exploration.profile==EnemyExplorationProfile.Elite))return false;
  return true;
 }
 List<int> SpeedrunTargets(){var result=new List<int>();foreach(int index in encounter)if(SpeedrunEligible(index))result.Add(index);return result;}
 public bool BeginSpeedrunSelection(){
  if(!SpeedrunTurnAvailable||selectingEnemy)return false;var targets=SpeedrunTargets();if(targets.Count==0)return false;
  if(CurrentBits<SpeedrunBitsCost){SystemMessages.Show(SystemMessageCode.SpeedrunNoBits);return false;}
  pendingTargetHack=null;activeEnemy=targets[0];enemyHP=enemyLives[activeEnemy];
  if(targets.Count==1)return TryExecuteSpeedrun(activeEnemy);
  selectingSpeedrun=selectingEnemy=true;message="SPEEDRUN: elige un objetivo válido y confirma.";return true;
 }
 public bool TryExecuteSpeedrun(int index){
  if(!SpeedrunTurnAvailable)return false;
  if(!SpeedrunEligible(index)){selectingSpeedrun=selectingEnemy=false;SystemMessages.Show(SystemMessageCode.SpeedrunInvalid);return false;}
  if(CurrentBits<SpeedrunBitsCost){SystemMessages.Show(SystemMessageCode.SpeedrunNoBits);return false;}
  activeEnemy=index;selectingSpeedrun=selectingEnemy=false;pendingTargetHack=null;busy=speedrunExecuting=true;
  playerBits=CurrentBits-SpeedrunBitsCost;message="";SystemMessages.Show(SystemMessageCode.SpeedrunStart,"",0,DigitalIcon.Speedrun);
  StartCoroutine(ExecuteSpeedrun(index,enemyObjects[index]));return true;
 }
 IEnumerator ExecuteSpeedrun(int index,GameObject opponent){
  var effect=opponent.AddComponent<SpeedrunDeleteEffect>();
  try{
   yield return effect.Play(view,.5f);
   if(!battle||opponent==null||index>=enemyObjects.Count||enemyObjects[index]!=opponent)yield break;
   activeEnemy=index;SystemMessages.Show(SystemMessageCode.SpeedrunComplete,"",0,DigitalIcon.Speedrun);
   yield return ResolveCombatDamage(0,"SPEEDRUN",true);
  }finally{if(effect!=null)Destroy(effect);speedrunExecuting=false;}
 }
 bool SpeedrunInput(){var key=Keyboard.current;var pad=Gamepad.current;if(speedrunInputFrame==Time.frameCount)return false;if((key!=null&&key.rKey.wasPressedThisFrame)||(pad!=null&&pad.buttonNorth.wasPressedThisFrame)){speedrunInputFrame=Time.frameCount;BeginSpeedrunSelection();return true;}return false;}
 void DrawSpeedrunButton(float width,float height){
  if(!SpeedrunTurnAvailable||SpeedrunTargets().Count==0||selectingEnemy&&!selectingSpeedrun)return;
  var r=new Rect(width-235,height-218,215,74);var saved=GUI.color;
  NeonDungeonUI.Solid(r,new Color(.015f,.025f,.065f,.97f));NeonDungeonUI.Outline(r,NeonDungeonUI.Cyan,2);
  GUI.color=new Color(1,1,1,.75f+.25f*Mathf.Sin(Time.unscaledTime*4));GUI.DrawTexture(new Rect(r.x+10,r.y+19,31,31),DigitalIcons.Texture(DigitalIcon.Speedrun));GUI.color=saved;
  var style=new GUIStyle(GUI.skin.label){fontSize=19,alignment=TextAnchor.MiddleCenter};style.normal.textColor=NeonDungeonUI.Cyan;
  GUI.Label(new Rect(r.x+38,r.y+5,r.width-42,29),"SPEEDRUN",style);style.fontSize=12;
  GUI.Label(new Rect(r.x+5,r.y+36,r.width-10,28),CurrentBits<5?"5 BITS · FALTAN BITS":selectingSpeedrun?"5 BITS · CONFIRMAR":"5 BITS · R / Y",style);
  GUI.enabled=CurrentBits>=5;if(GUI.Button(r,GUIContent.none,GUIStyle.none)){if(selectingSpeedrun)TryExecuteSpeedrun(activeEnemy);else BeginSpeedrunSelection();}GUI.enabled=true;
 }
}
