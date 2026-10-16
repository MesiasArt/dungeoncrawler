using UnityEngine;

// Shared safety notice. A recovered older save must never look like a normal load.
public sealed class DungeonSaveRecoveryNotice:MonoBehaviour {
 static DungeonSaveRecoveryNotice current;
 public static bool Active {get{return current!=null;}}
 string notice;
 void Update(){var keyboard=UnityEngine.InputSystem.Keyboard.current;var gamepad=UnityEngine.InputSystem.Gamepad.current;if(keyboard!=null&&(keyboard.enterKey.wasPressedThisFrame||keyboard.numpadEnterKey.wasPressedThisFrame||keyboard.spaceKey.wasPressedThisFrame)||gamepad!=null&&gamepad.buttonSouth.wasPressedThisFrame)Destroy(gameObject);}
 public static void Show(string value){if(string.IsNullOrEmpty(value))return;if(current==null){var root=new GameObject("Save recovery notice");DontDestroyOnLoad(root);current=root.AddComponent<DungeonSaveRecoveryNotice>();}current.notice=value;}
 void OnGUI(){HelmetCRTLayerMask.BeginGUI();var matrix=GUI.matrix;var color=GUI.color;bool enabled=GUI.enabled;int depth=GUI.depth;try{float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));GUI.depth=-31000;GUI.color=Color.white;GUI.enabled=true;float width=Screen.width/scale,height=Screen.height/scale;var panel=new Rect(width/2-330,height/2-110,660,220);NeonDungeonUI.ApplyFont();NeonDungeonUI.Panel(panel);NeonDungeonUI.Label(new Rect(panel.x+24,panel.y+20,612,115),notice);if(NeonDungeonUI.MenuAction(new Rect(panel.x+170,panel.y+155,320,44),"ENTENDIDO",true))Destroy(gameObject);var e=Event.current;if(e.type==EventType.KeyDown&&(e.keyCode==KeyCode.Return||e.keyCode==KeyCode.KeypadEnter||e.keyCode==KeyCode.Space)){Destroy(gameObject);e.Use();}else if(e.isMouse&&e.type!=EventType.MouseMove)e.Use();}finally{GUI.matrix=matrix;GUI.color=color;GUI.enabled=enabled;GUI.depth=depth;}}
 void OnDestroy(){if(current==this)current=null;}
}
