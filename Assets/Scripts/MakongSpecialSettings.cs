using UnityEngine;
[CreateAssetMenu(menuName="Virtual Dungeon/Makong — especiales")]
public sealed class MakongSpecialSettings:ScriptableObject {
 [HideInInspector] public int specialEvery=3;
 [Header("IA táctica: turnos completos de espera después de usar una habilidad")]
 [Min(0)] public int earthquakeCooldownTurns=3,swallowCooldownTurns=4,summonCooldownTurns=4;
 [Min(0)] public int normalTurnsBetweenSpecials=1;
 [Min(0)] public int maximumSwallows=2;
 [Range(0,2)] public int maximumHelpers=2;
 [Range(0,1)] public float earthquakeProbability=.7f,swallowProbability=1,summonProbability=.75f,twoHelpersProbability=.25f;
 [Range(0,1)] public float minimumSummonHPFraction=.45f;
 public int earthquakePriority=60,swallowPriority=100,summonPriority=80;
 [Min(1)] public int helperMinimumLevel=3,helperMaximumLevel=5;
 [Min(1)] public int helperMinimumAgeTurns=2;
 public bool debugDecisions=true;
 public SpeciesSpawnWeight[] helperSpecies={new SpeciesSpawnWeight{speciesId="Tapaku",weight=1},new SpeciesSpawnWeight{speciesId="Hongo",weight=1}};
 [Min(.03f)] public float normalFrameSeconds=.2f,normalImpactDelay=.22f;
 [Header("Super salto")]
 [Min(.03f)] public float windupSeconds=.45f,ascentSeconds=.25f,aboveSeconds=.8f,fallSeconds=.35f,landingFrameSeconds=.2f;
 [Min(1)] public float jumpHeight=8;
 [Header("Sacudida del pisotón")]
 [Min(0)] public float landingShakeAmplitude=.38f;
 [Min(.01f)] public float landingShakeSeconds=.85f;
 [Min(.05f)] public float circleStartRadius=.3f,circleEndRadius=2;
 [Header("Lengua y curación")]
 [Range(0,1)] public float healBelowFraction=.45f,healFraction=0;
 [Min(0)] public int healFlat=18;
 [Min(.03f)] public float swallowFrameSeconds=.18f,retractSeconds=.65f;
 [Header("Lengua separada (Sliced)")]
 [Min(.03f)] public float tongueExtendSeconds=.3f;
 [Min(.1f)] public float tongueThickness=1;
 public Vector2 tongueMouthOffset=new Vector2(.46f,1.27f);
 static MakongSpecialSettings cached;
 public static MakongSpecialSettings Current {get{if(cached==null)cached=Resources.Load<MakongSpecialSettings>("Combat/MakongSpecialSettings");if(cached==null)cached=CreateInstance<MakongSpecialSettings>();return cached;}}
 public int Healing(int maximumHP){return Mathf.Max(0,Mathf.RoundToInt(maximumHP*healFraction)+healFlat);}
}
