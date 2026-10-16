using System;using System.Collections.Generic;using UnityEngine;
public enum DigitalKingdomService { Inn, Shop, Temple, Research, Travel, Sewers, Exit, DungeonGuide }
[CreateAssetMenu(menuName="Virtual Dungeon/Digital Kingdom/Ciudad")]
public class DigitalKingdomSettings:ScriptableObject {
 public string scenePath="Assets/Scenes/DigitalKingdom.unity",fallbackDungeonScene="Assets/Scenes/SampleScene.unity";
 public HelmetCRTSettings crtSettings;public DigitalKingdomStorySettings story;
 public Texture2D panorama;public DigitalKingdomLocation[] locations=new DigitalKingdomLocation[0];
 public string requiredBossId="makong.first-sub-boss";public Vector2Int fallbackReturnCell=new Vector2Int(12,65);public int fallbackReturnHeading=2;
 [Range(.05f,1)]public float transitionSeconds=.25f;
 [Header("Transicion luminosa")]
 [Range(.8f,1.2f)] public float whiteOutSeconds=1;
 [Range(.2f,2)] public float whiteInSeconds=.8f;
 public AudioClip teleportSound;
 public DigitalKingdomTexts texts=new DigitalKingdomTexts();
 public WorldDialogueLine[] firstVisit=new[]{new WorldDialogueLine("SISTEMA","Digital Kingdom descubierto."),new WorldDialogueLine("GAMERBOY","Espera... ¿hay toda una ciudad aquí?"),new WorldDialogueLine("GAMERBOY","Vale. Este juego es mucho más grande de lo que pensaba.")};
 public static DigitalKingdomSettings Current {get{return Resources.Load<DigitalKingdomSettings>("DigitalKingdom/City");}}
}
[Serializable]public class DigitalKingdomProgress {
 public bool discovered,firstVisitCompleted,inCity,templeRegistered;public string returnMap,returnScene,returnMapId,returnZone,locationId;
 public Vector2Int returnCell;public int returnHeading;
 public List<string> visitedLocations=new List<string>(),storyFlags=new List<string>(),readNotices=new List<string>();
 public List<string> unlockedLocations=new List<string>(),conversationsSeen=new List<string>();
}
