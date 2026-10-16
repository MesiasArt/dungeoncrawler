using UnityEngine;
[CreateAssetMenu(menuName="Dungeon/Audio Settings")]
public sealed class DungeonAudioSettings : ScriptableObject
{
    [Range(0,1)] public float dialogueVolume=.22f;
    [Range(0,1)] public float dialogueAdvanceVolume=.35f;
    public AudioClip dungeonMusic;
    public AudioClip combatMusic;
    [Header("Listas de musica (vacias: usar las pistas anteriores)")]
    public AudioClip[] explorationPlaylist=new AudioClip[0],normalCombatPlaylist=new AudioClip[0];
    public bool shufflePlaylist=true;
    [Header("Digital Kingdom")]
    public AudioClip cityMusic;
    [Range(.1f,3)]public float cityMusicFadeSeconds=1;

    [Range(0,1)] public float musicVolume=.28f;
    [Range(0,1)] public float menuMusicMultiplier=.7f;
    [Range(0,1)] public float effectsVolume=.45f;
}
