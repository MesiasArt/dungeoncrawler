using System;using System.Collections.Generic;using UnityEngine;
public enum DigitalKingdomService { Inn, Shop, Temple, Research, Travel, Sewers, Exit }
[CreateAssetMenu(menuName="Virtual Dungeon/Digital Kingdom/Ciudad")]
public class DigitalKingdomSettings:ScriptableObject {
 public string scenePath="Assets/Scenes/DigitalKingdom.unity",fallbackDungeonScene="Assets/Scenes/SampleScene.unity";
 public Texture2D panorama;public DigitalKingdomLocation[] locations=new DigitalKingdomLocation[0];
 public string requiredBossId="makong.first-sub-boss";public Vector2Int fallbackReturnCell=new Vector2Int(12,65);public int fallbackReturnHeading=2;
 [Range(.05f,1)]public float transitionSeconds=.25f;
 public WorldDialogueLine[] firstVisit=new[]{new WorldDialogueLine("SYSTEM","Digital Kingdom discovered."),new WorldDialogueLine("GAMERBOY","Wait... there's an entire city in here?"),new WorldDialogueLine("GAMERBOY","Okay. This game is way bigger than I thought.")};
 public static DigitalKingdomSettings Current {get{return Resources.Load<DigitalKingdomSettings>("DigitalKingdom/City");}}
}
[Serializable]public class DigitalKingdomProgress {
 public bool discovered,firstVisitCompleted,inCity,templeRegistered;public string returnMap,returnScene,returnMapId,returnZone,locationId;
 public Vector2Int returnCell;public int returnHeading;
 public List<string> unlockedLocations=new List<string>(),conversationsSeen=new List<string>();
}
