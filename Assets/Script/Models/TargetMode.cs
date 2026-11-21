using System.Collections.Generic;
using UnityEngine;

public abstract class TargetMode : ScriptableObject
{
    public abstract List<CombatantView> GetTargets();
}