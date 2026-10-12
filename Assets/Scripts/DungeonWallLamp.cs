using UnityEngine;
using System.Collections.Generic;

// Local +Z points away from the wall. The mount origin is at floor level.
[ExecuteAlways]
public class DungeonWallLamp : MonoBehaviour
{
    public Color lightColor = new Color(1f,.02f,.45f);
    [Min(0)] public float intensity = 2.0f;
    [Min(.1f)] public float range = 4.5f;
    [Range(0,.5f)] public float flicker = .07f;
    [System.NonSerialized] GameObject visual;
    Light glow;
    Transform flame;
    readonly List<Object> owned = new List<Object>();

    void OnEnable()
    {
        if (!gameObject.scene.IsValid() || !gameObject.scene.isLoaded) return;
        Build();
    }
    public void Build()
    {
        if (visual != null) return;
        visual = new GameObject("Lamp visuals");
        visual.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        visual.transform.SetParent(transform,false);
        var dark = Material("Virtual Dungeon/Wall Lamp Flame",new Color(.025f,.018f,.045f));
        dark.SetFloat("_Smoothness",.4f);
        var outer = Material("Virtual Dungeon/Wall Lamp Flame",new Color(1.3f,.015f,.42f));
        var middle = Material("Virtual Dungeon/Wall Lamp Flame",new Color(1.6f,.12f,.7f));
        var core = Material("Virtual Dungeon/Wall Lamp Flame",new Color(1.65f,.9f,1.25f));
        var bracket = new List<CombineInstance>();
        Block(bracket,new Vector3(0,.9f,.04f),new Vector3(.34f,.85f,.08f));
        Block(bracket,new Vector3(0,.65f,.2f),new Vector3(.12f,.12f,.36f));
        Block(bracket,new Vector3(0,.95f,.33f),new Vector3(.14f,.6f,.14f));
        Block(bracket,new Vector3(0,1.22f,.33f),new Vector3(.44f,.12f,.3f));
        for(int side=-1;side<=1;side+=2)
            Block(bracket,new Vector3(side*.19f,1.32f,.33f),new Vector3(.065f,.25f,.3f));
        var bracketRoot = new GameObject("Compact bracket");
        bracketRoot.transform.SetParent(visual.transform,false);
        bracketRoot.transform.localScale = Vector3.one * .6f;
        bracketRoot.transform.localPosition = new Vector3(0,1.32f*(1-.6f),0);
        Render("Dark wall bracket",bracket,dark,bracketRoot.transform);
        var flameRoot = new GameObject("Pixel flame"); flameRoot.transform.SetParent(visual.transform,false);
        flame = flameRoot.transform;
        var outside = new List<CombineInstance>(); var inside = new List<CombineInstance>(); var white = new List<CombineInstance>();
        // Jagged silhouette with a narrow upper tip and a white-hot center.
        int[] widths={3,5,5,5,3,3,1,1,1};
        for(int y=0;y<widths.Length;y++) for(int x=0;x<widths[y];x++)
        {
            float px=(x-(widths[y]-1)*.5f)*.085f + (y>=6?.085f:0);
            bool center=Mathf.Abs(px)<.045f && y<4;
            var target=center ? white : (Mathf.Abs(px)<.18f && y<7 ? inside : outside);
            Block(target,new Vector3(px,1.34f+y*.085f,.2f),new Vector3(.086f,.086f,.12f));
        }
        Render("Magenta flame edge",outside,outer,flame);
        Render("Pink flame",inside,middle,flame);
        Render("White flame core",white,core,flame);
        Shader haloShader=Resources.Load<Shader>("Shaders/WallLampHalo");
        if(haloShader!=null)
        {
            var haloMaterial=new Material(haloShader); owned.Add(haloMaterial);
            haloMaterial.SetColor("_Color",new Color(1,.015f,.35f,.38f));
            var halo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            halo.name="Soft magenta halo";halo.transform.SetParent(visual.transform,false);
            halo.transform.localPosition=new Vector3(0,1.68f,.14f);halo.transform.localScale=new Vector3(1.45f,1.8f,1);
            Remove(halo.GetComponent<Collider>());
            halo.GetComponent<Renderer>().sharedMaterial=haloMaterial;
        }
        glow = new GameObject("Magenta wall light").AddComponent<Light>();
        glow.transform.SetParent(visual.transform,false); glow.transform.localPosition=new Vector3(0,1.65f,.45f);
        glow.type=LightType.Point;glow.shadows=LightShadows.None;
        Tick();
    }
    Material Material(string shader,Color color)
    {
        var material=new Material(shader=="Virtual Dungeon/Wall Lamp Flame" ? Resources.Load<Shader>("Shaders/WallLampFlame") : Shader.Find(shader));owned.Add(material);material.SetColor("_BaseColor",color);return material;
    }
    void Block(List<CombineInstance> blocks,Vector3 position,Vector3 size)
    {
        // Built-in cube geometry is combined into one draw mesh per color.
        if(cube==null)
        {
            var temporary=GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube=temporary.GetComponent<MeshFilter>().sharedMesh;Remove(temporary);
        }
        blocks.Add(new CombineInstance { mesh=cube,transform=Matrix4x4.TRS(position,Quaternion.identity,size) });
    }
    static Mesh cube;
    void Render(string name,List<CombineInstance> blocks,Material material,Transform parent)
    {
        if(blocks.Count==0) return;
        var mesh=new Mesh { name=name };mesh.CombineMeshes(blocks.ToArray());owned.Add(mesh);
        var part=new GameObject(name);part.transform.SetParent(parent,false);
        part.AddComponent<MeshFilter>().sharedMesh=mesh;
        var renderer=part.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;
        renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    void Update() { if(visual==null) Build();Tick(); }
    void Tick()
    {
        if(glow==null) return;
        float phase=transform.position.x*.7f+transform.position.z*1.3f;
        float pulse=1+flicker*(Mathf.Sin(Time.time*7+phase)*.65f+Mathf.Sin(Time.time*13+phase)*.35f);
        var settings=DungeonVisualSettings.Current;
        if(settings!=null){lightColor=settings.torchColor;range=settings.torchRange;intensity=settings.torchIntensity;flicker=settings.torchFlicker;}
        glow.enabled=true;glow.color=lightColor;glow.range=range;glow.intensity=intensity*pulse;
        if(flame!=null) flame.localScale=new Vector3(1,1+(pulse-1)*.15f,1);
    }
    void OnDisable()
    {
        if(visual!=null) Remove(visual);
        visual=null;glow=null;flame=null;
        foreach(var resource in owned) if(resource!=null) Remove(resource);
        owned.Clear();
    }
    static void Remove(Object target)
    {
        if(target==null) return;
        if(Application.isPlaying) Destroy(target);else DestroyImmediate(target);
    }
}