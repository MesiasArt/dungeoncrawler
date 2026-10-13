using UnityEngine;
public sealed class DungeonDecorationMotion:MonoBehaviour {
 public Transform rotor;public float degreesPerSecond=35;
 void Update(){if(rotor!=null)rotor.Rotate(0,0,degreesPerSecond*Time.deltaTime,Space.Self);}
}
