using UnityEngine;
public sealed class DigitalKingdomController:MonoBehaviour {
 public DungeonPrototype stateHost;public Camera cityCamera;
 void Awake(){if(stateHost==null)stateHost=GetComponent<DungeonPrototype>();stateHost.enabled=false;var data=DigitalKingdomTransit.CityData;if(data==null)data=new DungeonSaveData{introductionCompleted=true,city=new DigitalKingdomProgress{inCity=true,discovered=true,firstVisitCompleted=true},player=new PlayerProgressionRecord{currentHP=30,hasBits=true,currentBits=100},refuge=new RefugeProgress()};stateHost.InitializeCityModel(data,cityCamera);}
 void Update(){stateHost.TickCityModel();}
 void OnGUI(){stateHost.RenderCityModel();}
}
