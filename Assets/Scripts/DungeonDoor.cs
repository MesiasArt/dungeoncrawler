using System;
using System.Collections;
using UnityEngine;

// Local +Z is the exit side; opening slides the panel upward.
public class DungeonDoor : MonoBehaviour
{
    public DungeonDoorSettings settings;
    public Transform panel;
    public Collider blocker;
    [Min(.1f)] public float interactionDistance=2.2f;
    [Min(.1f)] public float doorwayWidth=3;
    [Min(.1f)] public float openingTravel=3.2f;
    public bool closeAfterPassage=true,repeatTransition;
    public bool IsPermanentlyOpen {get;private set;}
    public void ResetPermanentOpen(){IsPermanentlyOpen=false;closeAfterPassage=true;}
    public void KeepOpenPermanently(){IsPermanentlyOpen=true;closeAfterPassage=false;if(!IsOpen)SetOpenImmediately();}
    public bool IsOpen { get; private set; }
    public bool IsOpening { get; private set; }
    public bool HasBeenPassed { get; private set; }
    public event Action Passed;
    public event Action Opened;
    public event Action Closed;
    Vector3 closedPosition;
    bool initialized;
    Material portalMaterial;
    Material doorSurface;DigitalDoorVisual digitalVisual;
    public void ConfigureDigital(DigitalDoorAccess access,bool boss,bool effects,bool requiresToken=true){Initialize();digitalVisual=GetComponent<DigitalDoorVisual>()??gameObject.AddComponent<DigitalDoorVisual>();digitalVisual.Build(this,access,boss,effects,requiresToken);}
    void Start() { if(!initialized && panel!=null && blocker!=null) Initialize(); }
    public void Initialize()
    {
        if(initialized) return;
        initialized=true;
        if(settings==null) settings=ScriptableObject.CreateInstance<DungeonDoorSettings>();
        // Every generated door gets its own plain surface, never the corridor grid material.
        var renderer=panel.GetComponent<Renderer>();
        var shader=Resources.Load<Shader>("Shaders/DungeonMetalToon");
        if(renderer!=null && shader!=null)
        {
            doorSurface=new Material(shader) { name="Door - dark blue metal" };
            doorSurface.SetTexture("_BaseMap",Texture2D.whiteTexture);
            doorSurface.SetColor("_BaseColor",new Color(.055f,.12f,.22f,1));
            doorSurface.SetColor("_ShadowColor",new Color(.025f,.045f,.085f,1));
            doorSurface.SetFloat("_Metallic",.8f);
            doorSurface.SetFloat("_Smoothness",.65f);
            doorSurface.SetFloat("_EmissionStrength",0);
            doorSurface.SetFloat("_ContinuousFloorGrid",0);
            doorSurface.SetFloat("_ContinuousWallGrid",0);
            renderer.sharedMaterial=doorSurface;
        }
        closedPosition=panel.localPosition;
        IsOpen=settings.initiallyOpen;
        panel.localPosition=closedPosition+Vector3.up*(IsOpen ? openingTravel : 0);
        blocker.enabled=!IsOpen;
    }
    void OnDestroy() { if(portalMaterial!=null)Destroy(portalMaterial); if(doorSurface!=null) { if(Application.isPlaying) Destroy(doorSurface); else DestroyImmediate(doorSurface); } }
    public bool CanInteract(Vector3 player)
    { return !IsOpen&&!IsOpening && Vector3.Distance(player,transform.position+Vector3.up*1.5f)<=interactionDistance; }
    public void SetOpenImmediately(){Initialize();StopAllCoroutines();panel.localPosition=closedPosition+Vector3.up*openingTravel;blocker.enabled=false;IsOpen=true;IsOpening=false;if(digitalVisual!=null)digitalVisual.SyncImmediate(true);}
    public void SetClosedImmediately(){if(IsPermanentlyOpen)return;Initialize();bool changed=IsOpen||IsOpening;StopAllCoroutines();panel.localPosition=closedPosition;blocker.enabled=true;IsOpen=false;IsOpening=false;if(digitalVisual!=null)digitalVisual.SyncImmediate(false);if(changed&&Closed!=null)Closed();}
    public bool CrossesPassage(Vector3 from,Vector3 to){return Crosses(from,to);}
    public void SetCityEntranceAppearance(){CreateCityLight();if(doorSurface==null)return;doorSurface.SetColor("_BaseColor",new Color(.9f,.95f,1));doorSurface.SetColor("_ShadowColor",new Color(.65f,.75f,.85f));doorSurface.SetFloat("_EmissionStrength",1.5f);}
    void CreateCityLight(){
        if(transform.Find("City light")!=null||panel==null)return;
        var shader=Shader.Find("Universal Render Pipeline/Unlit");if(shader==null)return;
        portalMaterial=new Material(shader);portalMaterial.SetColor("_BaseColor",Color.white);portalMaterial.SetFloat("_Cull",0);
        var plane=GameObject.CreatePrimitive(PrimitiveType.Quad);plane.name="City light";plane.transform.SetParent(transform,false);
        plane.transform.localPosition=new Vector3(0,closedPosition.y,.09f);plane.transform.localScale=new Vector3(doorwayWidth,panel.localScale.y,1);plane.GetComponent<Renderer>().sharedMaterial=portalMaterial;
        Destroy(plane.GetComponent<Collider>());
    }
    public void OpenDoor()
    { if(!IsOpen&&!IsOpening) StartCoroutine(AnimateOpen()); }
    IEnumerator AnimateOpen()
    {
        DungeonAudio.Play(DungeonAudio.Cue.Door);
        IsOpening=true;
        if(digitalVisual!=null)yield return digitalVisual.Unlock();
        for(float distance=0;distance<openingTravel;distance+=Mathf.Max(.1f,settings.openingSpeed)*Time.deltaTime)
        { panel.localPosition=closedPosition+Vector3.up*distance; yield return null; }
        panel.localPosition=closedPosition+Vector3.up*openingTravel;
        blocker.enabled=false;IsOpen=true;IsOpening=false;if(Opened!=null)Opened();
    }
    bool Crosses(Vector3 from, Vector3 to)
    {
        Vector3 a=transform.InverseTransformPoint(from), b=transform.InverseTransformPoint(to);
        if((a.z<0)==(b.z<0) || Mathf.Abs(b.z-a.z)<.001f) return false;
        float t=-a.z/(b.z-a.z);Vector3 p=Vector3.Lerp(a,b,t);
        return Mathf.Abs(p.x)<doorwayWidth*.5f && p.y>=0 && p.y<=3;
    }
    public bool Blocks(Vector3 from, Vector3 to) { return !IsOpen&&Crosses(from,to); }
    public void CloseDoor(){if(!IsPermanentlyOpen&&IsOpen&&!IsOpening)StartCoroutine(AnimateClose());}
    IEnumerator AnimateClose(){DungeonAudio.Play(DungeonAudio.Cue.Door);IsOpening=true;IsOpen=false;blocker.enabled=true;if(Closed!=null)Closed();for(float distance=openingTravel;distance>0;distance-=Mathf.Max(.1f,settings.openingSpeed)*Time.deltaTime){panel.localPosition=closedPosition+Vector3.up*distance;yield return null;}panel.localPosition=closedPosition;IsOpening=false;}
    public void CheckPassage(Vector3 from, Vector3 to)
    {
        if(!IsOpen || !Crosses(from,to)) return;
        if((!HasBeenPassed||repeatTransition) && transform.InverseTransformPoint(to).z>0){HasBeenPassed=true;if(Passed!=null) Passed();}
        if(closeAfterPassage)CloseDoor();
    }
}
