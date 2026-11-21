using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Gain Shield Effect")]
public class GainShieldEffect : Effect
{
    public int ShieldAmount;

    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        return new GainShieldGA(targets, ShieldAmount);
    }
}
