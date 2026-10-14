using UnityEngine;
// Input remains in DungeonPrototype. This component is an ID-based adapter, not another controller.
public sealed class DungeonSwitch:MonoBehaviour {
 public string id;public Vector2Int cell;public bool initiallyActive;public DungeonInteractionSettings interaction;public DungeonPrototype owner;
 public bool Active {get{return owner!=null&&owner.IsDungeonSwitchActive(id);}}
 public bool Trigger(){return owner!=null&&owner.ActivateDungeonSwitch(this);}
 void OnDrawGizmosSelected(){Gizmos.color=Color.green;Gizmos.DrawWireCube(transform.position+Vector3.up*.5f,Vector3.one*.5f);}
}
