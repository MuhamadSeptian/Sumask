using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Effect : ScriptableObject
{
    public abstract GameAction GetGameAction(List<CombatantView> targets);
}