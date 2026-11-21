using System.Collections.Generic; // Wajib ada untuk List<>
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);
        
        // Hanya spawn 1 enemy (enemy pertama dari list)
        List<EnemyData> singleEnemy = new List<EnemyData>();
        if (enemyDatas != null && enemyDatas.Count > 0)
        {
            singleEnemy.Add(enemyDatas[0]);
        }
        EnemySystem.Instance.Setup(singleEnemy);
        
        // 1. Setup Deck Awal
        CardSystem.Instance.Setup(heroData.Deck);

        // 2. Buat Aksi Draw 5 Kartu
        DrawCardsGA drawCardsGA = new DrawCardsGA(5);

        // 3. Kirim Aksi ke ActionSystem
        // PERHATIKAN: Gunakan 'AddReaction', bukan 'Perform'
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}