using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;

    public List<EnemyView> EnemyViews { get; private set; } = new();

    private void Awake()
    {
        // Validasi: Cek apakah slots sudah di-assign
        if (slots == null || slots.Count == 0)
        {
            Debug.LogError("EnemyBoardView: Slots belum di-assign atau kosong! Setup slots di Inspector.", this);
        }
    }

    public void AddEnemy(EnemyData enemyData)
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyBoardView: EnemyData is NULL!", this);
            return;
        }

        if (EnemyViews.Count >= slots.Count)
        {
            Debug.LogWarning("EnemyBoardView: Tidak ada slot kosong lagi!", this);
            return;
        }

        // Mengambil slot berdasarkan jumlah musuh yang sudah ada
        Transform slot = slots[EnemyViews.Count];

        // Validasi: Pastikan EnemyViewCreator Instance ada
        if (EnemyViewCreator.Instance == null)
        {
            Debug.LogError("[EnemyBoardView] EnemyViewCreator.Instance is NULL! Pastikan ada GameObject 'EnemyViewCreator' di Scene dengan script EnemyViewCreator!", this);
            return;
        }

        // Meminta Creator untuk membuat instance musuh di posisi slot tersebut
        EnemyView enemyView = EnemyViewCreator.Instance.CreateEnemyView(enemyData, slot.position, Quaternion.identity);

        // Validasi: Pastikan enemyView berhasil dibuat
        if (enemyView == null)
        {
            Debug.LogError($"[EnemyBoardView] Gagal membuat EnemyView untuk '{enemyData.name}'! Cek EnemyViewCreator prefab assignment.", this);
            return;
        }

        // Set parent dan reset local transform untuk 2D
        enemyView.transform.SetParent(slot, false);
        enemyView.transform.localPosition = Vector3.zero;
        enemyView.transform.localRotation = Quaternion.identity;
        enemyView.transform.localScale = Vector3.one;
        
        EnemyViews.Add(enemyView);
        
        Debug.Log($"[EnemyBoardView] Enemy '{enemyData.name}' ditambahkan ke slot {EnemyViews.Count - 1}");
    }
    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        // Menghapus dari daftar musuh aktif di papan
        EnemyViews.Remove(enemyView);

        // Animasi mengecil hingga hilang menggunakan DOTween
        Tween tween = enemyView.transform.DOScale(Vector3.zero, 0.25f);
        yield return tween.WaitForCompletion();

        // Menghancurkan objek dari memory
        Destroy(enemyView.gameObject);
    }
}