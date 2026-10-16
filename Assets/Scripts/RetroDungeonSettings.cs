using UnityEngine;
[CreateAssetMenu(menuName="Codebound/Retro dungeon experiment")]
public sealed class RetroDungeonSettings:ScriptableObject {
 public HelmetCRTSettings crtProfile;
 public DungeonAudioSettings audioProfile;public Texture2D frontPipe;
 public RetroNormalLayer[] normalMaps=new RetroNormalLayer[0];
 public RetroFrontPipeDefinition[] frontPipes=new RetroFrontPipeDefinition[0];
 [Range(0,1)]public float normalStrength=.28f;
 [Header("Capas del PSD actualizado (en orden de fondo a frente)")]
 public RetroTextureRegion[] textureRegions=new RetroTextureRegion[0];
 [Range(0,1)]public float decorativeBushFrequency=.23f,swayingBushFraction=.45f;
 public Texture2D floorEdge;[Min(.01f)]public float floorEdgeHeight=.48f;[Range(.25f,.95f)]public float layerAlphaCutoff=.7f;
 public RetroOrderedLayer[] orderedLayers=new RetroOrderedLayer[0];
 [Header("Apariciones desde arbustos")]
 public bool bushEmergence=true;[Min(.1f)]public float bushTellSeconds=.8f,jumpSeconds=.65f;[Min(0)]public float bushShake=.045f,jumpHeight=1.15f;
 public int seed=1985;public Texture2D sky,floor,pipe;public Texture2D[] clouds,mountains,bushes;
 [Range(0,2)]public float cloudDensity=1.2f,mountainDensity=1.35f,bushDensity=1.65f;
 public Vector2 cloudHeight=new Vector2(.5f,1.15f),mountainHeight=new Vector2(.9f,2.15f),bushHeight=new Vector2(.4f,1.1f);
 [Header("Prueba reversible de profundidad 2.5D")]
 public bool layeredDepth=true;[Range(0,3)]public float depthStrength=1.5f;[Range(0,.5f)]public float mountainAtmosphere=.22f;public Color depthAtmosphere=new Color(.48f,.7f,1);
 [Range(0,.12f)]public float cloudParallax=.006f,mountainParallax=.025f,bushParallax=.075f;
 [Range(0,1)]public float pipeFrequency=.18f;[Range(.1f,1)]public float pipeWidth=.7f;[Range(.2f,3)]public float pipeHeight=1.7f;
 [Header("Comparacion de atmosfera (cambia en vivo)")]
 public RetroAtmosphereVariant atmosphereVariant=RetroAtmosphereVariant.C_BandasRetro;
 public Color luminousFogColor=new Color(.58f,.78f,1);
 [Range(1,10)]public float fogStartCells=4.5f;[Range(2,16)]public float fogEndCells=11;
 [Range(.3f,2)]public float fogResponse=.65f;
 public bool quantizedFog=true;[Range(5,8)]public int fogLevels=6;[Range(.05f,.49f)]public float bandSoftness=.2f;
 public Color distanceLight=Color.white;[Min(1)]public float fogStart=16,fogEnd=40;
 [Header("Interactive pipe - independent test placements")]
 public Vector2Int interactiveCell=new Vector2Int(11,13);[Range(0,3)]public int pipeWallDirection=1;
 public string[] placementIds={"retro.pipe.enemy.0","retro.pipe.enemy.1","retro.pipe.enemy.2"};
 [Range(0,1)]public float spawnProbability=1;[Min(1)]public int activationSteps=2;[Min(.1f)]public float cooldownSeconds=5;
 [Min(1)]public int maximumSpawns=1;public bool once=true;
}

[System.Serializable]public class RetroNormalLayer {public Texture2D texture,normal;}
[System.Serializable]public class RetroFrontPipeDefinition {public string id;public Vector2Int cell;[Range(0,3)]public int direction;public string[] placementIds=new string[0];}

public enum RetroAtmosphereVariant { A_Actual, B_NieblaSuave, C_BandasRetro }

[System.Serializable]public class RetroOrderedLayer {public string name;public Texture2D[] textures=new Texture2D[0];public RetroLayerDepth depth;public bool continuous;public float density=1;public Vector2 heights=new Vector2(.7f,1.4f);[Range(0,1)]public float altitude;public float offset=.05f;[Range(0,2)]public float parallaxMultiplier=1;}
public enum RetroLayerDepth { Cloud,Mountain,Bush }

[System.Serializable]public class RetroTextureRegion {public Texture2D texture;public Vector4 uv=new Vector4(0,0,1,1);}
