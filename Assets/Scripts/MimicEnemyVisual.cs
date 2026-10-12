using UnityEngine;
public sealed class MimicEnemyVisual : ImportedEnemyVisual
{
 public override int Kind { get { return 7; } }
 protected override int AttackFrames { get { return 1; } }
 protected override int SpecialFrames { get { return 0; } }
}
