using UnityEngine;

public class EnemyViewCreator : Singleton<EnemyViewCreator>
{
    [SerializeField] private EnemyView enemyViewPrefab;

    protected override void Awake()
    {
        base.Awake(); // PENTING: Panggil Singleton Awake untuk set Instance!
        
        if (enemyViewPrefab == null)
        {
            Debug.LogError("[EnemyViewCreator] enemyViewPrefab belum di-assign di Inspector!", this);
        }
        else
        {
            Debug.Log($"[EnemyViewCreator] Prefab sudah di-assign: {enemyViewPrefab.name}");
        }
    }

    public EnemyView CreateEnemyView(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        if (enemyViewPrefab == null)
        {
            Debug.LogError("[EnemyViewCreator] TIDAK BISA MEMBUAT ENEMY! Prefab belum di-assign. Buka GameObject 'EnemyViewCreator' di Scene dan assign prefab EnemyView di Inspector!", this);
            return null;
        }

        Debug.Log($"[EnemyViewCreator] Membuat enemy '{enemyData.name}' di posisi {position}");
        
        EnemyView enemyView = Instantiate(enemyViewPrefab, position, rotation);
        
        if (enemyView == null)
        {
            Debug.LogError("[EnemyViewCreator] Instantiate gagal! EnemyView is NULL!");
            return null;
        }
        
        Debug.Log($"[EnemyViewCreator] Enemy instance dibuat: {enemyView.name} di {enemyView.transform.position}");
        enemyView.Setup(enemyData);
        
        return enemyView;
    }
}
