using System.Collections;using UnityEngine;
public partial class DungeonPrototype {
 bool makongMusicTransition;CorruptedDialoguePresentation makongPresentation;
 GameObject pendingMakongNarrative;string pendingMakongNarrativeId;
 CorruptedEncounterPresentationSettings MakongPresentationSettings {get{return MakongSpecialSettings.Current.presentation;}}
 void BeginMakongAmbient(bool defeat=false){var profile=MakongPresentationSettings;if(profile==null)return;DungeonAudio.BeginEncounterAmbient(this,profile,defeat);if(makongPresentation==null)makongPresentation=GetComponent<CorruptedDialoguePresentation>()??gameObject.AddComponent<CorruptedDialoguePresentation>();makongPresentation.Bind(worldDialogue,this,view,profile,defeat);}
 void BeginMakongBattleMusic(int primary,System.Collections.Generic.IList<int> group){if(makongPresentation!=null)makongPresentation.Unbind();bool boss=enemyObjects[primary]!=null&&enemyObjects[primary].GetComponent<MakongEnemyVisual>()!=null;if(!boss&&group!=null)foreach(var member in group)if(member>=0&&member<enemyObjects.Count&&enemyObjects[member]!=null&&enemyObjects[member].GetComponent<MakongEnemyVisual>()!=null)boss=true;if(boss&&MakongPresentationSettings!=null)DungeonAudio.BeginEncounterBoss(this,MakongPresentationSettings);else DungeonAudio.EndEncounterMusic(this,true);}
 void EndMakongPresentation(){makongMusicTransition=false;if(makongPresentation!=null)makongPresentation.Unbind();DungeonAudio.EndEncounterMusic(this,false);}
 IEnumerator BeginMakongBattleAfterPause(int index,System.Collections.Generic.IList<int> group){makongMusicTransition=true;float pause=MakongPresentationSettings!=null?MakongPresentationSettings.dramaticPauseSeconds:0;if(pause>0)yield return new WaitForSecondsRealtime(pause);makongMusicTransition=false;BeginEncounterCombat(index,group);}
 bool MakongNarrativeAllowed {get{return MakongSpecialSettings.Current.narrativeEnabled&&GetComponent<CombatLaboratory>()==null;}}
 string MakongStoryId(GameObject actor,string phase){var stats=actor!=null?actor.GetComponent<EnemyCombatStats>():null;return "boss.makong."+phase+"/"+(stats!=null&&!string.IsNullOrEmpty(stats.placementId)?stats.placementId:"makong");}
 bool MakongIntroductionSeen(int index){return interactionProgress.completedEvents.Contains(MakongStoryId(enemyObjects[index],"introduction"));}
 void FinishMakongIntroduction(int index,System.Collections.Generic.IList<int> group){if(index<0||index>=enemyObjects.Count||enemyObjects[index]==null)return;var id=MakongStoryId(enemyObjects[index],"introduction");if(!interactionProgress.completedEvents.Contains(id))interactionProgress.completedEvents.Add(id);if(!makongMusicTransition)StartCoroutine(BeginMakongBattleAfterPause(index,group));}
 bool HoldMakongNarrative(GameObject corpse){if(!MakongNarrativeAllowed||corpse==null||corpse.GetComponent<MakongEnemyVisual>()==null||MakongSpecialSettings.Current.defeatDialogue.Length==0)return false;var id=MakongStoryId(corpse,"defeat");if(interactionProgress.completedEvents.Contains(id))return false;pendingMakongNarrative=corpse;pendingMakongNarrativeId=id;fadingEnemies.Add(corpse);return true;}
 IEnumerator FinishMakongBeforeFlee(){yield return FinishMakongNarrative();FinishSuccessfulFlee();}
 void FinishSuccessfulFlee(){EndMakongPresentation();if(enemyBarks!=null)enemyBarks.ClearDisplay();DirectorCombatEnded();RestoreEncounter();battle=false;busy=false;combatMenu=CombatMenu.Main;selectedCombatAction=0;activeEnemy=-1;glitch=0;StartCoroutine(RestoreBattleCamera());message="Escapaste. Retrocede o busca otra ruta.";}
 IEnumerator FinishMakongNarrative(){
  var corpse=pendingMakongNarrative;if(corpse==null)yield break;pendingMakongNarrative=null;var id=pendingMakongNarrativeId;pendingMakongNarrativeId=null;
  ClearMovementInput();if(enemyBarks!=null)enemyBarks.ClearDisplay();
  BeginMakongAmbient(true);worldDialogue.Begin(MakongSpecialSettings.Current.defeatDialogue);
  while(worldDialogue.Active&&corpse!=null)yield return null;
  if(corpse==null)yield break;
  if(makongPresentation!=null)makongPresentation.Disappearance();yield return DisappearDefeatedEnemy(corpse);
  worldDialogue.Begin(MakongSpecialSettings.Current.defeatEpilogue);
  while(worldDialogue.Active)yield return null;
  if(!interactionProgress.completedEvents.Contains(id))interactionProgress.completedEvents.Add(id);EndMakongPresentation();
 }
}
