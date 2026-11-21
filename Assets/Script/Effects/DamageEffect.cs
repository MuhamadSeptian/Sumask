using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Effects/Damage Effect")]
public class DamageEffect : Effect
{
    public int damageAmount = 4;

    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("[DamageEffect] Tidak ada target yang dipilih!");
            return null;
        }
        
        Debug.Log($"[DamageEffect] Memberikan {damageAmount} damage ke {targets.Count} target");
        DealDamageGA dealDamageGA = new(damageAmount, targets);
        return dealDamageGA;
    }
}