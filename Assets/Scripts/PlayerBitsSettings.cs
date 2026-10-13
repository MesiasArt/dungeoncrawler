using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Bits de GamerBoy")]
public class PlayerBitsSettings:ScriptableObject {
 [Min(1)] public int maximumBits=100;
 public static PlayerBitsSettings Current {get{return Resources.Load<PlayerBitsSettings>("Combat/PlayerBitsSettings");}}
}
