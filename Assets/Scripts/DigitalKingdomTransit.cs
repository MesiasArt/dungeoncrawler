using UnityEngine;using UnityEngine.SceneManagement;
public static class DigitalKingdomTransit {
 public static bool FirstKingdomVisit;public static DungeonSaveData CityData,ReturnData;public static DungeonMapSettings Map;public static string ArrivalNode;public static PlayerCharacterSettings Character;public static PlayerLevelSettings Levels;public static CombatInventorySettings Inventory;
 public static void Enter(DungeonSaveData data,DungeonMapSettings map,string cityScene,DigitalKingdomSettings presentation=null){if(!DungeonWhiteTransition.CanEnter)return;CityData=data;ReturnData=null;Map=map;DungeonAudio.KeepAcrossScenes();DungeonWhiteTransition.Enter(cityScene,presentation??DigitalKingdomSettings.Current);}
 public static void Return(DungeonSaveData data,DungeonMapSettings map,string fallback){data.cell=data.city.returnCell;data.heading=data.city.returnHeading;Travel(data,map,fallback,null);}
 public static void Travel(DungeonSaveData data,DungeonMapSettings map,string fallback,string node){CityData=null;Map=DungeonMapPersistence.Resolve(data.activeMapId)??map;DungeonMapPersistence.ResetExplorationOnReentry(data,Map);ReturnData=data;ArrivalNode=node;Time.timeScale=1;AudioListener.pause=false;SceneManager.LoadScene(string.IsNullOrEmpty(data.city.returnScene)?fallback:data.city.returnScene,LoadSceneMode.Single);}
 public static void Explore(DungeonSaveData data,DungeonMapSettings map,string scene,DigitalKingdomSettings presentation,bool first=false){if(!DungeonWhiteTransition.CanEnter)return;FirstKingdomVisit=first;CityData=null;ReturnData=data;Map=map;ArrivalNode=null;DungeonAudio.KeepAcrossScenes();Time.timeScale=1;AudioListener.pause=false;DungeonWhiteTransition.Enter(scene,presentation,true);}
 internal static void CancelPendingTransition(){FirstKingdomVisit=false;CityData=ReturnData=null;Map=null;ArrivalNode=null;}
 public static DungeonSaveData ConsumeReturn(){var data=ReturnData;ReturnData=null;return data;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){FirstKingdomVisit=false;CityData=ReturnData=null;Map=null;ArrivalNode=null;Character=null;Levels=null;Inventory=null;}
}
