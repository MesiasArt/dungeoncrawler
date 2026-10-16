using UnityEngine;
// Decorative only: no colliders, enemies, events or save records.
public sealed class RetroFoliageSway:MonoBehaviour {
 public DungeonPrototype owner;public float phase;Vector3 rest;Quaternion rotation;
 void Start(){rest=transform.localPosition;rotation=transform.localRotation;}
 void LateUpdate(){if(owner==null||!owner.RetroExplorationAvailable)return;float wave=Mathf.Sin(Time.time*1.9f+phase);transform.localPosition=rest+transform.right*(wave*.012f);transform.localRotation=rotation*Quaternion.Euler(0,0,wave*.65f);}
}
