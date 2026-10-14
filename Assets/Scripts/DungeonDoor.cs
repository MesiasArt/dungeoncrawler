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
    public bool closeAfterPassage,repeatTransition;
    public bool IsOpen { get; private set; }
    public bool IsOpening { get; private set; }
    public bool HasBeenPassed { get; private set; }
    public event Action Passed;
    Vector3 closedPosition;
    bool initialized;
    Material doorSurface;
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
    void OnDestroy() { if(doorSurface!=null) { if(Application.isPlaying) Destroy(doorSurface); else DestroyImmediate(doorSurface); } }
    public bool CanInteract(Vector3 player)
    { return !IsOpen&&!IsOpening && Vector3.Distance(player,transform.position+Vector3.up*1.5f)<=interactionDistance; }
    public void SetOpenImmediately(){Initialize();StopAllCoroutines();panel.localPosition=closedPosition+Vector3.up*openingTravel;blocker.enabled=false;IsOpen=true;IsOpening=false;}
    public void SetClosedImmediately(){Initialize();StopAllCoroutines();panel.localPosition=closedPosition;blocker.enabled=true;IsOpen=false;IsOpening=false;}
    public void SetCityEntranceAppearance(){if(doorSurface==null)return;doorSurface.SetColor("_BaseColor",new Color(.9f,.95f,1));doorSurface.SetColor("_ShadowColor",new Color(.65f,.75f,.85f));doorSurface.SetFloat("_EmissionStrength",1.5f);}
    public void OpenDoor()
    { if(!IsOpen&&!IsOpening) StartCoroutine(AnimateOpen()); }
    IEnumerator AnimateOpen()
    {
        DungeonAudio.Play(DungeonAudio.Cue.Door);
        IsOpening=true;
        for(float distance=0;distance<openingTravel;distance+=Mathf.Max(.1f,settings.openingSpeed)*Time.deltaTime)
        { panel.localPosition=closedPosition+Vector3.up*distance; yield return null; }
        panel.localPosition=closedPosition+Vector3.up*openingTravel;
        blocker.enabled=false;IsOpen=true;IsOpening=false;
    }
    bool Crosses(Vector3 from, Vector3 to)
    {
        Vector3 a=transform.InverseTransformPoint(from), b=transform.InverseTransformPoint(to);
        if((a.z<0)==(b.z<0) || Mathf.Abs(b.z-a.z)<.001f) return false;
        float t=-a.z/(b.z-a.z);Vector3 p=Vector3.Lerp(a,b,t);
        return Mathf.Abs(p.x)<doorwayWidth*.5f && p.y>=0 && p.y<=3;
    }
    public bool Blocks(Vector3 from, Vector3 to) { return !IsOpen&&Crosses(from,to); }
    public void CloseDoor(){if(IsOpen&&!IsOpening)StartCoroutine(AnimateClose());}
    IEnumerator AnimateClose(){DungeonAudio.Play(DungeonAudio.Cue.Door);IsOpening=true;IsOpen=false;blocker.enabled=true;for(float distance=openingTravel;distance>0;distance-=Mathf.Max(.1f,settings.openingSpeed)*Time.deltaTime){panel.localPosition=closedPosition+Vector3.up*distance;yield return null;}panel.localPosition=closedPosition;IsOpening=false;}
    public void CheckPassage(Vector3 from, Vector3 to)
    {
        if(!IsOpen || !Crosses(from,to)) return;
        if((!HasBeenPassed||repeatTransition) && transform.InverseTransformPoint(to).z>0){HasBeenPassed=true;if(Passed!=null) Passed();}
        if(closeAfterPassage)CloseDoor();
    }
}
