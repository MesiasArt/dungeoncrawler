using System.Collections.Generic;
using UnityEngine;

// Coordinates are in a fixed 900 x 360 UI arena; y is height above the floor.
public sealed class EnemyJumpModel
{
    public const float Width=900, Height=360, PlayerX=150, PlayerSize=32, EnemySize=40;
    public readonly List<float> Enemies = new List<float>();
    public float Y { get; private set; }
    public float Velocity { get; private set; }
    public int Combo { get; private set; }
    public bool Finished { get; private set; }
    public bool Won { get; private set; }
    public int Damage { get { return Combo; } }
    public string Reason { get; private set; }
    readonly EnemyJumpSettings settings;
    readonly System.Random random;
    float nextSpawn=.75f, clock, lastPress=-100, lastStomp=-100;
    bool weakLanding;
    float nextLandingTime=-1;
    public string BounceFeedback { get; private set; }
    float PressWindow { get { return Mathf.Clamp(settings.landingPressWindow,.03f,.2f); } }
    public EnemyJumpModel(EnemyJumpSettings config, int seed=0)
    { settings=config; random=new System.Random(seed); }
    public bool Jump()
    {
        if(Finished) return false;
        // Only fresh presses count; holding the key never boosts the next landing.
        lastPress=clock;
        if(weakLanding && clock-lastStomp<=PressWindow)
        {
            Y=EnemySize; Velocity=Bounce; weakLanding=false;
            lastPress=-100; nextLandingTime=clock+2*Bounce/Gravity; BounceFeedback="PERFECT!"; return true;
        }
        if(Y>.001f || Velocity>0) return false;
        lastPress=-100; // A ground jump cannot also count as a landing press.
        Velocity=Mathf.Clamp(settings.jumpForce,Mathf.Sqrt(2*Gravity*(EnemySize+PlayerSize)),MaxJump);
        return true;
    }
    float Gravity { get { return Mathf.Max(100,settings.gravity); } }
    float MaxJump { get { return Mathf.Sqrt(2*Gravity*(Height-EnemySize-PlayerSize-20)); } }
    float Bounce { get { return Mathf.Clamp(settings.bounceForce,Mathf.Sqrt(2*Gravity*(EnemySize+PlayerSize)),MaxJump); } }
    public void Tick(float dt)
    {
        // Small steps prevent tunneling even at low frame rates.
        while(dt>0 && !Finished) { float step=Mathf.Min(dt,1f/240); Step(step); dt-=step; }
    }
    void Step(float dt)
    {
        clock+=dt;
        float speed=Mathf.Max(30,settings.enemySpeed)*(1+Mathf.Min(Combo,20)*settings.speedPerCombo);
        // Keep the next enemy in the high bounce's landing window without teleporting it.
        // A missed timing press leaves ordinary enemy motion and the weak bounce intact.
        if(!weakLanding && nextLandingTime>clock+.01f && Enemies.Count>0)
        {
            float nearest=float.MaxValue;
            foreach(float x in Enemies) if(x+EnemySize>PlayerX) nearest=Mathf.Min(nearest,x);
            if(nearest<float.MaxValue)
            {
                float target=PlayerX+(PlayerSize-EnemySize)*.5f;
                speed=Mathf.Clamp((nearest-target)/(nextLandingTime-clock),speed*.65f,speed*1.4f);
            }
        }
        nextSpawn-=dt;
        if(nextSpawn<=0)
        {
            Enemies.Add(Width+EnemySize);
            // Clamp requested spacing to the descending part of the bounce cycle.
            // This preserves a reachable chain even with extreme Inspector settings.
            float bounceCycle=2*Bounce/Gravity;
            float tolerance=Mathf.Min(.08f,(PlayerSize+EnemySize)/speed*.2f);
            float variation=1+((float)random.NextDouble()*2-1)*settings.spacingVariation;
            float interval=Mathf.Clamp(settings.enemySpacing/speed,bounceCycle-tolerance,bounceCycle+tolerance);
            nextSpawn=Mathf.Clamp(interval*variation,bounceCycle-tolerance,bounceCycle+tolerance);
        }
        float oldY=Y;
        Velocity-=Gravity*dt; Y=Mathf.Max(0,Y+Velocity*dt);
        if(Y==0 && Velocity<0) Velocity=0;
        for(int i=Enemies.Count-1;i>=0;i--)
        {
            Enemies[i]-=speed*dt;
            float x=Enemies[i];
            bool overlap=x<PlayerX+PlayerSize && x+EnemySize>PlayerX;
            if(overlap && Velocity<0 && oldY>=EnemySize && Y<=EnemySize)
            {
                Enemies.RemoveAt(i); Y=EnemySize; Combo++;
                bool precise=clock-lastPress<=PressWindow;
                Velocity=precise ? Bounce : Mathf.Clamp(settings.weakBounceForce,0,Bounce*.35f);
                weakLanding=!precise; lastStomp=clock; lastPress=-100;
                nextLandingTime=precise ? clock+2*Bounce/Gravity : -1;
                BounceFeedback=precise ? "PERFECT!" : "REBOTE BAJO: PULSA AL CAER";
                if(Combo>=Mathf.Max(1,settings.enemiesToWin)) { Finished=Won=true; Reason="HACK COMPLETADO"; return; }
            }
            else if(overlap && Y<EnemySize && Y+PlayerSize>0)
            { Fail("COLISION LATERAL"); return; }
            else if(x+EnemySize<PlayerX) { Fail("ENEMIGO SIN DERROTAR"); return; }
        }
    }
    void Fail(string reason) { Finished=true; Won=false; Reason=reason; }
}
