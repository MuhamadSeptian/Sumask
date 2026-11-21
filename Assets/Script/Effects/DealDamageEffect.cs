using System.Collections.Generic;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;

    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        // Menargetkan semua musuh yang ada di dalam sistem musuh saat ini
        DealDamageGA dealDamageGA = new(damageAmount, targets);
        return dealDamageGA;
    }
}