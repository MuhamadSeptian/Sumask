using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AllEnemiesTM", menuName = "TargetModes/All Enemies")]
public class AllEnemiesTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        // Mengembalikan daftar semua musuh yang aktif di arena
        return new(EnemySystem.Instance.Enemies);
    }
}
