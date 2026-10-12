using UnityEngine;

[CreateAssetMenu(menuName="Dungeon/Enemy Jump Settings")]
public class EnemyJumpSettings : ScriptableObject
{
    [Header("Jump musical calibration")]
    [Tooltip("Approximate tempo of Arcade Showdown; adjust against the audible pulse.")]
    [Range(60,240)] public float bpm=145;
    [Tooltip("Seconds from the start of the original song to beat zero. Positive delays the grid.")]
    [Range(-2,2)] public float musicOffsetSeconds=0;
    [Tooltip("Extra judgement calibration for display/input latency. Positive shifts accepted landings later.")]
    [Range(-.25f,.25f)] public float judgementOffsetSeconds=0;
    [Tooltip("Half-window in seconds around the scheduled landing beat.")]
    [Range(.01f,.15f)] public float perfectWindowSeconds=.065f;
    [Range(.02f,.25f)] public float goodWindowSeconds=.125f;
    [Tooltip("Each enemy reaches the player on this many beats; high rebounds match this interval.")]
    [Range(1,4)] public int beatsPerEnemy=2;
    [Tooltip("Two beats gives the opening jump more height and airtime; one beat is a shorter hop.")]
    [Range(1,2)] public int groundJumpBeats=2;
    [Min(4)] public int firstEnemyBeat=8;
    [Range(.2f,1)] public float scheduleLeadSeconds=.5f;
    [Header("Tamano relativo a la galeria de enemigos")]
    public bool matchEnemyReferenceSize=true;
    [Min(32)] public float referenceEnemyPixels=80;
    [Range(32,180)] public float minimumEnemyPixels=48,maximumEnemyPixels=180;
    [Header("Existing Jump mechanics")]
    [Min(1)] public int enemiesToWin = 10;
    [Min(30)] public float enemySpeed = 260;
    [Min(100)] public float gravity = 2000;
    [Min(100)] public float jumpForce = 820;
    [Min(100)] public float bounceForce = 950;
    [Tooltip("Small automatic bounce when the landing press is missed.")]
    [Min(0)] public float weakBounceForce = 180;
    [Tooltip("Press Space/JUMP this many seconds before or after landing for a high bounce.")]
    [Range(.03f,.2f)] public float landingPressWindow = .14f;
    [Min(80)] public float enemySpacing = 260;
    [Range(0, .15f)] public float spacingVariation = .08f;
    [Range(0, .05f)] public float speedPerCombo = .018f;
}
