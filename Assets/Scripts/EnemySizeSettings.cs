using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Enemy Sizes")]
public sealed class EnemySizeSettings : ScriptableObject
{
 [Header("Tamano individual (1 = original). Se guarda para el juego y la galeria.")]
 [Min(.05f)] public float tapaku=1;
 [Min(.05f)] public float hongo=1;
 [Min(.05f)] public float soldier=1;
 [Min(.05f)] public float bigTapaku=1.2f;
 [Min(.05f)] public float makubaLanza=1.1f,makubaArco=1.1f,granMakuba=1.8f,mimic=1,makong=1.6f;
 public float ScaleFor(int kind){switch(kind){case 1:return Mathf.Max(.05f,hongo);case 2:return Mathf.Max(.05f,soldier);case 3:return Mathf.Max(.05f,bigTapaku);case 4:return Mathf.Max(.05f,makubaLanza);case 5:return Mathf.Max(.05f,makubaArco);case 6:return Mathf.Max(.05f,granMakuba);case 7:return Mathf.Max(.05f,mimic);case 8:return Mathf.Max(.05f,makong);default:return Mathf.Max(.05f,tapaku);}}

 static EnemySizeSettings current;
 public static EnemySizeSettings Current {get{if(current==null)current=Resources.Load<EnemySizeSettings>("Exploration/EnemySizeSettings");return current;}}
}
