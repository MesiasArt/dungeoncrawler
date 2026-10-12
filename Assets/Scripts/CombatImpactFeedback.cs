using UnityEngine;

// Offsets only the camera, then removes the offset exactly after the impact.
public class CombatImpactFeedback : MonoBehaviour
{
    [Min(0)] public float attackAmplitude=.035f;
    [Min(0)] public float hurtAmplitude=.065f;
    [Min(.01f)] public float attackDuration=.14f;
    [Min(.01f)] public float hurtDuration=.22f;
    Vector3 offset;
    float remaining, duration, amplitude, phase;
    public bool Active { get { return remaining>0; } }
    public void Play(bool received)
    {
        PlayImpact(received?hurtAmplitude:attackAmplitude,received?hurtDuration:attackDuration);
    }
    public void PlayImpact(float strength,float seconds)
    {
        if(Active&&amplitude>strength)return;
        duration=Mathf.Max(.01f,seconds);remaining=duration;amplitude=Mathf.Max(0,strength);phase=0;
    }
    void LateUpdate() { Tick(Time.unscaledDeltaTime); }
    public void Tick(float dt)
    {
        transform.position-=offset; offset=Vector3.zero;
        if(remaining<=0) return;
        remaining=Mathf.Max(0,remaining-dt); phase+=dt*95;
        float strength=amplitude*(remaining/duration);
        offset=transform.right*(Mathf.Sin(phase)*strength)
            +transform.up*(Mathf.Sin(phase*1.73f)*strength*.65f);
        transform.position+=offset;
    }
    void OnDisable() { transform.position-=offset; offset=Vector3.zero; remaining=0; }
}
