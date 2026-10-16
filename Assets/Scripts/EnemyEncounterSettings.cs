using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Enemy encounters")]
public class EnemyEncounterSettings : ScriptableObject
{
    [Header("Director de encuentros")]
    public bool encounterDirectorEnabled=true;
    [Min(1)]public int minimumSpawnNavigationSteps=6,preferredMaximumSpawnSteps=12;
    [Min(1)] public int minimumMapSpacing=4;
    [Min(0)]public int postCombatRestSteps=6;
    [Range(1,2)]public int desiredEnemiesPerSector=2;
    [Min(2)]public int sectorNavigationRadius=6;
    [Min(1)]public int directorCheckEverySteps=2,longQuietWalkSteps=16;
    [Min(.1f)]public float respawnStaggerSeconds=1.2f;
    [Min(0)]public int doorwayBufferSteps=2,safeAreaBufferSteps=2;
    public bool spawnOutsideView=true;

    [Header("Vida y dano de enemigos nuevos")]
    [Min(1)] public int tapakuHP=10;
    [Min(0)] public int tapakuDamage=3;
    [Min(1)] public int hongoHP=16;
    [Min(0)] public int hongoDamage=2;
    [Min(1)] public int soldierHP=13;
    [Min(0)] public int soldierDamage=2;
    [Min(1)] public int bigTapakuHP=24;
    [Min(0)] public int bigTapakuDamage=4;
    [Tooltip("Dano POR GOLPE del especial, no el total.")]
    [Min(0)] public int bigTapakuSpecialDamage=1;
    [Min(1)] public int bigTapakuSpecialHits=3;
    [Min(.03f)] public float bigTapakuSpecialHitInterval=.2f;
    [Min(1)] public int bigTapakuSpecialEvery=3;
    public bool spawnBigTapaku=true;
    public bool spawnMakubas=true;
    public bool spawnGranMakuba=true;
    [Header("Exploracion por instancia (Legacy conserva los valores anteriores)")]
    [Min(1)] public int explorationDetectionSteps=4;
    [Min(0)] public int explorationMemoryTurns=3;
    [Min(1)] public int explorationLeashSteps=6;
    [Header("Apariciones y movimiento")]
    [HideInInspector, Min(1)] public float respawnSeconds=30; // Legacy migration; Zone settings are authoritative.
    [Range(1,6)] public int detectionSteps=2;
    [Range(1,3)] public int minimumEnemies=1;
    [Range(1,3)] public int maximumEnemies=3;
    [Min(.05f)] public float moveSeconds=.18f;
    [HideInInspector, Range(3,20)] public int maximumMapEnemies=9; // Legacy migration.
}
