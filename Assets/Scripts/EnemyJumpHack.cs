using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyJumpHack : CombatHack
{
    public override bool UsesAttackLuck { get { return false; } }
    public override string DisplayName { get { return "Jump"; } }
    public EnemyJumpSettings settings;
    public EnemyJumpModel Model { get; private set; }
    public override bool Active { get { return Model!=null; } }
    Action<int> completed;
    EnemyJumpSettings sessionSettings;
    public JumpRhythmClock Rhythm { get; private set; }
    int shownBeat=-1;
    string beatText="BEAT 0";
    void StopRhythm()
    {
        if(Rhythm==null) return;
        Rhythm=null;
        if(sessionSettings!=null) { Destroy(sessionSettings);sessionSettings=null; }
    }
    float resultTime;
    bool jumpInputReleased;
    GUIStyle label, button;
    Texture2D playerSprite, jumpSprite, deathSprite;
    Texture2D[][] walkingEnemies;
    int opponentKind;
    int attackStrength=7,targetDefense,lastPreviewDamage;
    float completedQuality;HackExecutionOutcome completedOutcome;
    public override HackExecutionOutcome LastOutcome {get{return completedOutcome;}}
    public void ConfigureAttack(int strength,int defense){attackStrength=Mathf.Max(0,strength);targetDefense=Mathf.Max(0,defense);}
    public override float AttackQuality(int performanceScore){return Model!=null?CombatMath.JumpQuality(Model.Combo,Model.PerfectCount,Model.GoodCount,sessionSettings!=null?sessionSettings.enemiesToWin:settings.enemiesToWin,CombatBalanceSettings.Current):completedQuality;}
    int PreviewDamage(int combo,int perfect,int good){int target=Mathf.Max(1,sessionSettings!=null?sessionSettings.enemiesToWin:settings.enemiesToWin);return CombatMath.Damage(attackStrength,targetDefense,CombatMath.JumpQuality(combo,perfect,good,target,CombatBalanceSettings.Current),false,1,CombatBalanceSettings.Current);}
    public void SetOpponent(GameObject opponent)
    {
        var visual=opponent!=null?opponent.GetComponent<GhostEnemyVisual>():null;
        opponentKind=visual!=null?visual.Kind:0;
    }
    bool deathStarted, deathComplete;
    float deathTime, deathStartY, deathY;
    HackDungeonBackdrop backdrop;
    JumpParallaxBackdrop parallax;
    float backgroundStarted;
    int displayedCombo=-1, displayedTarget=-1;
    string comboText, damageText, resultText;
    struct DamagePopup { public float started,height,x;public int visualId;public string caption; }
    readonly DamagePopup[] popups=new DamagePopup[8];
    int nextPopup;
    GUIStyle popupStyle;
    void AddDamagePopup(int total,int added)
    {
        popups[nextPopup]=new DamagePopup { started=Time.unscaledTime,height=Model.Y,x=Model.LastStompX,visualId=Model.LastStompVisualId,caption="+"+Model.Combo };
        nextPopup=(nextPopup+1)%popups.Length;
        DungeonAudio.Play(DungeonAudio.Cue.Stomp);
        DungeonAudio.Play(DungeonAudio.Cue.JumpPoint);
    }
    void Jump(double? inputSongTime=null)
    {
        if(Model==null || Model.Finished) return;
        // Timestamp both keyboard and GUI input against the current audio clock.
        if(Rhythm!=null) Model.TickTo(System.Math.Max(0,inputSongTime ?? Rhythm.SongTime));
        if(Model.Jump()) DungeonAudio.Play(DungeonAudio.Cue.Jump);
        RefreshHud();
    }

    void RefreshHud()
    {
        if(Model==null) return;
        int target=Mathf.Max(1,sessionSettings!=null ? sessionSettings.enemiesToWin : settings.enemiesToWin);
        if(displayedCombo==Model.Combo && displayedTarget==target) return;
        int preview=PreviewDamage(Model.Combo,Model.PerfectCount,Model.GoodCount);
        if(displayedCombo>=0 && Model.Combo>displayedCombo)AddDamagePopup(preview,Mathf.Max(0,preview-lastPreviewDamage));
        lastPreviewDamage=preview;
        displayedCombo=Model.Combo;displayedTarget=target;
        comboText="COMBO x "+Model.Combo.ToString("00");
        damageText="DAÑO: "+preview.ToString("00")+" / "+PreviewDamage(target,target,0).ToString("00");
        resultText="DAÑO CONFIRMADO: "+preview;
    }
    public override void Begin(Action<int> callback)
    {
        if(Active) return;
        if(playerSprite==null) playerSprite=Resources.Load<Texture2D>("UI/GamerboyJump");
        if(jumpSprite==null) jumpSprite=Resources.Load<Texture2D>("UI/GamerboyJumpAir");
        if(deathSprite==null) deathSprite=Resources.Load<Texture2D>("UI/GamerboyJumpDeath");
        if(walkingEnemies==null)
        {
            walkingEnemies=new Texture2D[EnemyRoster.Count][];
            for(int i=0;i<EnemyRoster.Count;i++)walkingEnemies[i]=new[] {
                Resources.Load<Texture2D>("UI/JumpEnemies/"+EnemyRoster.ResourceIds[i]+"_0")??Resources.Load<Texture2D>("Enemies/"+EnemyRoster.ResourceIds[i]+"/idle_0"),
                Resources.Load<Texture2D>("UI/JumpEnemies/"+EnemyRoster.ResourceIds[i]+"_1")??Resources.Load<Texture2D>("Enemies/"+EnemyRoster.ResourceIds[i]+"/idle_1") };

        }
        if(parallax==null)parallax=GetComponent<JumpParallaxBackdrop>()??gameObject.AddComponent<JumpParallaxBackdrop>();
        parallax.Prepare();backgroundStarted=Time.unscaledTime;
        Array.Clear(popups,0,popups.Length);nextPopup=0;
        DungeonAudio.Play(DungeonAudio.Cue.Confirm);
        completedQuality=0;completedOutcome=HackExecutionOutcome.Completed;lastPreviewDamage=0;
        deathStarted=deathComplete=false;deathTime=0;
        if(settings==null) settings=Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");
        if(settings==null) settings=ScriptableObject.CreateInstance<EnemyJumpSettings>();
        sessionSettings=Instantiate(settings);
        Rhythm=new JumpRhythmClock(sessionSettings);
        shownBeat=-1;
        float extent=sessionSettings.referenceEnemyPixels;
        var sizes=EnemySizeSettings.Current;
        if(sessionSettings.matchEnemyReferenceSize&&sizes!=null)extent*=sizes.ScaleFor(opponentKind)/sizes.ScaleFor(0);
        extent=Mathf.Clamp(extent,Mathf.Min(sessionSettings.minimumEnemyPixels,sessionSettings.maximumEnemyPixels),Mathf.Max(sessionSettings.minimumEnemyPixels,sessionSettings.maximumEnemyPixels));
        Model=new EnemyJumpModel(sessionSettings,Environment.TickCount,true,extent); completed=callback; resultTime=0; jumpInputReleased=false;
        displayedCombo=displayedTarget=-1;RefreshHud();
        if(backdrop==null) backdrop=gameObject.GetComponent<HackDungeonBackdrop>();
        if(backdrop==null) backdrop=gameObject.AddComponent<HackDungeonBackdrop>();
        backdrop.Capture(GetComponentInChildren<Camera>());
    }
    void Update()
    {
        if(!Active) return;
        if(Model.Finished)
        {
            if(!Model.Won && !deathComplete)
            {
                if(!deathStarted) { deathStarted=true;deathStartY=deathY=Model.Y;deathTime=0;DungeonAudio.Play(DungeonAudio.Cue.HackFail); }
                deathTime+=Time.unscaledDeltaTime;
                // A brief freeze, an upward kick, then an unchecked fall below the arena.
                float t=Mathf.Max(0,deathTime-.15f);
                deathY=deathStartY+520*t-625*t*t;
                if(deathTime>.15f && deathY < -80f) { deathComplete=true;resultTime=0; }
                return;
            }
            resultTime+=Time.unscaledDeltaTime;
            if(resultTime>=1.8f) ConfirmResult();
            return;
        }
        if(Keyboard.current!=null)
        {
            if(!Keyboard.current.spaceKey.isPressed) jumpInputReleased=true;
            if(jumpInputReleased && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                // Preserve the input event's time if rendering delayed this frame.
                double age=System.Math.Max(0,UnityEngine.InputSystem.LowLevel.InputState.currentTime-Keyboard.current.lastUpdateTime);
                Jump(Rhythm!=null ? Rhythm.SongTime-age : (double?)null);
            }
        }
        if(Rhythm!=null)
        {
            Model.TickTo(System.Math.Max(0,Rhythm.SongTime));
            int beat=Rhythm.BeatCount;
            if(beat!=shownBeat) { shownBeat=beat;beatText="BEAT "+beat; }
        }
        RefreshHud();
        if(Model.Finished && Model.Won) DungeonAudio.Play(DungeonAudio.Cue.HackWin);
    }
    public void ConfirmResult()
    {
        if(!Active || !Model.Finished || (!Model.Won && !deathComplete)) return;
        int damage=Model.Damage;completedOutcome=!Model.Won?HackExecutionOutcome.Failed:Model.PerfectCount>=sessionSettings.enemiesToWin?HackExecutionOutcome.Perfect:HackExecutionOutcome.Success;completedQuality=AttackQuality(damage);Model=null;StopRhythm();
        if(backdrop!=null) backdrop.Restore();
        var callback=completed; completed=null; if(callback!=null) callback(damage);
    }
    void OnDisable() { StopRhythm();if(backdrop!=null) backdrop.Restore();Model=null;completed=null; }
    void OnDestroy() { StopRhythm(); }
    void OnGUI()
    {
        if(!Active) return;
        int savedDepth=GUI.depth;
        GUI.depth=-20;
        var savedMatrix=GUI.matrix; var savedColor=GUI.color;
        var savedBackground=GUI.backgroundColor; var savedContent=GUI.contentColor;
        bool savedEnabled=GUI.enabled;
        WristHackScreen.DrawFrame();
        WristHackScreen.ApplyContentMatrix();
        float impact=nextPopup>0||popups[0].caption!=null?Mathf.Clamp01(1-(Time.unscaledTime-popups[(nextPopup+popups.Length-1)%popups.Length].started)/.12f):0;
        GUI.matrix=GUI.matrix*Matrix4x4.Translate(new Vector3(Mathf.Sin(Time.unscaledTime*125)*impact*3,Mathf.Cos(Time.unscaledTime*103)*impact*2,0));
        GUI.enabled=true;
        if(label==null) { label=new GUIStyle(GUI.skin.label){fontSize=24}; button=new GUIStyle(GUI.skin.button){fontSize=24}; }
        Fill(new Rect(0,0,1000,570),new Color(.008f,.004f,.025f,1));
        GUI.Label(new Rect(25,12,650,35),"HACK_01 / ENEMY JUMP",label);
        GUI.Label(new Rect(800,12,190,35),comboText,label);
        Fill(new Rect(24,54,952,442),new Color(.15f,.45f,.85f)); Fill(new Rect(27,57,946,436),Color.black);

        if(parallax!=null)parallax.Draw(new Rect(27,120,946,340),Rhythm!=null?Rhythm.SongTime:Time.unscaledTime-backgroundStarted);
        GUI.Label(new Rect(50,72,700,35),damageText,label);
        GUI.Label(new Rect(50,108,880,30),"SPACE / JUMP: salta libremente y aterriza al pulso",label);
        if(!string.IsNullOrEmpty(Model.BounceFeedback)) GUI.Label(new Rect(50,458,880,30),Model.BounceFeedback,label);
        if(Rhythm!=null)
        {
            bool launchCue=!Model.Finished && Model.Y<.01f && System.Math.Abs(Rhythm.SongTime-(Model.UpcomingLanding-Model.GroundFlightSeconds))<.12;
            GUI.Label(new Rect(770,72,180,30),launchCue ? "SALTA / JUMP" : beatText,label);
            float pulse=1-Mathf.Min(Rhythm.Phase,1-Rhythm.Phase)*2;
            Fill(new Rect(748,80,8,8),new Color(.3f,.85f,1,.25f+pulse*.65f));
            Fill(new Rect(700,104,240,3),new Color(.15f,.3f,.45f,.5f));
            Fill(new Rect(700+Rhythm.Phase*236,102,4,7),new Color(.3f,.85f,1,.65f));
        }
        // Clip the defeat fall to the device's arena, not the dungeon or controls.
        float playerY=Model.Finished && !Model.Won ? (deathStarted ? deathY : Model.Y) : Model.Y;
        Texture2D pose=Model.Finished && !Model.Won ? deathSprite :
            (Model.Y>.001f || Model.Velocity>0 ? jumpSprite : playerSprite);
        if(pose==null) pose=playerSprite;
        GUI.BeginGroup(new Rect(27,120,946,340));
        if(!deathComplete && pose!=null && Event.current.type==EventType.Repaint)
        {
            const float spriteSize=68f;
            GUI.color=Color.white;
            GUI.DrawTexture(new Rect(23+EnemyJumpModel.PlayerX+(EnemyJumpModel.PlayerSize-spriteSize)*.5f,
                EnemyJumpLayout.GroundY-playerY-spriteSize,spriteSize,spriteSize),pose,ScaleMode.ScaleToFit,true);
        }
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(50,120,900,340));
        if(Event.current.type==EventType.Repaint)
        for(int i=0;i<Model.Enemies.Count;i++)
        {
            float x=Model.Enemies[i];
            if(x>=EnemyJumpModel.Width) continue;
            int id=Model.EnemyVisualIds[i];
            Texture2D frame=walkingEnemies[opponentKind][(Mathf.FloorToInt(Time.unscaledTime/.22f)+id)%2];
            GUI.color=Color.white;
            float size=Model.EnemyExtent*(frame!=null?(float)frame.width/frame.height:1);

            if(frame!=null) GUI.DrawTexture(new Rect(x+(Model.EnemyExtent-size)*.5f,EnemyJumpLayout.GroundY-Model.EnemyExtent,size,Model.EnemyExtent),frame,ScaleMode.StretchToFill,true);
            else Fill(new Rect(x,EnemyJumpLayout.GroundY-Model.EnemyExtent,Model.EnemyExtent,Model.EnemyExtent),new Color(1,.15f,.23f));
        }
        for(int n=0;n<popups.Length;n++){
            var stomp=popups[n];float age=Time.unscaledTime-stomp.started;if(stomp.caption==null||age<0||age>.42f)continue;
            var frame=walkingEnemies[opponentKind][stomp.visualId%2];if(frame==null)continue;
            float squash=Mathf.Clamp01(age/.1f);float height=Model.EnemyExtent*Mathf.Lerp(1,.16f,squash);float width=Model.EnemyExtent*(float)frame.width/frame.height*Mathf.Lerp(1,1.35f,squash);
            GUI.color=new Color(1,1,1,1-Mathf.Clamp01((age-.12f)/.3f));GUI.DrawTexture(new Rect(stomp.x+(Model.EnemyExtent-width)*.5f,EnemyJumpLayout.GroundY-height,width,height),frame,ScaleMode.StretchToFill,true);
        }
        if(popupStyle==null) { popupStyle=new GUIStyle(GUI.skin.label){fontSize=52,fontStyle=FontStyle.Bold,alignment=TextAnchor.MiddleCenter};popupStyle.normal.textColor=new Color(1,.3f,.65f); }
        for(int i=0;i<popups.Length;i++)
        {
            if(popups[i].caption==null) continue;
            float age=Time.unscaledTime-popups[i].started;
            if(age<0 || age>.85f) continue;
            GUI.color=new Color(1,1,1,1-age/.85f);
            GUI.Label(new Rect(EnemyJumpModel.PlayerX-65,EnemyJumpLayout.GroundY-popups[i].height-105-age*65,180,80),popups[i].caption,popupStyle);
        }
        GUI.color=Color.white;
        GUI.EndGroup();
        if(Model.Finished && (Model.Won || deathComplete))
        {
            Fill(new Rect(210,200,580,105),Color.black);
            GUI.Label(new Rect(235,213,540,35),Model.Reason,label);
            GUI.Label(new Rect(235,250,540,35),resultText,label);
            if(GUI.Button(new Rect(350,513,300,40),"VOLVER AL COMBATE",button)) ConfirmResult();
        }
        else if(!Model.Finished && GUI.Button(new Rect(370,513,260,40),"JUMP / SPACE",button)) Jump();
        GUI.matrix=savedMatrix; GUI.color=savedColor; GUI.backgroundColor=savedBackground; GUI.contentColor=savedContent; GUI.enabled=savedEnabled; GUI.depth=savedDepth;
    }
    static void Fill(Rect rect, Color color) { if(Event.current.type!=EventType.Repaint) return; GUI.color=color; GUI.DrawTexture(rect,Texture2D.whiteTexture); GUI.color=Color.white; }
}
