using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
// Only visible 3D enemies are masked. UI is captured as its own complete image.
public sealed class HelmetCRTLayerMask:MonoBehaviour {
 struct Entry {public Renderer renderer;public bool enemy;}
 readonly List<Entry> renderers=new List<Entry>();
 readonly List<Material> materialSlots=new List<Material>(4);
 readonly Plane[] planes=new Plane[6];
 RenderTexture mask;Material material;float nextRefresh;
 public static void BeginGUI(){HelmetCRTWholeFrame.CaptureGUI();}
 void Refresh(){
  renderers.Clear();foreach(var r in FindObjectsByType<Renderer>()){
   if(!(r is MeshRenderer)&&!(r is SkinnedMeshRenderer)&&!(r is SpriteRenderer))continue;
   renderers.Add(new Entry{renderer=r,enemy=r.GetComponentInParent<EnemyCombatStats>()!=null});
  }
  nextRefresh=Time.unscaledTime+.25f;
 }
 public RenderTexture Build(Camera camera,int width,int height){
  if(mask==null||mask.width!=width||mask.height!=height){Release();mask=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear){name="CRT visible enemy mask",filterMode=FilterMode.Point,hideFlags=HideFlags.HideAndDontSave};mask.Create();}
  if(material==null)material=new Material(Resources.Load<Shader>("HelmetCRT/LayerMask")){hideFlags=HideFlags.HideAndDontSave};
  material.SetInt("_CRTMaskZTest",(int)(SystemInfo.usesReversedZBuffer?CompareFunction.GreaterEqual:CompareFunction.LessEqual));
  if(Time.unscaledTime>=nextRefresh)Refresh();
  GeometryUtility.CalculateFrustumPlanes(camera,planes);
  var cmd=CommandBufferPool.Get("CRT visible layer mask");
  cmd.SetRenderTarget(mask);cmd.ClearRenderTarget(true,true,Color.black,SystemInfo.usesReversedZBuffer?0:1);
  cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix,GL.GetGPUProjectionMatrix(camera.projectionMatrix,false));
  foreach(var item in renderers){var r=item.renderer;if(r==null||!r.enabled||!r.gameObject.activeInHierarchy||(camera.cullingMask&(1<<r.gameObject.layer))==0||!GeometryUtility.TestPlanesAABB(planes,r.bounds))continue;
   materialSlots.Clear();r.GetSharedMaterials(materialSlots);
   for(int i=0;i<materialSlots.Count;i++){
    var original=materialSlots[i];if(original==null)continue;
    // Transparent decorative glows do not occlude enemies in the mask.
    bool cutout=original.IsKeywordEnabled("_ALPHATEST_ON")||original.renderQueue>=2450||r is SpriteRenderer;
    if(!item.enemy&&original.renderQueue>=3000)continue;
    Texture texture=Texture2D.whiteTexture;
    if(original.HasProperty("_BaseMap"))texture=original.GetTexture("_BaseMap")??texture;
    else if(original.HasProperty("_MainTex"))texture=original.GetTexture("_MainTex")??texture;
    var sprite=r as SpriteRenderer;if(sprite!=null&&sprite.sprite!=null)texture=sprite.sprite.texture;
    cmd.SetGlobalTexture("_CRTMaskTexture",texture);cmd.SetGlobalFloat("_CRTMaskCutout",cutout?1:0);
    Vector2 scale=Vector2.one,offset=Vector2.zero;
    if(original.HasProperty("_BaseMap")){scale=original.GetTextureScale("_BaseMap");offset=original.GetTextureOffset("_BaseMap");}
    else if(original.HasProperty("_MainTex")){scale=original.GetTextureScale("_MainTex");offset=original.GetTextureOffset("_MainTex");}
    cmd.SetGlobalVector("_CRTMaskST",new Vector4(scale.x,scale.y,offset.x,offset.y));
    cmd.SetGlobalColor("_CRTMaskColor",item.enemy?Color.red:Color.black);cmd.DrawRenderer(r,material,i);
   }
  }
  Graphics.ExecuteCommandBuffer(cmd);CommandBufferPool.Release(cmd);
  return mask;
 }
 void Release(){if(mask!=null){if(RenderTexture.active==mask)RenderTexture.active=null;mask.Release();if(Application.isPlaying)Destroy(mask);else DestroyImmediate(mask);mask=null;}}
 void OnDestroy(){Release();if(material!=null){if(Application.isPlaying)Destroy(material);else DestroyImmediate(material);}}
}
