using UnityEngine;using UnityEngine.SceneManagement;
public static class DigitalKingdomTransit {
 public static DungeonSaveData CityData,ReturnData;public static DungeonMapSettings Map;public static string ArrivalNode;public static PlayerCharacterSettings Character;public static PlayerLevelSettings Levels;public static CombatInventorySettings Inventory;
 public static void Enter(DungeonSaveData data,DungeonMapSettings map,string cityScene){CityData=data;ReturnData=null;Map=map;DungeonAudio.KeepAcrossScenes();SceneManager.LoadScene(cityScene,LoadSceneMode.Single);}
 public static void Return(DungeonSaveData data,DungeonMapSettings map,string fallback){data.cell=data.city.returnCell;data.heading=data.city.returnHeading;Travel(data,map,fallback,null);}
 public static void Travel(DungeonSaveData data,DungeonMapSettings map,string fallback,string node){CityData=null;ReturnData=data;Map=DungeonMapPersistence.Resolve(data.activeMapId)??map;ArrivalNode=node;Time.timeScale=1;AudioListener.pause=false;SceneManager.LoadScene(string.IsNullOrEmpty(data.city.returnScene)?fallback:data.city.returnScene,LoadSceneMode.Single);}
 public static DungeonSaveData ConsumeReturn(){var data=ReturnData;ReturnData=null;return data;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void Reset(){CityData=ReturnData=null;Map=null;ArrivalNode=null;Character=null;Levels=null;Inventory=null;}
}
