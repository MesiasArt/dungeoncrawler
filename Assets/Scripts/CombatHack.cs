using System;
using UnityEngine;

// Hacks report performance points; attack quality feeds the common ATK/DEF calculation.
public abstract class CombatHack : MonoBehaviour
{
    public virtual string DisplayName { get { return GetType().Name; } }
    public virtual bool UsesAttackLuck { get { return true; } }
    public virtual float AttackQuality(int performanceScore) { return Mathf.Clamp(performanceScore/10f,0,CombatBalanceSettings.Current.jumpMaximumQuality); }
    public abstract bool Active { get; }
    public abstract void Begin(Action<int> completed);
}
