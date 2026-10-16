using UnityEngine;
public partial class DungeonPrototype {
 float previousHelmetGlitch;int previousHelmetHP=-1;string previousHelmetRegion;
 void UpdateHelmetContext(){
  if(view==null)return;var crt=view.GetComponent<HelmetCRTController>();if(crt==null)return;crt.SetCombatContext(battle&&!lost&&!levelComplete);
  if(glitch>previousHelmetGlitch+.05f)crt.TriggerContext("Memory / encounter",.22f);
  if(previousHelmetHP>=0&&previousHelmetHP-hp>=Mathf.Max(1,MaximumHP/4))crt.TriggerContext("Heavy damage",.4f);
  var region=RegionAt(position);string id=region!=null?region.id:null;
  if(previousHelmetRegion!=null&&id!=previousHelmetRegion&&ActiveMap!=null&&ActiveMap.decorations!=null){foreach(var rule in ActiveMap.decorations.zones)if(rule.zoneId==id&&rule.theme==DecorationTheme.Corrupt){crt.TriggerContext("Corrupt zone",.3f);break;}}
  previousHelmetGlitch=glitch;previousHelmetHP=hp;previousHelmetRegion=id;
 }
 public void TriggerHelmetGlitch(float intensity=.3f,float seconds=.18f){if(view!=null){var crt=view.GetComponent<HelmetCRTController>();if(crt!=null)crt.TriggerGlitch(intensity,seconds);}}
}
