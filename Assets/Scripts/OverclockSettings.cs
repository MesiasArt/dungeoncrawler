using UnityEngine;
using UnityEngine.InputSystem;
public enum OverclockPadButton { South, East, West, North, LeftShoulder, RightShoulder }
[CreateAssetMenu(menuName="Virtual Dungeon/OVERCLOCK")]
public class OverclockSettings:ScriptableObject {
 [Min(.1f)] public float durationSeconds=5;
 [Min(1)] public int requiredPresses=20;
 [Min(0)] public float successQuality=2.5f,failureQuality=.5f;
 public Key keyboardKey=Key.Space;
 public OverclockPadButton gamepadButton=OverclockPadButton.South;
 [Range(.2f,2f)] public float resultDisplaySeconds=1.1f;
 [Range(0,1)] public float feedbackStrength=1;
 public bool playSounds=true;
 public DungeonAudio.Cue pulseSound=DungeonAudio.Cue.Confirm,successSound=DungeonAudio.Cue.HackWin,failureSound=DungeonAudio.Cue.HackFail;
 public static OverclockSettings Current {get{return Resources.Load<OverclockSettings>("Combat/OverclockSettings");}}
}
