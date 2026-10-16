using UnityEngine;
public sealed class DigitalKingdomController:MonoBehaviour {
 public bool Ready {get;private set;}
 public DungeonPrototype stateHost;public Camera cityCamera;
 void Awake(){if(stateHost==null)stateHost=GetComponent<DungeonPrototype>();stateHost.enabled=false;var data=DigitalKingdomTransit.CityData;if(data==null)data=new DungeonSaveData{introductionCompleted=true,city=new DigitalKingdomProgress{inCity=true,discovered=true,firstVisitCompleted=true},player=new PlayerProgressionRecord{currentHP=30,hasBits=true,currentBits=100},refuge=new RefugeProgress()};stateHost.InitializeCityModel(data,cityCamera);var crt=cityCamera.GetComponent<HelmetCRTController>()??cityCamera.gameObject.AddComponent<HelmetCRTController>();if(DigitalKingdomSettings.Current.crtSettings!=null)crt.settings=DigitalKingdomSettings.Current.crtSettings;
 Ready=cityCamera!=null&&DigitalKingdomSettings.Current!=null&&DigitalKingdomSettings.Current.panorama!=null;}
 void Update(){stateHost.TickCityModel();}
 void OnGUI(){stateHost.RenderCityModel();}
}
