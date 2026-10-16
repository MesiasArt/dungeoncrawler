using System;
using UnityEngine;
public enum SystemMessageCode { Received,HealPatch,HealRepair,HealFull,Denied,DeniedRed,DeniedGreen,DeniedRoot,Granted,GrantedRoot,BossWarning,Saved,Transfer,LevelUp,CacheOpened,CacheEmpty,HackInitialized,HackComplete,HackSuccess,HackPerfect,HackFailed,HackInterrupted,Status,Victory,SpeedrunStart,SpeedrunComplete,SpeedrunNoBits,SpeedrunInvalid,CheatLevelUp }
[Serializable] public class SystemMessageOverride {public SystemMessageCode code;[TextArea(2,5)]public string text;}
[CreateAssetMenu(menuName="Codebound/System Messages")]
public sealed class SystemMessageSettings:ScriptableObject {
 public bool enabledMessages=true,sounds=true;
 [Range(.6f,4)]public float noticeSeconds=1.7f,hackSeconds=1.25f,warningSeconds=2.2f;
 [Range(40,300)]public float charactersPerSecond=160;
 public SystemMessageOverride[] messages=new SystemMessageOverride[0];
 public static SystemMessageSettings Current {get{return Resources.Load<SystemMessageSettings>("UI/SystemMessageSettings");}}
 public string Text(SystemMessageCode code){foreach(var row in messages??new SystemMessageOverride[0])if(row!=null&&row.code==code&&!string.IsNullOrWhiteSpace(row.text))return row.text;return DefaultText(code);}
 public static string DefaultText(SystemMessageCode code){switch(code){
 case SystemMessageCode.SpeedrunStart:return "SPEEDRUN.EXE\nWEAK TARGET CONFIRMED_";
 case SystemMessageCode.SpeedrunComplete:return "INSTANT CLEAR!\nTARGET DELETED_";
 case SystemMessageCode.SpeedrunNoBits:return "INSUFFICIENT BITS\nEXECUTION CANCELLED_";
 case SystemMessageCode.SpeedrunInvalid:return "ACCESS DENIED\nINVALID TARGET_";
 case SystemMessageCode.CheatLevelUp:return "CHEAT CODE ACCEPTED\nLEVEL UP!\nSYSTEM PARAMETERS UPDATED_";
 case SystemMessageCode.Status:return "{name}";
 case SystemMessageCode.Victory:return "HOSTILE PROCESS TERMINATED\nCOMBAT COMPLETE\nEXPLORATION RESUMED_";
 case SystemMessageCode.Received:return "DATA RECEIVED\n+{amount} {name}\nINVENTORY UPDATED_";
 case SystemMessageCode.HealPatch:return "HP PATCH INSTALLED\n+{amount} HP RESTORED\nSYSTEM STATUS: STABLE_";
 case SystemMessageCode.HealRepair:return "SYSTEM REPAIR COMPLETE\n+{amount} HP RESTORED\nINTEGRITY RECOVERED_";
 case SystemMessageCode.HealFull:return "FULL RESTORE COMPLETE\n+{amount} HP RESTORED\nRECOVERY APPLIED_";
 case SystemMessageCode.Denied:return "ACCESS DENIED\nVALID TOKEN REQUIRED_";
 case SystemMessageCode.DeniedRed:return "ACCESS DENIED\nRED ACCESS TOKEN REQUIRED_";
 case SystemMessageCode.DeniedGreen:return "ACCESS DENIED\nGREEN ACCESS TOKEN REQUIRED_";
 case SystemMessageCode.DeniedRoot:return "WARNING\nRESTRICTED AREA\nROOT KEY REQUIRED_";
 case SystemMessageCode.Granted:return "ACCESS TOKEN VERIFIED\nSECURITY LOCK DISABLED\nACCESS GRANTED_";
 case SystemMessageCode.GrantedRoot:return "ROOT KEY VERIFIED\nROOT ACCESS GRANTED\nSECURITY PROTOCOL DISABLED_";
 case SystemMessageCode.BossWarning:return "WARNING\nHOSTILE ENTITY DETECTED\nPROCEED WITH CAUTION_";
 case SystemMessageCode.Saved:return "RESTORE POINT DETECTED\nSYSTEM BACKUP COMPLETE\nDATA SAVED_";
 case SystemMessageCode.Transfer:return "DESTINATION VERIFIED\nTRANSFER INITIALIZED\nRELOCATING DATA_";
 case SystemMessageCode.LevelUp:return "SYSTEM UPGRADE COMPLETE\nLEVEL INCREASED\nPARAMETERS UPDATED_";
 case SystemMessageCode.CacheOpened:return "DATA CACHE OPENED\nCONTENTS RECOVERED_";
 case SystemMessageCode.CacheEmpty:return "DATA CACHE EMPTY\nNO FILES DETECTED_";
 case SystemMessageCode.HackInitialized:return "HACK INITIALIZED\n> {name}.EXE\nEXECUTING_";
 case SystemMessageCode.HackSuccess:return "HACK COMPLETE\nEXECUTION SUCCESSFUL_";
 case SystemMessageCode.HackPerfect:return "PERFECT EXECUTION\nMAXIMUM OUTPUT_";
 case SystemMessageCode.HackFailed:return "EXECUTION FAILED\nPROCESS TERMINATED_";
 case SystemMessageCode.HackInterrupted:return "PROCESS INTERRUPTED\nEXECUTION ABORTED_";
 default:return "HACK COMPLETE\nEXECUTION FINISHED_";
 }}
}
