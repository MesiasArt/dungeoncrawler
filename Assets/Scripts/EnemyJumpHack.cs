using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyJumpHack : CombatHack
{
    public override string DisplayName { get { return "Jump"; } }
    public EnemyJumpSettings settings;
    public EnemyJumpModel Model { get; private set; }
    public override bool Active { get { return Model!=null; } }
    Action<int> completed;
    float resultTime;
    bool jumpInputReleased;
    GUIStyle label, button;
    public override void Begin(Action<int> callback)
    {
        if(Active) return;
        if(settings==null) settings=Resources.Load<EnemyJumpSettings>("Combat/EnemyJumpSettings");
        if(settings==null) settings=ScriptableObject.CreateInstance<EnemyJumpSettings>();
        Model=new EnemyJumpModel(settings,Environment.TickCount); completed=callback; resultTime=0; jumpInputReleased=false;
    }
    void Update()
    {
        if(!Active) return;
        if(Model.Finished) { resultTime+=Time.unscaledDeltaTime; if(resultTime>=1.8f) ConfirmResult(); return; }
        if(Keyboard.current!=null)
        {
            if(!Keyboard.current.spaceKey.isPressed) jumpInputReleased=true;
            if(jumpInputReleased && Keyboard.current.spaceKey.wasPressedThisFrame) Model.Jump();
        }
        Model.Tick(Mathf.Min(Time.unscaledDeltaTime,.1f));
    }
    public void ConfirmResult()
    {
        if(!Active || !Model.Finished) return;
        int damage=Model.Damage; Model=null;
        var callback=completed; completed=null; if(callback!=null) callback(damage);
    }
    void OnGUI()
    {
        if(!Active) return;
        GUI.depth=-20;
        var savedMatrix=GUI.matrix; var savedColor=GUI.color;
        var savedBackground=GUI.backgroundColor; var savedContent=GUI.contentColor;
        bool savedEnabled=GUI.enabled;
        float scale=Mathf.Min(Screen.width/1000f,Screen.height/570f)*.92f;
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1000*scale)/2,(Screen.height-570*scale)/2,0),Quaternion.identity,Vector3.one*scale);
        GUI.enabled=true; GUI.color=Color.white; GUI.contentColor=Color.white; GUI.backgroundColor=Color.black;
        if(label==null) { label=new GUIStyle(GUI.skin.label){fontSize=20}; button=new GUIStyle(GUI.skin.button){fontSize=20}; }
        Fill(new Rect(0,0,1000,570),new Color(.035f,.035f,.035f,1));
        GUI.Label(new Rect(25,12,650,35),"HACK_01 / ENEMY JUMP",label);
        GUI.Label(new Rect(800,12,190,35),"COMBO x "+Model.Combo.ToString("00"),label);
        Fill(new Rect(24,54,952,442),Color.white); Fill(new Rect(27,57,946,436),Color.black);
        GUI.Label(new Rect(50,72,700,35),"DAMAGE: "+Model.Damage.ToString("00")+" / "+Mathf.Max(1,settings.enemiesToWin).ToString("00"),label);
        GUI.Label(new Rect(50,108,880,30),"Pulsa SPACE / JUMP justo al pisar: rebote alto",label);
        if(!string.IsNullOrEmpty(Model.BounceFeedback)) GUI.Label(new Rect(50,458,880,30),Model.BounceFeedback,label);
        // The floor is invisible at local y=440. Logical arena coordinates remain resolution independent.
        Fill(new Rect(50+EnemyJumpModel.PlayerX,440-Model.Y-EnemyJumpModel.PlayerSize,EnemyJumpModel.PlayerSize,EnemyJumpModel.PlayerSize),Color.white);
        GUI.BeginGroup(new Rect(50,120,900,340));
        foreach(float x in Model.Enemies) if(x<EnemyJumpModel.Width)
            Fill(new Rect(x,320-EnemyJumpModel.EnemySize,EnemyJumpModel.EnemySize,EnemyJumpModel.EnemySize),new Color(1,.15f,.23f));
        GUI.EndGroup();
        if(Model.Finished)
        {
            Fill(new Rect(210,200,580,105),Color.black);
            GUI.Label(new Rect(235,213,540,35),Model.Reason,label);
            GUI.Label(new Rect(235,250,540,35),"DAÑO CONFIRMADO: "+Model.Damage,label);
            if(GUI.Button(new Rect(350,513,300,40),"VOLVER AL COMBATE",button)) ConfirmResult();
        }
        else if(GUI.Button(new Rect(370,513,260,40),"JUMP / SPACE",button)) Model.Jump();
        GUI.matrix=savedMatrix; GUI.color=savedColor; GUI.backgroundColor=savedBackground; GUI.contentColor=savedContent; GUI.enabled=savedEnabled;
    }
    static void Fill(Rect rect, Color color) { GUI.color=color; GUI.DrawTexture(rect,Texture2D.whiteTexture); GUI.color=Color.white; }
}
