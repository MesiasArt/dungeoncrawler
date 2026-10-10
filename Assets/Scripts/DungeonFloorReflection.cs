using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Planar reflection for either horizontal dungeon surface.
public class DungeonFloorReflection : MonoBehaviour
{
    Camera source, mirror;
    Material floor;
    RenderTexture reflection;
    Vector3 lastPosition;
    Quaternion lastRotation;
    Matrix4x4 lastProjection;
    float planeHeight;
    bool captured;
    float lastCaptureTime;
    readonly UniversalRenderPipeline.SingleCameraRequest request = new UniversalRenderPipeline.SingleCameraRequest();

    public void Initialize(Camera camera, Material material, float height = 0)
    {
        source = camera; floor = material; planeHeight = height;
        var root = new GameObject(height == 0 ? "Floor reflection camera" : "Ceiling reflection camera");
        root.transform.SetParent(transform, false);
        mirror = root.AddComponent<Camera>();
        mirror.enabled = false;
        reflection = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGBHalf);
        reflection.name = height == 0 ? "Dungeon floor planar reflection" : "Dungeon ceiling planar reflection";
        reflection.filterMode = FilterMode.Bilinear;
        reflection.wrapMode = TextureWrapMode.Clamp;
        reflection.useMipMap = true;
        reflection.autoGenerateMips = false;
        reflection.Create();
        request.destination = reflection;
        floor.SetTexture("_PlanarReflection", reflection);
        floor.SetFloat("_PlanarStrength", .16f);
    }

    void LateUpdate()
    {
        if (source == null || mirror == null) return;
        // Follow movement immediately; refresh at 2 Hz when idle for enemies/combat.
        if (captured && source.transform.position == lastPosition &&
            source.transform.rotation == lastRotation && source.projectionMatrix == lastProjection &&
            Time.unscaledTime-lastCaptureTime < .5f) return;
        mirror.CopyFrom(source);
        mirror.enabled = false;
        mirror.cameraType = CameraType.Reflection;
        mirror.targetTexture = reflection;
        mirror.allowMSAA = false;
        mirror.useOcclusionCulling = false;
        Matrix4x4 flip = Matrix4x4.TRS(new Vector3(0,2*planeHeight,0), Quaternion.identity, new Vector3(1,-1,1));
        mirror.transform.position = new Vector3(source.transform.position.x,2*planeHeight-source.transform.position.y,source.transform.position.z);
        mirror.transform.rotation = Quaternion.LookRotation(Vector3.Reflect(source.transform.forward,Vector3.up),
            Vector3.Reflect(source.transform.up,Vector3.up));
        mirror.worldToCameraMatrix = source.worldToCameraMatrix * flip;
        Vector3 clipPosition = mirror.worldToCameraMatrix.MultiplyPoint(new Vector3(0,planeHeight + (planeHeight == 0 ? .02f : -.02f),0));
        Vector3 clipNormal = mirror.worldToCameraMatrix.MultiplyVector(planeHeight == 0 ? Vector3.up : Vector3.down).normalized;
        Vector4 clipPlane = new Vector4(clipNormal.x,clipNormal.y,clipNormal.z,-Vector3.Dot(clipPosition,clipNormal));
        mirror.projectionMatrix = source.CalculateObliqueMatrix(clipPlane);
        floor.SetMatrix("_PlanarMatrix", GL.GetGPUProjectionMatrix(mirror.projectionMatrix,true) * mirror.worldToCameraMatrix);
        try
        {
            Shader.SetGlobalFloat("_DungeonReflectionCapture",1);
            // Unity 6's URP render request handles the reflected view's winding.
            RenderPipeline.SubmitRenderRequest(mirror,request);
            reflection.GenerateMips();
            floor.SetFloat("_PlanarReady",1);
            captured = true;
            lastPosition = source.transform.position;
            lastRotation = source.transform.rotation;
            lastProjection = source.projectionMatrix;
            lastCaptureTime = Time.unscaledTime;
        }
        finally
        {
            Shader.SetGlobalFloat("_DungeonReflectionCapture",0);
        }
    }

    void OnDestroy()
    {
        if (floor != null) floor.SetFloat("_PlanarReady",0);
        if (reflection != null) { reflection.Release(); Destroy(reflection); }
        if (mirror != null) Destroy(mirror.gameObject);
    }
}
