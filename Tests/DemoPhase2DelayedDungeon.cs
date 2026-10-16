#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
public sealed class DemoPhase2DelayedDungeon:MonoBehaviour {
 IEnumerator Start(){yield return new WaitForSecondsRealtime(.4f);new GameObject("Delayed dungeon").AddComponent<DungeonPrototype>();}
}
#endif
