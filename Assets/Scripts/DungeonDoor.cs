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
    public bool IsOpen { get; private set; }
    public bool IsOpening { get; private set; }
    public bool HasBeenPassed { get; private set; }
    public event Action Passed;
    Vector3 closedPosition;
    bool initialized;
    void Start() { if(!initialized && panel!=null && blocker!=null) Initialize(); }
    public void Initialize()
    {
        if(initialized) return;
        initialized=true;
        if(settings==null) settings=ScriptableObject.CreateInstance<DungeonDoorSettings>();
        closedPosition=panel.localPosition;
        IsOpen=settings.initiallyOpen;
        panel.localPosition=closedPosition+Vector3.up*(IsOpen ? openingTravel : 0);
        blocker.enabled=!IsOpen;
    }
    public bool CanInteract(Vector3 player)
    { return !IsOpen&&!IsOpening && Vector3.Distance(player,transform.position+Vector3.up*1.5f)<=interactionDistance; }
    public void OpenDoor()
    { if(!IsOpen&&!IsOpening) StartCoroutine(AnimateOpen()); }
    IEnumerator AnimateOpen()
    {
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
    public void CheckPassage(Vector3 from, Vector3 to)
    {
        if(!IsOpen || HasBeenPassed || !Crosses(from,to) || transform.InverseTransformPoint(to).z<=0) return;
        HasBeenPassed=true;if(Passed!=null) Passed();
    }
}
