using System.Collections.Generic;
using UnityEngine;

// Coordinates are in a fixed 900 x 360 UI arena; y is height above the floor.
public sealed class EnemyJumpModel
{
    public const float Width=900, Height=360, PlayerX=150, PlayerSize=32, EnemySize=80;
    public readonly float EnemyExtent;
    public readonly List<float> Enemies = new List<float>();
    public readonly List<int> EnemyVisualIds = new List<int>();
    int spawnedEnemies;
    readonly bool rhythmic;
    readonly List<double> arrivalTimes=new List<double>();
    double nextArrival;
    float flightGravity;
    double Interval { get { return BeatSeconds*System.Math.Max(1,settings.beatsPerEnemy); } }
    public double GroundFlightSeconds { get { return rhythmic ? BeatSeconds*System.Math.Max(1,settings.groundJumpBeats) : (settings.jumpForce+System.Math.Sqrt(settings.jumpForce*settings.jumpForce-2*Gravity*EnemyExtent))/Gravity; } }
    public double UpcomingLanding { get { return arrivalTimes.Count>0 ? arrivalTimes[0] : nextArrival; } }
    float MotionGravity { get { return flightGravity>0 ? flightGravity : Gravity; } }
    float RhythmBounceGravity { get { return Mathf.Min(Gravity,(float)(8*(Height-EnemyExtent-PlayerSize-20)/(Interval*Interval))); } }
    void HighBounce()
    {
        flightGravity=rhythmic ? RhythmBounceGravity : Gravity;
        Velocity=Bounce;
    }
    public enum LandingGrade { None, Perfect, Good, Miss }
    public LandingGrade LastGrade { get; private set; }
    public double LastLandingError { get; private set; }
    public double MusicalTime { get { return clock; } }
    public double BeatSeconds { get { return 60d/System.Math.Max(1,settings.bpm); } }
    public double NextArrival { get { return nextArrival; } }
    public static LandingGrade Judge(double error,float perfect,float good)
    {
        error=System.Math.Abs(error);
        return error<=System.Math.Max(.001,perfect) ? LandingGrade.Perfect :
            error<=System.Math.Max(perfect,good) ? LandingGrade.Good : LandingGrade.Miss;
    }
    public void TickTo(double songTime)
    {
        if(!rhythmic) throw new System.InvalidOperationException("TickTo requires rhythm mode.");
        if(songTime<=clock || Finished) return;
        Tick((float)(songTime-clock));
        if(!Finished) clock=(float)songTime;
    }
    public float Y { get; private set; }
    public float Velocity { get; private set; }
    public int Combo { get; private set; }
    public float LastStompX {get;private set;}
    public int LastStompVisualId {get;private set;}
    public bool Finished { get; private set; }
    public bool Won { get; private set; }
    // Legacy performance points, not final combat damage.
    public int Damage { get { return Combo*2; } }
    public int PerfectCount { get; private set; }
    public int GoodCount { get; private set; }
    public string Reason { get; private set; }
    readonly EnemyJumpSettings settings;
    readonly System.Random random;
    float nextSpawn=.75f, clock, lastPress=-100, lastStomp=-100;
    bool weakLanding;
    float nextLandingTime=-1;
    public string BounceFeedback { get; private set; }
    float PressWindow { get { return Mathf.Clamp(settings.landingPressWindow,.03f,.2f); } }
    public EnemyJumpModel(EnemyJumpSettings config, int seed=0, bool rhythmMode=false, float enemyExtent=EnemySize)
    {
        settings=config;random=new System.Random(seed);rhythmic=rhythmMode;EnemyExtent=Mathf.Clamp(enemyExtent,32,180);
        nextArrival=settings.musicOffsetSeconds+System.Math.Max(4,settings.firstEnemyBeat)*BeatSeconds;
        // First encounter needs enough travel time to appear at the arena's right edge.
        double travel=(Width+EnemyExtent-(PlayerX+(PlayerSize-EnemyExtent)*.5f))/System.Math.Max(30,settings.enemySpeed);
        while(nextArrival<travel) nextArrival+=System.Math.Max(1,settings.beatsPerEnemy)*BeatSeconds;
    }
    public bool Jump()
    {
        if(Finished) return false;
        // Only fresh presses count; holding the key never boosts the next landing.
        lastPress=clock;
        if(weakLanding && clock-lastStomp<=PressWindow)
        {
            Y=EnemyExtent; HighBounce();
            if(rhythmic)
            {
                float elapsed=Mathf.Max(0,clock-lastStomp);
                Y+=Velocity*elapsed-.5f*MotionGravity*elapsed*elapsed;Velocity-=MotionGravity*elapsed;
            }
            weakLanding=false;
            lastPress=-100; nextLandingTime=clock+2*Bounce/Gravity; if(!rhythmic) BounceFeedback="PERFECT!"; return true;
        }
        if(Y>.001f || Velocity>0) return false;
        lastPress=-100; // A ground jump cannot also count as a landing press.
        if(rhythmic)
        {
            double duration=GroundFlightSeconds;
            flightGravity=Mathf.Min(Gravity,(float)(8*(Height-PlayerSize-20-EnemyExtent)/(duration*duration)));
            // A manual ground jump lasts the configured full beat count until it reaches the enemy's top.
            Velocity=(float)(EnemyExtent/duration+flightGravity*duration*.5);
        }
        else { flightGravity=Gravity;Velocity=Mathf.Clamp(settings.jumpForce,Mathf.Sqrt(2*Gravity*(EnemyExtent+PlayerSize)),MaxJump); }
        return true;
    }
    float Gravity { get { return Mathf.Max(100,settings.gravity); } }
    float MaxJump { get { return Mathf.Sqrt(2*Gravity*(Height-EnemyExtent-PlayerSize-20)); } }
    float Bounce { get { return rhythmic ? (float)(RhythmBounceGravity*Interval*.5) : Mathf.Clamp(settings.bounceForce,Mathf.Sqrt(2*Gravity*(EnemyExtent+PlayerSize)),MaxJump); } }
    public void Tick(float dt)
    {
        // Small steps prevent tunneling even at low frame rates.
        while(dt>0 && !Finished) { float step=Mathf.Min(dt,rhythmic ? 1f/120 : 1f/240); Step(step); dt-=step; }
    }
    void Step(float dt)
    {
        // Keep visual IDs aligned when tools seed the public enemy list for simulations.
        while(EnemyVisualIds.Count<Enemies.Count) EnemyVisualIds.Add(spawnedEnemies++);
        clock+=dt;
        float speed=Mathf.Max(30,settings.enemySpeed)*(1+Mathf.Min(Combo,20)*settings.speedPerCombo);
        // Keep the next enemy in the high bounce's landing window without teleporting it.
        // A missed timing press leaves ordinary enemy motion and the weak bounce intact.
        if(!rhythmic && !weakLanding && nextLandingTime>clock+.01f && Enemies.Count>0)
        {
            float nearest=float.MaxValue;
            foreach(float x in Enemies) if(x+EnemyExtent>PlayerX) nearest=Mathf.Min(nearest,x);
            if(nearest<float.MaxValue)
            {
                float target=PlayerX+(PlayerSize-EnemyExtent)*.5f;
                speed=Mathf.Clamp((nearest-target)/(nextLandingTime-clock),speed*.65f,speed*1.4f);
            }
        }
        nextSpawn-=dt;
        if(!rhythmic && nextSpawn<=0)
        {
            Enemies.Add(Width+EnemyExtent);
            EnemyVisualIds.Add(spawnedEnemies++);
            // Clamp requested spacing to the descending part of the bounce cycle.
            // This preserves a reachable chain even with extreme Inspector settings.
            float bounceCycle=2*Bounce/Gravity;
            float tolerance=Mathf.Min(.08f,(PlayerSize+EnemyExtent)/speed*.2f);
            float variation=1+((float)random.NextDouble()*2-1)*settings.spacingVariation;
            float interval=Mathf.Clamp(settings.enemySpacing/speed,bounceCycle-tolerance,bounceCycle+tolerance);
            nextSpawn=Mathf.Clamp(interval*variation,bounceCycle-tolerance,bounceCycle+tolerance);
        }
        if(rhythmic)
        {
            speed=Mathf.Max(30,settings.enemySpeed);
            double travel=(Width+EnemyExtent-(PlayerX+(PlayerSize-EnemyExtent)*.5f))/speed;
            while(clock+travel>=nextArrival && spawnedEnemies<Mathf.Max(1,settings.enemiesToWin))
            {
                Enemies.Add(Width+EnemyExtent);EnemyVisualIds.Add(spawnedEnemies++);
                arrivalTimes.Add(nextArrival);
                nextArrival+=System.Math.Max(1,settings.beatsPerEnemy)*BeatSeconds;
            }
        }
        float oldY=Y;
        float oldVelocity=Velocity;
        if(rhythmic) { Y=Mathf.Max(0,Y+Velocity*dt-.5f*MotionGravity*dt*dt);Velocity-=MotionGravity*dt; }
        else { Velocity-=Gravity*dt;Y=Mathf.Max(0,Y+Velocity*dt); }
        if(Y==0 && Velocity<0) Velocity=0;
        for(int i=Enemies.Count-1;i>=0;i--)
        {
            if(rhythmic) Enemies[i]=PlayerX+(PlayerSize-EnemyExtent)*.5f+(float)(arrivalTimes[i]-clock)*speed;
            else Enemies[i]-=speed*dt;
            float x=Enemies[i];
            bool overlap=x<PlayerX+PlayerSize && x+EnemyExtent>PlayerX;
            if(overlap && Velocity<0 && oldY>=EnemyExtent && Y<=EnemyExtent)
            {
                float remainder=0;
                float contactClock=clock;
                if(rhythmic)
                {
                    double crossing=clock-dt+Mathf.Clamp((oldVelocity+Mathf.Sqrt(oldVelocity*oldVelocity+2*MotionGravity*(oldY-EnemyExtent)))/MotionGravity,0,dt);
                    remainder=Mathf.Max(0,clock-(float)crossing);contactClock=(float)crossing;
                    LastLandingError=crossing-arrivalTimes[i]-settings.judgementOffsetSeconds;
                    LastGrade=Judge(LastLandingError,settings.perfectWindowSeconds,settings.goodWindowSeconds);
                    if(LastGrade==LandingGrade.Miss) { BounceFeedback="MISS";Fail("MISS: FUERA DEL PULSO");return; }
                    arrivalTimes.RemoveAt(i);
                }
                LastStompX=x;LastStompVisualId=EnemyVisualIds[i];
                Enemies.RemoveAt(i); EnemyVisualIds.RemoveAt(i); Y=EnemyExtent; Combo++;if(LastGrade==LandingGrade.Good)GoodCount++;else PerfectCount++;
                bool precise=contactClock-lastPress<=PressWindow;
                if(precise) HighBounce();
                else { flightGravity=Gravity;Velocity=Mathf.Clamp(settings.weakBounceForce,0,Bounce*.35f); }
                if(rhythmic) { Y+=Velocity*remainder-.5f*MotionGravity*remainder*remainder;Velocity-=MotionGravity*remainder; }
                weakLanding=!precise; lastStomp=contactClock; lastPress=-100;
                nextLandingTime=precise ? clock+2*Bounce/Gravity : -1;
                BounceFeedback=rhythmic ? (LastGrade==LandingGrade.Perfect ? "PERFECT" : "GOOD")+(precise ? "" : " / PULSA PARA REBOTAR") : precise ? "PERFECT!" : "REBOTE BAJO: PULSA AL CAER";
                if(Combo>=Mathf.Max(1,settings.enemiesToWin)) { Finished=Won=true; Reason="HACK COMPLETADO"; return; }
            }
            else if(overlap && Y<EnemyExtent && Y+PlayerSize>0)
            { Fail("COLISION LATERAL"); return; }
            else if(x+EnemyExtent<PlayerX) { Fail("ENEMIGO SIN DERROTAR"); return; }
        }
    }
    void Fail(string reason) { if(rhythmic) { LastGrade=LandingGrade.Miss;BounceFeedback="MISS"; } Finished=true; Won=false; Reason=reason; }
}
