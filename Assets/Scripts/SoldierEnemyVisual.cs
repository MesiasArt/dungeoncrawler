using System.Collections;
using UnityEngine;

public sealed class SoldierEnemyVisual : GhostEnemyVisual
{
    protected override string FramesPath { get { return "Enemies/Soldier/"; } }
    protected override Vector2 SpriteSize { get { return new Vector2(2f,1.8f); } }
    Texture2D ready, throwing, grenadeTexture, explosionTexture;
    int counterattacks;
    GameObject projectile;
    Material projectileMaterial;
    public bool NextAttackIsGrenade() { return ++counterattacks % 3 == 0; }
    protected override void Awake()
    {
        ready=Load("special_0");throwing=Load("special_1");grenadeTexture=Load("grenade");
        explosionTexture=Load("explosion");
        base.Awake();
    }
    protected override float Duration(Pose pose) { return pose==Pose.Special ? .65f : base.Duration(pose); }
    protected override Texture SpecialFrame(float elapsed) { return elapsed<.18f ? ready : throwing; }
    public IEnumerator ThrowGrenade(Camera target,bool missed=false)
    {
        Play(Pose.Special);
        yield return new WaitForSeconds(.28f);
        DungeonAudio.Play(DungeonAudio.Cue.GrenadeThrow);
        projectile=GameObject.CreatePrimitive(PrimitiveType.Quad);
        projectile.name="Soldado grenade";
        Destroy(projectile.GetComponent<Collider>());
        projectileMaterial=new Material(SpriteMaterial);
        projectileMaterial.SetTexture("_BaseMap",grenadeTexture);
        projectile.GetComponent<Renderer>().sharedMaterial=projectileMaterial;
        Vector3 start=transform.position+Vector3.up*1.2f;
        float elapsed=0;
        while(elapsed<.32f && target!=null)
        {
            float t=elapsed/.32f;
            projectile.transform.position=Vector3.Lerp(start,target.transform.position+target.transform.forward*.45f+(missed?target.transform.right*.65f:Vector3.zero),t)+Vector3.up*Mathf.Sin(t*Mathf.PI)*.35f;
            projectile.transform.rotation=target.transform.rotation;
            projectile.transform.localScale=Vector3.one*.22f;
            elapsed+=Time.deltaTime;
            yield return null;
        }
        // Start the flash now; the combat coroutine applies damage and shake this frame.
        if(target!=null && explosionTexture!=null) StartCoroutine(ShowExplosion(target,missed));
        else ClearProjectile();
    }
    IEnumerator ShowExplosion(Camera target,bool missed)
    {
        projectile.name="Grenade impact explosion";
        projectileMaterial.SetTexture("_BaseMap",explosionTexture);
        projectile.transform.SetParent(target.transform,false);
        projectile.transform.localPosition=new Vector3(missed?.65f:0,-.025f,.45f);
        projectile.transform.localRotation=Quaternion.identity;
        projectile.transform.localScale=new Vector3(.7f,.48f,1);
        yield return new WaitForSeconds(.24f);
        ClearProjectile();
    }
    void ClearProjectile()
    {
        if(projectile!=null) Destroy(projectile);
        if(projectileMaterial!=null) Destroy(projectileMaterial);
    }
    protected override void OnDestroy() { ClearProjectile();base.OnDestroy(); }
}
