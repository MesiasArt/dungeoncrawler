using UnityEngine;

[CreateAssetMenu(menuName="Dungeon/Enemy Jump Settings")]
public class EnemyJumpSettings : ScriptableObject
{
    [Min(1)] public int enemiesToWin = 10;
    [Min(30)] public float enemySpeed = 260;
    [Min(100)] public float gravity = 2000;
    [Min(100)] public float jumpForce = 820;
    [Min(100)] public float bounceForce = 950;
    [Tooltip("Small automatic bounce when the landing press is missed.")]
    [Min(0)] public float weakBounceForce = 180;
    [Tooltip("Press Space/JUMP this many seconds before or after landing for a high bounce.")]
    [Range(.03f,.2f)] public float landingPressWindow = .09f;
    [Min(80)] public float enemySpacing = 260;
    [Range(0, .15f)] public float spacingVariation = .08f;
    [Range(0, .05f)] public float speedPerCombo = .018f;
}
