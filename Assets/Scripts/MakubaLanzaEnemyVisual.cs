using UnityEngine;
public sealed class MakubaLanzaEnemyVisual : ImportedEnemyVisual
{
 public override int Kind { get { return 4; } }
 protected override int AttackFrames { get { return 1; } }
 protected override int SpecialFrames { get { return 3; } }
}
