using UnityEngine;
// Inherits Ghost's stepped poses, timings, sounds and floor reflection.
public class TapakuEnemyVisual : GhostEnemyVisual
{
    protected override string FramesPath { get { return "Enemies/Tapaku/"; } }
    protected override Vector2 SpriteSize { get { return new Vector2(2.76f,1.8f); } }
}
