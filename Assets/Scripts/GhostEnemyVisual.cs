using UnityEngine;

// Deliberately stepped sprite animation: no blending, tweening or interpolated poses.
public class GhostEnemyVisual : MonoBehaviour
{
    public enum Pose { Idle, Attack, Hurt, Death, Special }
    [Min(.1f)] public float idleFrameSeconds = .32f;
    public float attackSeconds = .18f, hurtSeconds = .28f, deathSeconds = .65f;
    public Pose CurrentPose { get; private set; }
    Texture2D[] idle;
    Texture2D attack, hurt, death;
    Material material;
    Transform sprite;
    float poseStarted;
    public virtual int Kind { get { return this is BigTapakuEnemyVisual?3:this is SoldierEnemyVisual?2:this is HongoEnemyVisual?1:0; } }
    float SizeMultiplier { get { var settings=EnemySizeSettings.Current;return settings!=null?settings.ScaleFor(Kind):1; } }
    public float WorldHeight { get { return (SpriteSize.y+SpriteOffset.y)*SizeMultiplier*transform.lossyScale.y; } }
    Texture displayed;

    protected virtual string FramesPath { get { return "Enemies/Ghost/"; } }
    [System.NonSerialized] public float minimumSpecialSeconds;
    public virtual bool NextAttackIsSpecial(){return false;}
    protected virtual Texture AttackFrame(float elapsed){return attack;}
    protected virtual Texture DeathFrame(float elapsed){return death;}
    protected Transform SpriteTransform {get{return sprite;}}
    protected virtual Vector2 SpriteOffset { get { return Vector2.zero; } }
    protected virtual Vector2 SpriteSize { get { return new Vector2(3.4f,1.8f); } }
    protected virtual void Awake()
    {
        idle = new[] { Load("idle_0"), Load("idle_1"), Load("idle_2") };
        attack=Load("attack");hurt=Load("hurt");death=Load("death");
        material=new Material(Resources.Load<Shader>("Shaders/EnemySprite")??Shader.Find("Universal Render Pipeline/Unlit"));
        material.SetColor("_BaseColor",Color.white);
        material.SetFloat("_Surface",1);material.SetFloat("_Blend",0);
        material.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetFloat("_ZWrite",0);material.SetFloat("_Cull",0);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.renderQueue=(int)UnityEngine.Rendering.RenderQueue.Transparent;
        var quad=GameObject.CreatePrimitive(PrimitiveType.Quad);
        sprite=quad.transform;
        quad.name=gameObject.name+" sprite";quad.transform.SetParent(transform,false);
        quad.transform.localPosition=new Vector3(SpriteOffset.x,SpriteSize.y*.5f+SpriteOffset.y,0);
        quad.transform.localRotation=Quaternion.Euler(0,180,0);
        quad.transform.localScale=new Vector3(SpriteSize.x,SpriteSize.y,1);
        Destroy(quad.GetComponent<Collider>());
        var renderer=quad.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;
        renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        Play(Pose.Idle);
    }
    protected Texture2D Load(string frame)
    {
        var texture=Resources.Load<Texture2D>(FramesPath+frame);
        if(texture==null) Debug.LogError("Missing enemy frame: "+FramesPath+frame,this);
        return texture;
    }
    public void ResetToIdle(){CurrentPose=Pose.Idle;poseStarted=Time.time;Refresh();}
    public float Play(Pose pose)
    {
        if(CurrentPose==Pose.Death) return deathSeconds;
        if(pose==Pose.Attack) DungeonAudio.Play(this is SoldierEnemyVisual ? DungeonAudio.Cue.SoldierAttack : this is HongoEnemyVisual ? DungeonAudio.Cue.HongoAttack : DungeonAudio.Cue.GhostAttack);
        else if(pose==Pose.Hurt) DungeonAudio.Play(DungeonAudio.Cue.EnemyHurt);
        else if(pose==Pose.Death) DungeonAudio.Play(DungeonAudio.Cue.EnemyDeath);
        CurrentPose=pose;poseStarted=Time.time;Refresh();
        return Duration(pose);
    }
    protected virtual float Duration(Pose pose)
    { return pose==Pose.Attack ? attackSeconds : pose==Pose.Hurt ? hurtSeconds : pose==Pose.Death ? deathSeconds : 0; }
    protected virtual void Update()
    {
        if(CurrentPose!=Pose.Idle && CurrentPose!=Pose.Death && Time.time-poseStarted>=Duration(CurrentPose)) Play(Pose.Idle);
        if(sprite!=null){sprite.localScale=new Vector3(SpriteSize.x*SizeMultiplier,SpriteSize.y*SizeMultiplier,1);sprite.localPosition=new Vector3(SpriteOffset.x*SizeMultiplier,(SpriteSize.y*.5f+SpriteOffset.y)*SizeMultiplier,0);}
        Refresh();
    }
    void Refresh()
    {
        if(material==null || idle==null) return;
        Texture frame=CurrentPose==Pose.Special ? SpecialFrame(Time.time-poseStarted) : CurrentPose==Pose.Attack ? AttackFrame(Time.time-poseStarted) : CurrentPose==Pose.Hurt ? hurt : CurrentPose==Pose.Death ? DeathFrame(Time.time-poseStarted) :
            idle[Mathf.FloorToInt((Time.time-poseStarted)/Mathf.Max(.1f,idleFrameSeconds))%idle.Length];
        if(frame==displayed) return;
        displayed=frame;material.SetTexture("_BaseMap",frame);
    }
    protected virtual Texture SpecialFrame(float elapsed) { return attack; }
    protected Material SpriteMaterial { get { return material; } }
    protected virtual void OnDestroy() { if(material!=null) Destroy(material); }
}