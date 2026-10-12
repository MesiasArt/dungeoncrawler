using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// The dungeon is stationary during a hack. Capture it once instead of rendering
// the world and two planar reflections behind the wrist screen every frame.
public class HackDungeonBackdrop : MonoBehaviour
{
    Camera source, overlay;
    RenderTexture image;
    bool previousEnabled;
    public bool Captured { get { return source!=null && WristHackScreen.Background==image; } }
    public void Capture(Camera camera)
    {
        Restore();
        if(camera==null || !camera.enabled) return;
        float scale=Mathf.Min(1f,1280f/Mathf.Max(1,Mathf.Max(Screen.width,Screen.height)));
        int width=Mathf.Max(1,Mathf.RoundToInt(Screen.width*scale));
        int height=Mathf.Max(1,Mathf.RoundToInt(Screen.height*scale));
        if(image==null || image.width!=width || image.height!=height)
        {
            ReleaseImage();
            image=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);
            image.name="Cached dungeon behind HACK";
            image.filterMode=FilterMode.Bilinear;image.useMipMap=false;image.Create();
        }
        try
        {
            var request=new UniversalRenderPipeline.SingleCameraRequest { destination=image };
            RenderPipeline.SubmitRenderRequest(camera,request);
            source=camera;previousEnabled=camera.enabled;
            WristHackScreen.Background=image;
            source.enabled=false;
            if(overlay==null)
            {
                var root=new GameObject("HACK UI-only camera");root.transform.SetParent(transform,false);
                overlay=root.AddComponent<Camera>();overlay.cullingMask=0;
                overlay.clearFlags=CameraClearFlags.SolidColor;overlay.backgroundColor=Color.black;
                overlay.allowHDR=false;overlay.allowMSAA=false;
                var data=overlay.GetUniversalAdditionalCameraData();
                data.renderPostProcessing=false;data.renderShadows=false;
                data.requiresColorTexture=false;data.requiresDepthTexture=false;
            }
            overlay.depth=source.depth+1;overlay.enabled=true;
        }
        catch(System.Exception exception)
        {
            // Keep live rendering if a pipeline cannot produce this snapshot.
            Debug.LogWarning("HACK background capture unavailable: "+exception.Message,this);
            Restore();
        }
    }
    public void Restore()
    {
        if(overlay!=null) overlay.enabled=false;
        if(source!=null) source.enabled=previousEnabled;
        source=null;
        if(image!=null && WristHackScreen.Background==image) WristHackScreen.Background=null;
    }
    void OnDisable() { Restore(); }
    void OnDestroy() { Restore();ReleaseImage();if(overlay!=null) Destroy(overlay.gameObject); }
    void ReleaseImage()
    {
        if(image==null) return;
        image.Release();Destroy(image);image=null;
    }
}