using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    
    [Header("Attack Animation")]
    [SerializeField] private Sprite[] attackAnimationSprites; // Array sprite untuk animasi serangan
    [SerializeField] private float attackFrameTime = 0.1f; // Waktu per frame animasi
    [SerializeField] private bool autoScaleSprites = true; // Auto-fix scale jika sprite berbeda ukuran
    
    public int AttackPower { get; set; }
    private Sprite originalSprite; // Sprite asli enemy
    private Vector3 originalScale; // Scale asli enemy (untuk fix sprite kecil)
    private Animator animator; // Animator untuk idle animation (opsional)

    private void Awake()
    {
        // Validasi: Cek apakah attackText sudah di-assign di Inspector
        if (attackText == null) Debug.LogError("EnemyView: Field 'attackText' belum di-assign di Inspector!", this);
        
        // Ambil animator jika ada (opsional, untuk idle animation)
        animator = GetComponentInChildren<Animator>();
        
        // Simpan scale asli untuk mencegah sprite mengecil
        originalScale = transform.localScale;
    }

    public void Setup(EnemyData enemyData)
    {
        Debug.Log($"[EnemyView] Setup dipanggil untuk '{enemyData.name}'");
        
        if (enemyData == null)
        {
            Debug.LogError("EnemyView.Setup: EnemyData is NULL!", this);
            return;
        }

        AttackPower = enemyData.AttackPower;
        Debug.Log($"[EnemyView] AttackPower set ke: {AttackPower}");
        
        UpdateAttackText();
        
        Debug.Log($"[EnemyView] Memanggil SetupBase - Health: {enemyData.Health}, Image: {(enemyData.Image != null ? enemyData.Image.name : "NULL")}");
        SetupBase(enemyData.Health, enemyData.Image);
                // Simpan sprite asli enemy dan scale asli
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
            originalScale = transform.localScale; // Update scale asli setelah Setup
        }
                Debug.Log($"[EnemyView] Setup selesai! Position: {transform.position}, Active: {gameObject.activeSelf}");
    }

    private void UpdateAttackText()
    {
        if (attackText != null)
        {
            attackText.text = "ATK: " + AttackPower;
            Debug.Log($"[EnemyView] Attack text updated: '{attackText.text}', Active: {attackText.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[EnemyView] attackText is NULL! Cannot update text.", this);
        }
    }
    
    /// <summary>
    /// Memainkan animasi serangan enemy menggunakan sprite animation
    /// Auto-fix scale jika sprite berbeda ukuran
    /// </summary>
    public void PlayAttackAnimation()
    {
        if (attackAnimationSprites == null || attackAnimationSprites.Length == 0)
        {
            Debug.LogWarning($"[EnemyView] {name}: Tidak ada attack animation sprites! Animasi tidak dimainkan.");
            return;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"[EnemyView] {name}: SpriteRenderer is NULL! Cannot play attack animation!");
            return;
        }

        Debug.Log($"[EnemyView] {name}: Memulai attack animation dengan {attackAnimationSprites.Length} frames");
        StartCoroutine(PlayAttackAnimationCoroutine());
    }

    private System.Collections.IEnumerator PlayAttackAnimationCoroutine()
    {
        Debug.Log($"[EnemyView] {name}: Attack animation coroutine started");
        
        // DISABLE Animator supaya tidak override sprite kita (jika ada)
        if (animator != null && animator.enabled)
        {
            Debug.Log($"[EnemyView] {name}: Disabling Animator");
            animator.enabled = false;
        }
        
        // Simpan scale asli sebelum animasi (untuk fix sprite kecil)
        Vector3 scaleBeforeAnimation = transform.localScale;
        
        // Play semua frame animasi serangan
        for (int i = 0; i < attackAnimationSprites.Length; i++)
        {
            if (attackAnimationSprites[i] == null)
            {
                Debug.LogError($"[EnemyView] {name}: Frame {i+1} sprite is NULL!");
                continue;
            }
            
            Debug.Log($"[EnemyView] {name}: Playing attack frame {i+1}/{attackAnimationSprites.Length}");
            spriteRenderer.sprite = attackAnimationSprites[i];
            
            // AUTO-FIX: Kembalikan scale jika sprite mengubah ukuran
            if (autoScaleSprites)
            {
                transform.localScale = originalScale;
            }
            
            yield return new WaitForSeconds(attackFrameTime);
        }

        Debug.Log($"[EnemyView] {name}: Attack animation complete, restoring original sprite & scale");
        
        // Kembali ke sprite asli
        if (originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
        
        // PASTIKAN scale kembali ke aslinya
        transform.localScale = originalScale;
        
        // RE-ENABLE Animator supaya idle animation jalan lagi (jika ada)
        if (animator != null)
        {
            Debug.Log($"[EnemyView] {name}: Re-enabling Animator");
            animator.enabled = true;
        }
    }
}