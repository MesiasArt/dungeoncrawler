using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Visual Settings")]
public sealed class DungeonVisualSettings : ScriptableObject
{
 [Header("Espacio para enemigos (se aplica al iniciar la partida)")]
 public bool autoExpandEnemySpace=true;
 [Min(3)] public float minimumCorridorWidth=3;
 [Min(3)] public float minimumCorridorHeight=3;
 [Min(3)] public float minimumCombatHeight=4.5f;
 [Min(.1f)] public float enemySpaceMargin=.4f;
 [Header("Antorchas")]
 public Color torchColor=new Color(1,.02f,.45f);
 [Min(0)] public float torchIntensity=2;
 [Min(.1f)] public float torchRange=4.5f;
 [Range(0,.5f)] public float torchFlicker=.07f;
 [Header("Reflejos")]
 [Range(0,1)] public float floorReflection=.23f;
 [Range(0,1)] public float ceilingReflection=.16f;
 [Range(0,1)] public float reflectionBottomVisibility=.55f;
 [Header("Fondo de HACK JUMP")]
 [Range(0,1)] public float jumpBackgroundBrightness=.8f;
 public static DungeonVisualSettings Current { get { if(current==null)current=Resources.Load<DungeonVisualSettings>("Exploration/DungeonVisualSettings");return current; } }
 static DungeonVisualSettings current;
}
