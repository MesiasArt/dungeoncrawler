using UnityEngine;using UnityEngine.InputSystem;using System.Collections;
public sealed class DungeonDecorationShowcase:MonoBehaviour {
 DungeonPrototype dungeon;
 #if UNITY_EDITOR
 string previousSavePath;void Awake(){previousSavePath=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath="Tests/DecorationShowcaseFixture.json";}void OnDestroy(){DungeonSaveStore.editorTestPath=previousSavePath;}
 #endif
 IEnumerator Start(){dungeon=GetComponent<DungeonPrototype>();while(!dungeon.LaboratoryReady)yield return null;yield return null;Show(0);}
 public void Show(int index){var positions=new[]{new Vector2Int(3,1),new Vector2Int(9,5),new Vector2Int(11,18)};dungeon.PrepareDecorationPreview(positions[Mathf.Clamp(index,0,2)],0);}
 void Update(){var k=Keyboard.current;if(k==null||dungeon==null||!dungeon.LaboratoryReady)return;if(k.digit1Key.wasPressedThisFrame)Show(0);if(k.digit2Key.wasPressedThisFrame)Show(1);if(k.digit3Key.wasPressedThisFrame)Show(2);}
 void OnGUI(){GUI.Label(new Rect(15,Screen.height-35,Screen.width-30,30),"DECORACIÓN / PRUEBA SEGURA — 1 pasillo corto · 2 pasillo largo · 3 sala amplia · WASD recorrer");}
}
