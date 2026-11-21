using System.Collections;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }

    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        // Show attack indicator jika Hero yang menyerang
        bool heroAttacking = false;
        if (dealDamageGA.Targets != null && dealDamageGA.Targets.Count > 0)
        {
            // Cek jika target adalah enemy (berarti hero yang attack)
            if (dealDamageGA.Targets[0] is EnemyView)
            {
                heroAttacking = true;
                HeroSystem.Instance.HeroView.ShowAttackIndicator();
            }
        }
        
        foreach (var target in dealDamageGA.Targets)
        {
            // Mengurangi HP target dan memicu animasi getar
            target.Damage(dealDamageGA.Amount);

            // Memunculkan efek visual kerusakan di posisi target
            Instantiate(damageVFX, target.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.15f);
            // Setelah target menerima kerusakan
            if (target.CurrentHealth <= 0)
            {
                if (target is EnemyView enemyView)
                {
                    // Jika target adalah musuh, picu aksi KillEnemy sebagai reaksi
                    KillEnemyGA killEnemyGA = new(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }
                else
                {
                    // Jika target bukan musuh (Pahlawan), jalankan logika Game Over
                    // // Open Game Over Screen...
                }
            }

        }

        // Kembali ke idle setelah attack selesai
        if (heroAttacking)
        {
            yield return new WaitForSeconds(0.3f);
            HeroSystem.Instance.HeroView.ShowIdleIndicator();
        }
    }
}