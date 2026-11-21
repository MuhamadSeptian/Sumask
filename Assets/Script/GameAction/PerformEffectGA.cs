using System.Collections.Generic;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }

    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    {
        Effect = effect;

        // Logika ini mengecek apakah targets null. 
        // Jika tidak null, ia membuat salinan daftar baru (new list) 
        // agar tidak mengubah referensi asli.
        Targets = targets == null ? null : new(targets);
    }
}