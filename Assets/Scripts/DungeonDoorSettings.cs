using UnityEngine;
[CreateAssetMenu(menuName="Dungeon/Door Settings")]
public class DungeonDoorSettings : ScriptableObject
{
    public bool initiallyOpen;
    [Min(.1f)] public float openingSpeed=3.5f;
    public bool completesLevel=true;
    [Min(.1f)] public float fadeDuration=.8f;
}
