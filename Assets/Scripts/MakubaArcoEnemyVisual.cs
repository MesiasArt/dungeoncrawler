using UnityEngine;
public sealed class MakubaArcoEnemyVisual : ImportedEnemyVisual
{
 public override int Kind { get { return 5; } }
 protected override int AttackFrames { get { return 2; } }
 protected override int SpecialFrames { get { return 0; } }
}
