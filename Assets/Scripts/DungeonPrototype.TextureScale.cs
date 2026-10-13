using UnityEngine;
public partial class DungeonPrototype {
 float appliedWallGridScale=-1,appliedFloorGridScale=-1,appliedCeilingTextureScale=-1;
 void LateUpdate(){ApplyNeonTextureScales();TickPartyPanel();TickConversationCamera();}
 void ApplyNeonTextureScales(){if(wall==null||floor==null||ceiling==null)return;var visual=DungeonVisualSettings.Current;float w=visual!=null?Mathf.Clamp(visual.wallGridScale,.25f,6):1,f=visual!=null?Mathf.Clamp(visual.floorGridScale,.25f,6):1,c=visual!=null?Mathf.Clamp(visual.ceilingTextureScale,.25f,6):1;
  if(w!=appliedWallGridScale){wall.SetFloat("_FloorGridSize",Cell/9f*w);appliedWallGridScale=w;}
  if(f!=appliedFloorGridScale){floor.SetFloat("_FloorGridSize",Cell/9f*f);appliedFloorGridScale=f;}
  if(c!=appliedCeilingTextureScale){ceiling.SetTextureScale("_BaseMap",Vector2.one/c);appliedCeilingTextureScale=c;}
 }
}
