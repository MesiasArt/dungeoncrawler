using UnityEngine;
using UnityEditor;
using System.IO;

// Edits the real Resources assets, with normal Inspector Undo and persistence.
public sealed class VirtualDungeonEditorWindow : EditorWindow
{
 static readonly string[] Names={"Audio: musica y SFX","Enemigos: apariciones","Ambiente: horizonte y fondo","Antorchas, reflejos y fondo JUMP","Interfaz: posiciones y tamanos","Dialogos de enemigos","HACK JUMP: ritmo y saltos","Inventario y vida del jugador","Puerta de salida","Tamanos de enemigos","Reglas de suerte para HACK","Personajes: estadisticas y equipo","HACKS: JUMP y OVERCLOCK"};
 static readonly string[] Paths={"Audio/DungeonAudioSettings","Exploration/EnemyEncounterSettings","Exploration/DungeonAtmosphereSettings","Exploration/DungeonVisualSettings","UI/DungeonUILayout","Combat/Barks/EnemyBarkSettings","Combat/EnemyJumpSettings","Combat/CombatInventorySettings","Exploration/ExitDoorSettings","Exploration/EnemySizeSettings","Combat/CombatAccuracySettings","Combat/PlayerCharacterSettings"};
 int section;Vector2 scroll;UnityEditor.Editor inspector;Object current;
 [MenuItem("Editor/Panel de Virtual Dungeon",false,2000)] public static void Open(){GetWindow<VirtualDungeonEditorWindow>("Virtual Dungeon").Show();}
 [MenuItem("Editor/Audio - musica y SFX",false,2010)] static void Audio(){ShowSection(0);}
 [MenuItem("Editor/Enemigos - apariciones",false,2011)] static void Enemies(){ShowSection(1);}
 [MenuItem("Editor/Ambiente - horizonte",false,2012)] static void Atmosphere(){ShowSection(2);}
 [MenuItem("Editor/Escenario - espacio y techo",false,2023)] static void Space(){ShowSection(3);}
 [MenuItem("Editor/Antorchas y reflejos",false,2013)] static void Visuals(){ShowSection(3);}
 [MenuItem("Editor/Interfaz y barras",false,2014)] static void UI(){ShowSection(4);}
 [MenuItem("Editor/Dialogos y frases",false,2015)] static void Barks(){ShowSection(5);}
 [MenuItem("Editor/HACK JUMP",false,2016)] static void Jump(){ShowSection(6);}
 [MenuItem("Editor/Inventario y vida",false,2017)] static void Inventory(){ShowSection(7);}
 [MenuItem("Editor/Puerta de salida",false,2018)] static void Door(){ShowSection(8);}
 [MenuItem("Editor/Tamanos de enemigos",false,2019)] static void Sizes(){ShowSection(9);}
 [MenuItem("Editor/Reglas de suerte para HACK",false,2021)] static void Accuracy(){ShowSection(10);}
 [MenuItem("Editor/Personaje y equipo",false,2022)] static void Character(){CharacterStatisticsEditor.Open();}
 static void ShowSection(int index){var window=GetWindow<VirtualDungeonEditorWindow>("Virtual Dungeon");window.section=index;window.Show();}
 [InitializeOnLoadMethod] static void Initialize(){EditorApplication.delayCall+=EnsureVisualAsset;}
 static void EnsureVisualAsset(){const string characterPath="Assets/Resources/Combat/PlayerCharacterSettings.asset";if(AssetDatabase.LoadAssetAtPath<PlayerCharacterSettings>(characterPath)==null)AssetDatabase.CreateAsset(CreateInstance<PlayerCharacterSettings>(),characterPath);const string accuracyPath="Assets/Resources/Combat/CombatAccuracySettings.asset";if(AssetDatabase.LoadAssetAtPath<CombatAccuracySettings>(accuracyPath)==null)AssetDatabase.CreateAsset(CreateInstance<CombatAccuracySettings>(),accuracyPath);const string sizePath="Assets/Resources/Exploration/EnemySizeSettings.asset";if(AssetDatabase.LoadAssetAtPath<EnemySizeSettings>(sizePath)==null)AssetDatabase.CreateAsset(CreateInstance<EnemySizeSettings>(),sizePath);const string path="Assets/Resources/Exploration/DungeonVisualSettings.asset";if(AssetDatabase.LoadAssetAtPath<DungeonVisualSettings>(path)!=null)return;Directory.CreateDirectory(Path.GetDirectoryName(path));AssetDatabase.CreateAsset(CreateInstance<DungeonVisualSettings>(),path);AssetDatabase.SaveAssets();}
 void OnDisable(){if(inspector!=null)DestroyImmediate(inspector);inspector=null;current=null;}
 void OnEnable(){inspector=null;current=null;minSize=new Vector2(520,580);}
 void OnGUI(){
  EditorGUILayout.LabelField("EDITOR DE VIRTUAL DUNGEON",EditorStyles.boldLabel);
  EditorGUILayout.HelpBox("Estos ajustes se guardan en el proyecto, incluso si los editas durante Play Mode. Undo: Ctrl+Z. Algunos cambios de combate o aparicion se aplican al siguiente encuentro o partida.",MessageType.Info);
  section=EditorGUILayout.Popup("Seccion",section,Names);
  if(section==12){EditorGUILayout.HelpBox("JUMP y OVERCLOCK: daño, minijuegos, desbloqueos y costes de BP.",MessageType.Info);if(GUILayout.Button("Abrir pestañas de HACKS"))HackSettingsWindow.Open();return;}
  var asset=AssetDatabase.LoadAssetAtPath<Object>("Assets/Resources/"+Paths[section]+".asset");
  if(current!=asset||inspector==null){if(inspector!=null)DestroyImmediate(inspector);current=asset;inspector=asset!=null?UnityEditor.Editor.CreateEditor(asset):null;}
  if(asset==null){EditorGUILayout.HelpBox("No se encontro el archivo de ajustes de esta seccion.",MessageType.Warning);return;}
  if((section==1||section==7||section==10||section==11)&&GUILayout.Button("Editar estadísticas por personaje"))CharacterStatisticsEditor.Open();
  if(section==10)EditorGUILayout.HelpBox("Las estadisticas individuales se editan en Personajes. Aqui solo se configura la activacion de suerte y las reglas de HACK; JUMP siempre esta excluido.",MessageType.None);
  if(section==9&&GUILayout.Button("Abrir galeria con regla en pies"))EnemySizeGalleryEditor.Open();
  if(section==0)EditorGUILayout.HelpBox("Music Volume = musica. Effects Volume = todos los SFX. Dialogue Volume y Dialogue Advance Volume = sonidos del texto. Dungeon Music y Combat Music permiten elegir las canciones.",MessageType.None);
  if(section==1)EditorGUILayout.HelpBox("Respawn Seconds: tiempo para reaparecer. Detection Steps: distancia de persecucion. Minimum/Maximum Enemies: cantidad en combate. Vida, dano, defensa y suerte se editan en Personajes.",MessageType.None);
  if(section==4)EditorGUILayout.HelpBox("Player Avatar y Player Health se editan por separado. Enemy Health: posicion y tamano de la barra superior del enemigo seleccionado. Las posiciones usan una pantalla de referencia de 1280 x 720.",MessageType.None);
  if(section==5)EditorGUILayout.HelpBox("Despliega Enemies y Rules para probabilidades y archivos de frases. Pulsa un archivo de Phrases para encontrarlo. Los dialogos de combate son automaticos.",MessageType.None);
  EditorGUILayout.BeginHorizontal();
  if(GUILayout.Button("Ver archivo en Inspector")){Selection.activeObject=asset;EditorGUIUtility.PingObject(asset);}
  if(GUILayout.Button("Guardar ajustes"))AssetDatabase.SaveAssets();
  EditorGUILayout.EndHorizontal();
  scroll=EditorGUILayout.BeginScrollView(scroll);
  if(section==1){var settings=new SerializedObject(asset);settings.Update();foreach(string key in new[]{"bigTapakuSpecialEvery","spawnBigTapaku","spawnMakubas","spawnGranMakuba","detectionSteps","minimumEnemies","maximumEnemies","moveSeconds"})EditorGUILayout.PropertyField(settings.FindProperty(key));settings.ApplyModifiedProperties();EditorGUILayout.HelpBox("Niveles, pesos, máximo simultáneo y respawn se configuran en Balance de combate / Zona 1.",MessageType.Info);if(GUILayout.Button("Balance y apariciones por zona"))EnemyProgressionEditor.Open();}else if(section==10){var settings=new SerializedObject(asset);settings.Update();foreach(string key in new[]{"enabledLuck","hacks","unlistedHacksUseLuck","unlistedHackMissPercent"})EditorGUILayout.PropertyField(settings.FindProperty(key),true);settings.ApplyModifiedProperties();}else inspector.OnInspectorGUI();
  EditorGUILayout.Space();EditorGUILayout.LabelField("Archivos y dibujos",EditorStyles.boldLabel);
  if(GUILayout.Button("Abrir carpeta de sprites y PSD"))EditorUtility.RevealInFinder("Assets/sprites");
  if(GUILayout.Button("Abrir frases de enemigos"))EditorUtility.RevealInFinder("Assets/Resources/Combat/Barks");
  if(GUILayout.Button("Abrir sonidos SFX"))EditorUtility.RevealInFinder("Assets/Resources/Audio/Temporary");
  EditorGUILayout.EndScrollView();
 }
}
