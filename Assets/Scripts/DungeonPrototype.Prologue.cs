using UnityEngine;
using VirtualDungeon;
public partial class DungeonPrototype {
 IntroLine introPresentationStage;
 CorruptedDialoguePresentation introEffects;
 CorruptedEncounterPresentationSettings introAudioProfile;
 AudioClip introHum;bool introTransmission,oldIntroCleanUI;
 HelmetCRTWholeFrame introCompositor;
 bool IntroShowsAvatar {get{return IntroductionActive&&introduction.Current!=null&&introduction.Current.Speaker=="GAMERBOY"&&!string.IsNullOrEmpty(introduction.Current.Text);}}
 readonly WorldDialogueLine introCorruption=new WorldDialogueLine("???",""){corruption=DialogueCorruptionLevel.Light,characterChance=.008f,flickerStrength=.015f,recoverySeconds=.09f};
 void BeginIntroPresentation(){
  introAudioProfile=ScriptableObject.CreateInstance<CorruptedEncounterPresentationSettings>();introAudioProfile.name="Wake transmission (runtime)";introAudioProfile.ambientGain=.045f;introAudioProfile.explorationFadeSeconds=0;introAudioProfile.restoreFadeSeconds=1.5f;introAudioProfile.effectCooldown=.65f;
  introAudioProfile.light.clip=Resources.Load<AudioClip>("Audio/Temporary/CorruptionLight");introAudioProfile.light.volume=.065f;introAudioProfile.light.intensity=.025f;introAudioProfile.light.seconds=.1f;
  introAudioProfile.moderate.clip=Resources.Load<AudioClip>("Audio/Temporary/CorruptionModerate");introAudioProfile.moderate.volume=.075f;introAudioProfile.moderate.intensity=.04f;introAudioProfile.moderate.seconds=.12f;
  DungeonAudio.BeginEncounterAmbient(this,introAudioProfile); // null music clip: true initial silence
  introEffects=GetComponent<CorruptedDialoguePresentation>()??gameObject.AddComponent<CorruptedDialoguePresentation>();introEffects.Bind(null,this,view,introAudioProfile);
  introCompositor=view!=null?view.GetComponent<HelmetCRTWholeFrame>():null;if(introCompositor!=null){oldIntroCleanUI=introCompositor.CinematicCleanUI;introCompositor.CinematicCleanUI=true;}
  introTransmission=false;introPresentationStage=null;UpdateIntroPresentation();ClearMovementInput();
 }
 void BeginIntroTransmission(){
  if(introTransmission||introAudioProfile==null)return;introTransmission=true;
  const int rate=22050;var samples=new float[rate*2];for(int i=0;i<samples.Length;i++){float t=i/(float)rate;samples[i]=.16f*Mathf.Sin(2*Mathf.PI*55*t)+.035f*Mathf.Sin(2*Mathf.PI*110*t);}
  introHum=AudioClip.Create("Quiet recovery carrier",samples.Length,1,rate,false);introHum.SetData(samples,0);introAudioProfile.dialogueMusic=introHum;DungeonAudio.BeginEncounterAmbient(this,introAudioProfile);
 }
 void UpdateIntroPresentation(){
  if(!IntroductionActive)return;var stage=introduction.Stage;if(ReferenceEquals(stage,introPresentationStage))return;introPresentationStage=stage;
  if(stage.Key=="wake.dialogue.01")BeginIntroTransmission();
  if(stage.Visual==IntroVisual.Glimpse)TriggerHelmetGlitch(.14f,.3f);
  if(stage.InterferenceBefore&&introEffects!=null)introEffects.Pulse(stage.Key=="wake.disconnected"?DialogueCorruptionLevel.Moderate:DialogueCorruptionLevel.Light,1);
  if(stage.Key=="wake.disconnected"){if(introAudioProfile!=null){introAudioProfile.dialogueMusic=null;DungeonAudio.BeginEncounterAmbient(this,introAudioProfile);}}
 }
 void EndIntroPresentation(bool restoreMusic){
  if(introEffects!=null)introEffects.Unbind();if(introCompositor!=null)introCompositor.CinematicCleanUI=oldIntroCleanUI;introCompositor=null;
  if(introAudioProfile!=null){DungeonAudio.EndEncounterMusic(this,false);Destroy(introAudioProfile);introAudioProfile=null;}
  if(introHum!=null){Destroy(introHum,2f);introHum=null;}introPresentationStage=null;introTransmission=false;
 }
 void DrawIntroductionCinematic(float width,float height){
  float darkness=introduction.Darkness;if(darkness>0){GUI.color=new Color(0,0,0,darkness);GUI.DrawTexture(new Rect(0,0,width,height),Texture2D.whiteTexture);GUI.color=Color.white;}
  if(introduction.Stage!=null&&introduction.Stage.Visual==IntroVisual.Glimpse){
   // Broken horizontal sync over the real camera, fading with the same black envelope.
   float strength=1-introduction.Darkness;for(int i=0;i<6;i++){float y=((i*.173f+introduction.LineAge*.35f)%1)*height;GUI.color=new Color(0,0,0,strength*(i%2==0?.6f:.3f));GUI.DrawTexture(new Rect(0,y,width,Mathf.Max(2,height*.007f)),Texture2D.whiteTexture);}GUI.color=Color.white;
  }
  var line=introduction.Current;if(line==null||string.IsNullOrEmpty(line.Text))return;
  if(line.Visual==IntroVisual.Boot){
   var style=new GUIStyle(GUI.skin.label){fontSize=24,wordWrap=false,normal={textColor=new Color(.45f,.9f,1)}};
   float x=width*.16f,y=height*.35f;string cursor=Mathf.FloorToInt(Time.unscaledTime*2)%2==0?"_":" ";string visible=introTyping.VisibleText(Time.unscaledTime,22,false);
   GUI.Label(new Rect(x,y,width*.7f,45),line.Key=="wake.boot.1"?visible+cursor:"SYSTEM BOOT",style);
   if(line.Key!="wake.boot.1")GUI.Label(new Rect(x,y+55,width*.7f,45),line.Key=="wake.boot.2"?visible+cursor:"Ejecutando recovery.exe...",style);
   if(line.Key=="wake.boot.3")GUI.Label(new Rect(x,y+100,width*.7f,45),visible+cursor,style);
   return;
  }
  string text=introTyping.VisibleText(Time.unscaledTime,Layout.lettersPerSecond,true);
  if(line.Speaker=="???"&&line.InterferenceBefore)text=DialogueCorruptionEffects.Display(text,introCorruption,introduction.LineAge,DialogueCorruptionSettings.Current);
  if(IntroShowsAvatar)NeonDungeonUI.DialogueAvatar(Layout.Resolve(Layout.dialogueAvatar,0,height));
  NeonDungeonUI.Dialogue(Layout.Resolve(Layout.dialogue,0,height),line.Speaker,text,Layout.preservePanelAspect,introTyping.IsComplete(Time.unscaledTime,Layout.lettersPerSecond));
  var hint=new GUIStyle(GUI.skin.label){fontSize=12,alignment=TextAnchor.MiddleCenter};GUI.Label(new Rect(width/2-250,height-28,500,24),"ESPACIO / A: continuar",hint);
 }
}
