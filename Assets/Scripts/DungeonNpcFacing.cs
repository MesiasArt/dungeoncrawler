using UnityEngine;
// NPC sprites follow the player camera even during movement, dialogue and pause.
public sealed class DungeonNpcFacing:MonoBehaviour {
 public Camera viewCamera;TextMesh[] labels;
 void Start(){labels=GetComponentsInChildren<TextMesh>();}
 void LateUpdate(){if(viewCamera==null)return;transform.rotation=Quaternion.Euler(0,viewCamera.transform.eulerAngles.y,0);if(labels!=null)foreach(var label in labels)if(label!=null)label.transform.rotation=viewCamera.transform.rotation;}
}
