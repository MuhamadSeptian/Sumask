// Assets/Script/Systems/EnemySystem.cs
using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;

    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
    }

    void OnDisable()
    {
        // PERBAIKAN: Tambahkan (EnemyTurnPerformer) agar sesuai dengan ActionSystem baru
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }

    public void Setup(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            enemyBoardView.AddEnemy(enemyData);
        }
    }

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {
        // Mengiterasi semua musuh di papan untuk melakukan serangan
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            AttackHeroGA attackHeroGA = new(enemy);
            ActionSystem.Instance.AddReaction(attackHeroGA);
        }
        yield return null;
    }

    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;

        // TAMBAHAN: Mainkan animasi serangan enemy
        attacker.PlayAttackAnimation();
        
        // Tunggu sebentar untuk animasi mulai
        yield return new WaitForSeconds(0.2f);

        // Gerakan visual menyerang menggunakan DOTween
        Tween tween = attacker.transform.DOMoveX(attacker.transform.position.x - 1f, 0.15f);
        yield return tween.WaitForCompletion();

        // Kembali ke posisi semula
        attacker.transform.DOMoveX(attacker.transform.position.x + 1f, 0.25f);

        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.HeroView });
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }

    [SerializeField] private string nextSceneName = "TimelineScene"; // Scene yang akan dimuat setelah menang
    
    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        // Memanggil fungsi di EnemyBoardView untuk menghapus musuh secara visual
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
        
        // Check apakah semua enemy sudah mati
        if (enemyBoardView.EnemyViews.Count == 0)
        {
            Debug.Log("[EnemySystem] Semua enemy mati! Loading timeline scene...");
            yield return new WaitForSeconds(1.5f); // Delay untuk efek kemenangan
            
            // Load scene timeline/victory
            SceneManager.LoadScene(nextSceneName);
        }
    }
}