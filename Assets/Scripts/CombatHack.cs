using System;
using UnityEngine;

// Each hack owns its rules/UI and reports only confirmed damage to combat.
public abstract class CombatHack : MonoBehaviour
{
    public virtual string DisplayName { get { return GetType().Name; } }
    public abstract bool Active { get; }
    public abstract void Begin(Action<int> completed);
}
