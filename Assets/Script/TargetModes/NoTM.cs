using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NoTM", menuName = "TargetModes/No Target")]
public class NoTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        // Mengembalikan daftar semua musuh yang aktif di arena
        return null;
    }
}
