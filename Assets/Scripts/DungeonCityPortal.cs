using UnityEngine;
// Keeps the existing ID/crossing adapter; the visible object is only a light portal.
public sealed class DungeonCityPortal:MonoBehaviour {
 DungeonPrototype owner;Mesh mesh;Material material;Renderer surface;bool returnPortal;
 public void Build(DungeonPrototype value,DungeonDoor door,float height,bool isReturn=false){owner=value;returnPortal=isReturn;var hidden=door.panel.GetComponent<Renderer>();if(hidden!=null)hidden.enabled=false;door.KeepOpenPermanently();
  var root=new GameObject("Portal blanco - luz");root.transform.SetParent(transform,false);root.transform.localPosition=Vector3.up*height*.5f-Vector3.forward*.52f;var filter=root.AddComponent<MeshFilter>();surface=root.AddComponent<MeshRenderer>();mesh=new Mesh{name="Portal ellipse"};const int sides=48;var vertices=new Vector3[sides+1];var triangles=new int[sides*6];for(int n=0;n<sides;n++){float angle=n*Mathf.PI*2/sides;vertices[n+1]=new Vector3(Mathf.Cos(angle)*door.doorwayWidth*.49f,Mathf.Sin(angle)*height*.49f,0);int a=n+1,b=(n+1)%sides+1;int t=n*6;triangles[t]=0;triangles[t+1]=a;triangles[t+2]=b;triangles[t+3]=0;triangles[t+4]=b;triangles[t+5]=a;}mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();filter.sharedMesh=mesh;material=new Material(Resources.Load<Shader>("Shaders/DungeonNeon"));material.SetColor("_BaseColor",Color.white*1.8f);surface.sharedMaterial=material;surface.enabled=(returnPortal||owner.CityPortalReady);}
 void LateUpdate(){if(surface!=null)surface.enabled=owner!=null&&(returnPortal||owner.CityPortalReady);}
 void OnDestroy(){if(mesh!=null)Destroy(mesh);if(material!=null)Destroy(material);}
}
