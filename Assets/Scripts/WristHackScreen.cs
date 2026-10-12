using UnityEngine;

// Both hack selection and its game share the same screen aperture.
public static class WristHackScreen
{
    static Texture2D arm;
    public static Texture Background { get; set; }
    public static readonly Color Text = new Color(.65f,.85f,1);
    public static void DrawFrame()
    {
        if(arm==null) arm=Resources.Load<Texture2D>("UI/HackArm");
        GUI.matrix=Matrix4x4.identity;GUI.color=Color.white;
        if(Event.current.type!=EventType.Repaint) return;
        if(Background!=null) GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Background,ScaleMode.StretchToFill,false);
        float sx=Screen.width/1672f,sy=Screen.height/941f;
        // Behind the frame: cover only the display hole, leaving exterior alpha clear.
        GUI.color=new Color(.008f,.004f,.025f,1);
        GUI.DrawTexture(new Rect(430*sx,163*sy,915*sx,600*sy),Texture2D.whiteTexture);
        GUI.color=Color.white;
        if(arm!=null) GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),arm,ScaleMode.StretchToFill,true);
    }
    public static void ApplyContentMatrix()
    {
        float sx=Screen.width/1672f,sy=Screen.height/941f;
        Rect safe=new Rect(465*sx,185*sy,850*sx,550*sy);
        float scale=Mathf.Min(safe.width/1000f,safe.height/570f);
        GUI.matrix=Matrix4x4.TRS(new Vector3(safe.center.x-500*scale,safe.center.y-285*scale,0),Quaternion.identity,new Vector3(scale,scale,1));
        GUI.color=Color.white;GUI.contentColor=Text;GUI.backgroundColor=new Color(.12f,.05f,.3f,.95f);
    }
}