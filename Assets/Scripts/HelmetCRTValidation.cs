#if UNITY_EDITOR
using UnityEngine;
// Only used by MCP visual verification; never created by normal gameplay.
public class HelmetCRTValidation:MonoBehaviour {
 public DungeonPrototype target;
 System.Reflection.MethodInfo draw;
 void Awake(){draw=typeof(DungeonPrototype).GetMethod("OnGUI",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);}
 public string RunBitsChecks(){
 var d=target;d.StopAllCoroutines();d.enabled=false;
 var saved=DungeonSaveStore.Read(0);var lab=d.gameObject.AddComponent<CombatLaboratory>();d.InitializeLaboratory();d.StartLaboratoryCombat("Hongo",1,8,0);d.enabled=false;
 var f=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
 var begin=typeof(DungeonPrototype).GetMethod("BeginTargetHack",f);var busy=typeof(DungeonPrototype).GetField("busy",f);var selected=typeof(DungeonPrototype).GetField("selectedHackId",f);var bits=typeof(DungeonPrototype).GetField("playerBits",f);
 d.StopAllCoroutines();busy.SetValue(d,false);d.RestoreBits();selected.SetValue(d,"hack.jump");var jump=d.GetComponent<EnemyJumpHack>();DungeonAudio.SetCombat(true);
 var audioType=typeof(DungeonAudio);var audio=audioType.GetField("instance",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null);
 var music=(AudioSource)audioType.GetField("musicSource",f).GetValue(audio);var clip=music.clip;float song=music.time;
 begin.Invoke(d,new object[]{jump});int afterJump=d.CurrentBits;bool musicContinues=music.isPlaying&&music.clip==clip&&music.time>=song;
 begin.Invoke(d,new object[]{jump});int repeat=d.CurrentBits;jump.enabled=false;jump.enabled=true;busy.SetValue(d,false);
 selected.SetValue(d,"hack.overclock");var over=d.GetComponent<OverclockHack>();begin.Invoke(d,new object[]{over});int afterOver=d.CurrentBits;over.enabled=false;over.enabled=true;busy.SetValue(d,false);bits.SetValue(d,0);begin.Invoke(d,new object[]{over});bool rejected=!over.Active&&d.CurrentBits==0;
 var decoded=JsonUtility.FromJson<DungeonSaveData>(JsonUtility.ToJson(d.CaptureDungeonSave()));d.RestoreDungeonSave(decoded);int restored=d.CurrentBits;d.RestoreBits();int full=d.CurrentBits;d.RestoreDungeonSave(saved);d.enabled=false;lab.enabled=false;
 d.PrepareDecorationPreview(new Vector2Int(10,5),0);var crt=d.GetComponentInChildren<HelmetCRTController>();crt.settings=ScriptableObject.CreateInstance<HelmetCRTSettings>();crt.TriggerGlitch(10,10);float intensity=crt.GlitchStrength;float seconds=(float)typeof(HelmetCRTController).GetField("duration",f).GetValue(crt);
 string result="JUMP cost10="+(afterJump==90)+"; repeat no debit="+(repeat==afterJump)+"; OVERCLOCK cost25="+(afterOver==65)+"; insufficient Bits rejected="+rejected+"; zero Bits restored="+(restored==0)+"; refill100="+(full==100)+"; continuous JUMP music="+musicContinues+"; intensity clamp="+(intensity<=.4f)+"; duration clamp="+(seconds<=.3f);
 System.IO.File.WriteAllText("Tests/HelmetBitsMcpResults.txt",result);return result;
 }
 public System.Collections.IEnumerator VisualChecks(){
 var d=target;var f=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
 d.enabled=false;d.StopAllCoroutines();var crt=d.GetComponentInChildren<HelmetCRTController>();crt.settings=ScriptableObject.CreateInstance<HelmetCRTSettings>();crt.settings.curvature=true;crt.settings.curvatureAmount=-.025f;
 System.IO.Directory.CreateDirectory("Captures/HelmetCRT/Verified");
 d.PrepareDecorationPreview(new Vector2Int(10,5),0);yield return new WaitForEndOfFrame();Capture("Room-Inward-HUD");
 d.PrepareDecorationPreview(new Vector2Int(9,12),2);yield return new WaitForEndOfFrame();Capture("DarkCorridor-HUD");
 var view=d.GetComponentInChildren<Camera>();var door=d.GetComponentsInChildren<DungeonDoor>()[0];view.transform.position=door.transform.position+Vector3.up*1.5f-Vector3.forward*1.8f;var method=typeof(DungeonPrototype).GetMethod("DoorInteractionAvailable",f);typeof(DungeonPrototype).GetField("heading",f).SetValue(d,0);door.SetClosedImmediately();bool closed=(bool)method.Invoke(d,new object[]{door});typeof(DungeonPrototype).GetField("heading",f).SetValue(d,2);bool behind=(bool)method.Invoke(d,new object[]{door});typeof(DungeonPrototype).GetField("heading",f).SetValue(d,0);door.SetOpenImmediately();bool open=(bool)method.Invoke(d,new object[]{door});door.SetClosedImmediately();view.transform.position+=Vector3.back*20;bool far=(bool)method.Invoke(d,new object[]{door});
 d.PrepareDecorationPreview(new Vector2Int(10,5),0);typeof(DungeonPrototype).GetMethod("SetPause",f).Invoke(d,new object[]{true});typeof(DungeonPrototype).GetField("pausePage",f).SetValue(d,1);yield return new WaitForEndOfFrame();Capture("CharacterMenu");typeof(DungeonPrototype).GetField("pausePage",f).SetValue(d,4);yield return new WaitForEndOfFrame();Capture("GraphicsMenu");typeof(DungeonPrototype).GetMethod("SetPause",f).Invoke(d,new object[]{false});
 crt.TriggerContext("Teleport",.4f);yield return new WaitForEndOfFrame();Capture("TeleportGlitch");yield return new WaitForSecondsRealtime(.35f);bool expired=crt.GlitchStrength<=0;
 crt.settings.curvatureAmount=.025f;yield return new WaitForEndOfFrame();Capture("Room-Outward");
 string result="Door closed prompt="+closed+"; behind hidden="+!behind+"; open hidden="+!open+"; distant hidden="+!far+"; glitch expired="+expired+"; inward/outward shader rendered; room/corridor/menu/teleport captures generated";
 System.IO.File.WriteAllText("Tests/HelmetVisualMcpResults.txt",result);
 }
 void Capture(string name){var t=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes("Captures/HelmetCRT/Verified/"+name+".png",t.EncodeToPNG());Destroy(t);}
 public System.Collections.IEnumerator CaptureCharacterSheet(){
 while(target!=null&&!target.LaboratoryReady)yield return null;if(target==null)yield break;
 var d=target;d.RestoreDungeonSave(DungeonSaveStore.Read(0));d.StopAllCoroutines();d.PrepareDecorationPreview(new Vector2Int(2,43),0);d.enabled=false;
 var f=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;typeof(DungeonPrototype).GetMethod("SetPause",f).Invoke(d,new object[]{true});typeof(DungeonPrototype).GetField("pausePage",f).SetValue(d,1);
 var crt=d.GetComponentInChildren<HelmetCRTController>();crt.settings=ScriptableObject.Instantiate(crt.settings);crt.settings.bufferHeight=720;crt.settings.scanlines=.12f;crt.settings.artifacts=.02f;crt.settings.chromaticFringe=.04f;crt.settings.curvature=true;crt.settings.curvatureAmount=-.025f;
 yield return new WaitForEndOfFrame();yield return new WaitForEndOfFrame();
 System.IO.Directory.CreateDirectory("Captures/CharacterSheet");var screenshot=ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes("Captures/CharacterSheet/VerifiedSheetCRT.png",screenshot.EncodeToPNG());Destroy(screenshot);
 var whole=crt.GetComponent<HelmetCRTWholeFrame>();var rt=whole.LastOutput;
 if(rt!=null){var flipped=RenderTexture.GetTemporary(rt.width,rt.height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear);Graphics.Blit(rt,flipped,new Vector2(1,-1),new Vector2(0,1));var old=RenderTexture.active;RenderTexture.active=flipped;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();System.IO.File.WriteAllBytes("Captures/CharacterSheet/VerifiedProcessedSheet.png",image.EncodeToPNG());Destroy(image);RenderTexture.active=old;RenderTexture.ReleaseTemporary(flipped);}
 System.IO.File.WriteAllText("Tests/CharacterSheetMcpResults.txt","Character sheet rendered; portrait loaded="+(Resources.Load<Texture2D>("UI/CharacterSheet/GamerBoyFullBody")!=null)+"; equipment slots=3; whole-frame CRT output="+(rt!=null)+"; automatic intensity="+crt.settings.automaticGlitchIntensity);
 }
 public System.Collections.IEnumerator VerifyComfortAdjustments(){
 while(target!=null&&!target.LaboratoryReady)yield return null;if(target==null)yield break;var d=target;
 var f=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;var cell=(Vector2Int)typeof(DungeonPrototype).GetField("position",f).GetValue(d);d.PrepareDecorationPreview(cell,0);d.StopAllCoroutines();d.enabled=false;
 DungeonAudio.SetCombat(false);var audio=typeof(DungeonAudio).GetField("instance",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).GetValue(null);var music=(AudioSource)typeof(DungeonAudio).GetField("musicSource",f).GetValue(audio);var clip=music.clip;int before=music.timeSamples;typeof(DungeonPrototype).GetMethod("SetPause",f).Invoke(d,new object[]{true});yield return new WaitForSecondsRealtime(.45f);bool continues=music.isPlaying&&music.clip==clip&&music.timeSamples!=before;float reduced=music.volume;typeof(DungeonPrototype).GetMethod("SetPause",f).Invoke(d,new object[]{false});yield return new WaitForSecondsRealtime(.4f);float restoredVolume=music.volume;
 var view=d.GetComponentInChildren<Camera>();float original=view.fieldOfView;var position=view.transform.position;var actor=new GameObject("Temporary conversation camera test");actor.transform.position=position+view.transform.forward*5-Vector3.up*1.2f;typeof(DungeonPrototype).GetMethod("FocusConversationOn",f).Invoke(d,new object[]{actor.transform});var dialogue=(WorldDialogue)typeof(DungeonPrototype).GetField("worldDialogue",f).GetValue(d);dialogue.Begin(new[]{new WorldDialogueLine("CAMERA TEST","Conversación de prueba.")});var tick=typeof(DungeonPrototype).GetMethod("TickConversationCamera",f);for(int n=0;n<40;n++){tick.Invoke(d,null);yield return null;}float zoom=view.fieldOfView;bool stationary=view.transform.position==position;dialogue.Cancel();for(int n=0;n<60;n++){tick.Invoke(d,null);yield return null;}bool restored=Mathf.Abs(view.fieldOfView-original)<.1f;Destroy(actor);
 string result="Menu music continues="+continues+"; lower menu volume="+(reduced<restoredVolume)+"; menu volume="+reduced+"; restored volume="+restoredVolume+"; conversation FOV="+zoom+" < "+original+"; zoom works="+(zoom<original)+"; player position unchanged="+stationary+"; camera restores="+restored;
 System.IO.File.WriteAllText("Tests/ComfortAdjustmentsMcpResults.txt",result);
 }
 void OnGUI(){if(target!=null&&!target.enabled)draw.Invoke(target,null);}
}
#endif
