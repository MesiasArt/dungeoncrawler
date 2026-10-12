using UnityEngine;
// Four soft shadow strips extending from the terminal wall onto its adjoining surfaces.
public class DungeonExitDepth : MonoBehaviour
{
 public float cellSize=3,ceilingHeight=3;
 Material material;Mesh mesh;
 void Start() {
  var shader=Resources.Load<Shader>("Shaders/ExitDepth");if(shader==null)return;
  material=new Material(shader);mesh=new Mesh();
  var vertices=new System.Collections.Generic.List<Vector3>();var uv=new System.Collections.Generic.List<Vector2>();var triangles=new System.Collections.Generic.List<int>();
  float left=8.5f*cellSize+.015f,right=9.5f*cellSize-.015f,end=8.5f*cellSize-.03f,near=end-1.52f;
  Add(vertices,uv,triangles,new Vector3(left,.015f,end),new Vector3(right,.015f,end),new Vector3(left,.015f,near),new Vector3(right,.015f,near));
  Add(vertices,uv,triangles,new Vector3(left,ceilingHeight-.015f,end),new Vector3(right,ceilingHeight-.015f,end),new Vector3(left,ceilingHeight-.015f,near),new Vector3(right,ceilingHeight-.015f,near));
  Add(vertices,uv,triangles,new Vector3(left,0,end),new Vector3(left,ceilingHeight,end),new Vector3(left,0,near),new Vector3(left,ceilingHeight,near));
  Add(vertices,uv,triangles,new Vector3(right,0,end),new Vector3(right,ceilingHeight,end),new Vector3(right,0,near),new Vector3(right,ceilingHeight,near));
  mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();
  var obj=new GameObject("Exit recess soft edge shadows");obj.transform.SetParent(transform,false);obj.AddComponent<MeshFilter>().sharedMesh=mesh;var r=obj.AddComponent<MeshRenderer>();r.sharedMaterial=material;r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
 }
 static void Add(System.Collections.Generic.List<Vector3> v,System.Collections.Generic.List<Vector2> u,System.Collections.Generic.List<int> t,Vector3 a,Vector3 b,Vector3 c,Vector3 d) { int n=v.Count;v.Add(a);v.Add(b);v.Add(c);v.Add(d);u.Add(new Vector2(0,0));u.Add(new Vector2(1,0));u.Add(new Vector2(0,1));u.Add(new Vector2(1,1));t.AddRange(new[]{n,n+2,n+1,n+1,n+2,n+3}); }
 void OnDestroy() { if(material!=null)Destroy(material);if(mesh!=null)Destroy(mesh); }
}
