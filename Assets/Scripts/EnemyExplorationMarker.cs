using UnityEngine;
// Existing visible map actor only; this marker adds no stats, encounters or persistence.
public sealed class EnemyExplorationMarker:MonoBehaviour {
 DungeonPrototype owner;GhostEnemyVisual visual;Transform marker;Material material;
 public void Initialize(DungeonPrototype dungeon,GhostEnemyVisual actor){owner=dungeon;visual=actor;var obj=GameObject.CreatePrimitive(PrimitiveType.Quad);obj.name="Elite exploration marker";obj.transform.SetParent(transform,false);marker=obj.transform;marker.localScale=Vector3.one*.25f;var collider=obj.GetComponent<Collider>();if(collider!=null)Destroy(collider);var shader=Resources.Load<Shader>("Shaders/DungeonNeon");if(shader==null)shader=Shader.Find("Unlit/Color");material=new Material(shader);material.SetColor("_BaseColor",Color.magenta);if(material.HasProperty("_Color"))material.SetColor("_Color",Color.magenta);obj.GetComponent<Renderer>().sharedMaterial=material;}
 void LateUpdate(){if(marker==null)return;bool show=owner!=null&&!owner.LaboratoryBattle;marker.gameObject.SetActive(show);if(!show)return;var camera=owner.ExplorationCamera;if(camera==null)return;marker.position=transform.position+Vector3.up*((visual!=null?visual.WorldHeight:2)+.3f);marker.rotation=camera.transform.rotation*Quaternion.Euler(0,0,45);}
 void OnDestroy(){if(material!=null)Destroy(material);}
}
