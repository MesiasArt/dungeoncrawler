using UnityEngine;
public enum EnemyBarkCategory { CombatStart, Hurt, Attack, Special, Defeat, ExtremeCorruption, LowHealth }
[System.Serializable]
public class EnemyBarkRule
{
 public EnemyBarkCategory category;
 [Range(0,1)] public float probability;
 public TextAsset phrases;
}
[System.Serializable]
public class EnemyBarkProfile
{
 public string enemyId;
 public EnemyBarkRule[] rules;
}
[CreateAssetMenu(menuName="Virtual Dungeon/Enemy Barks")]
public class EnemyBarkSettings : ScriptableObject
{
 [Min(.5f)] public float displaySeconds=3;
 [Min(0)] public float lettersPerSecond=35;
 [Min(12)] public int textFontSize=18;
 [Min(0)] public float normalCooldown=1.5f;
 [Min(1)] public int recentPhraseCount=5;
 [Tooltip("Offset from the enemy head and size, on the 1280 x 720 reference canvas. The artwork points left toward the enemy.")]
 public Rect displayRect=new Rect(16,-60,420,128);
 public EnemyBarkProfile[] enemies;
}
