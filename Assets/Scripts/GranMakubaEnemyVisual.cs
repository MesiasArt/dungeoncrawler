using UnityEngine;
public sealed class GranMakubaEnemyVisual : ImportedEnemyVisual
{
 public override int Kind { get { return 6; } }
 protected override int AttackFrames { get { return 2; } }
 protected override int SpecialFrames { get { return 5; } }
}
