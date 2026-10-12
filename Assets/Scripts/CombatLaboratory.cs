using UnityEngine;
using System.Collections;
[RequireComponent(typeof(DungeonPrototype))]
public sealed class CombatLaboratory:MonoBehaviour {
 public string speciesId="Makong";
 [Min(1)] public int enemyLevel=1,gamerLevel=8;
 [Range(0,2)] public int companions=2;
 [Range(0,3)] public int equipmentPreset;
 public bool MenuOpen {get;private set;}=true;
 DungeonPrototype dungeon;Vector2 scroll;
 #if UNITY_EDITOR
 string previousSavePath;
 #endif
 IEnumerator Start(){dungeon=GetComponent<DungeonPrototype>();
 #if UNITY_EDITOR
 previousSavePath=DungeonSaveStore.editorTestPath;DungeonSaveStore.editorTestPath=System.IO.Path.Combine(Application.persistentDataPath,"Codebound-laboratory.json");
 #endif
 while(!dungeon.LaboratoryReady)yield return null;dungeon.InitializeLaboratory();}
 public void Fight(){if(dungeon==null||!dungeon.LaboratoryReady)return;MenuOpen=false;dungeon.StartLaboratoryCombat(speciesId,enemyLevel,gamerLevel,companions,equipmentPreset);}
 public void Choose(){if(dungeon==null||!dungeon.LaboratoryReady)return;dungeon.ResetLaboratory(gamerLevel);MenuOpen=true;}
 void OnDestroy(){
 #if UNITY_EDITOR
 DungeonSaveStore.editorTestPath=previousSavePath;
 #endif
 }
 void OnGUI(){if(dungeon==null||!dungeon.LaboratoryReady)return;var matrix=GUI.matrix;var color=GUI.color;var background=GUI.backgroundColor;var content=GUI.contentColor;bool enabled=GUI.enabled;int depth=GUI.depth;
 float scale=Mathf.Min(Screen.width/1024f,Screen.height/720f);GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));float width=Screen.width/scale,height=Screen.height/scale;GUI.depth=-150;GUI.enabled=true;GUI.color=Color.white;GUI.contentColor=new Color(.65f,.9f,1);GUI.backgroundColor=new Color(.12f,.03f,.23f);
 try {if(!MenuOpen){GUILayout.BeginArea(new Rect(width-370,height-88,350,82),GUI.skin.box);GUILayout.BeginHorizontal();if(GUILayout.Button("Elegir enemigo"))Choose();if(GUILayout.Button("Reiniciar"))Fight();GUILayout.EndHorizontal();GUI.enabled=!dungeon.LaboratoryBusy&&dungeon.LaboratoryBattle;if(speciesId=="Makong"){GUILayout.BeginHorizontal();if(GUILayout.Button("Probar super salto"))dungeon.LaboratoryMakongSpecial(false);if(GUILayout.Button("Probar devorar"))dungeon.LaboratoryMakongSpecial(true);GUILayout.EndHorizontal();}GUILayout.EndArea();return;}
 GUI.color=new Color(0,0,0,.88f);GUI.DrawTexture(new Rect(0,0,width,height),Texture2D.whiteTexture);GUI.color=Color.white;
 GUILayout.BeginArea(new Rect(width/2-280,height/2-295,560,590),GUI.skin.box);GUILayout.Label("CODEBOUND / LABORATORIO DE COMBATE",new GUIStyle(GUI.skin.label){fontSize=21,alignment=TextAnchor.MiddleCenter});GUILayout.Space(10);GUILayout.Label("Selecciona un enemigo registrado:");scroll=GUILayout.BeginScrollView(scroll,GUILayout.Height(200));var catalog=EnemyCharacterCatalog.Current;foreach(var enemy in catalog.enemies){if(enemy==null||System.Array.IndexOf(EnemyRoster.ResourceIds,enemy.id)<0&&enemy.id!="DevotoCorrompido")continue;if(GUILayout.Button((speciesId==enemy.id?"> ":"")+enemy.character+" / "+enemy.category))speciesId=enemy.id;}GUILayout.EndScrollView();
 GUILayout.Label("Nivel del enemigo: "+enemyLevel);enemyLevel=Mathf.RoundToInt(GUILayout.HorizontalSlider(enemyLevel,1,50));GUILayout.Label("Nivel de GamerBoy: "+gamerLevel);gamerLevel=Mathf.RoundToInt(GUILayout.HorizontalSlider(gamerLevel,1,PlayerLevelSettings.Current.maximumLevel));GUILayout.Label("Equipo de prueba (solo laboratorio)");equipmentPreset=GUILayout.SelectionGrid(equipmentPreset,new[]{"Inicial","Guante MK-II","Guante + camiseta","Tres piezas nuevas"},2);GUILayout.Label("Compañeros adicionales: "+companions);companions=Mathf.RoundToInt(GUILayout.HorizontalSlider(companions,0,2));GUILayout.Label("0 = combate individual. Makong necesita un compañero para devorar.");GUILayout.Space(8);if(GUILayout.Button("INICIAR COMBATE",GUILayout.Height(40)))Fight();GUILayout.Label("Usa el combate habitual: ATTACK, JUMP y HACKS disponibles.\nLas partidas personales no se modifican.");GUILayout.EndArea();
 }finally{GUI.matrix=matrix;GUI.color=color;GUI.backgroundColor=background;GUI.contentColor=content;GUI.enabled=enabled;GUI.depth=depth;}}
}
