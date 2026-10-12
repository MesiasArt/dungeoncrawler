using UnityEngine;

public class HongoEnemyVisual : GhostEnemyVisual
{
    protected override string FramesPath { get { return "Enemies/Hongo/"; } }
    protected override Vector2 SpriteSize { get { return new Vector2(2.7f,1.8f); } }
}