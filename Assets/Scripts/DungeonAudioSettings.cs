using UnityEngine;
[CreateAssetMenu(menuName="Dungeon/Audio Settings")]
public sealed class DungeonAudioSettings : ScriptableObject
{
    [Range(0,1)] public float dialogueVolume=.22f;
    [Range(0,1)] public float dialogueAdvanceVolume=.35f;
    public AudioClip dungeonMusic;
    public AudioClip combatMusic;
    [Range(0,1)] public float musicVolume=.28f;
    [Range(0,1)] public float menuMusicMultiplier=.7f;
    [Range(0,1)] public float effectsVolume=.45f;
}
