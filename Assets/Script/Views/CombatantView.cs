using TMPro;
using UnityEngine;
using DG.Tweening;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject shieldVisual; // GameObject dengan SpriteRenderer shield + Text angka
    [SerializeField] private TMP_Text shieldAmountText; // Text untuk angka shield
    [SerializeField] protected SpriteRenderer spriteRenderer; // Protected agar child class bisa akses

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int Shield { get; private set; }

    private void Awake()
    {
        // Validasi: Cek apakah semua field sudah di-assign di Inspector
        if (healthText == null) Debug.LogError("CombatantView: Field 'healthText' belum di-assign di Inspector!", this);
        if (shieldVisual == null) Debug.LogWarning("CombatantView: Field 'shieldVisual' belum di-assign. Shield visual tidak akan muncul.", this);
        if (spriteRenderer == null) Debug.LogError("CombatantView: Field 'spriteRenderer' belum di-assign di Inspector!", this);
        
        // Hide shield visual di awal
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    protected void SetupBase(int health, Sprite image)
    {
        Debug.Log($"[CombatantView] SetupBase - Health: {health}, Sprite: {(image != null ? image.name : "NULL")}");
        
        MaxHealth = CurrentHealth = health;
        
        if (spriteRenderer != null && image != null)
        {
            spriteRenderer.sprite = image;
            Debug.Log($"[CombatantView] Sprite di-set! SpriteRenderer enabled: {spriteRenderer.enabled}, Sprite: {spriteRenderer.sprite.name}");
            Debug.Log($"[CombatantView] SpriteRenderer SortingLayer: {spriteRenderer.sortingLayerName}, Order: {spriteRenderer.sortingOrder}");
        }
        else if (image == null)
        {
            Debug.LogWarning("CombatantView: Image/Sprite is NULL! Check EnemyData.", this);
        }
        else if (spriteRenderer == null)
        {
            Debug.LogError("CombatantView: SpriteRenderer is NULL! Assign di Inspector!", this);
        }
        
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + CurrentHealth;
            Debug.Log($"[CombatantView] Health text updated: '{healthText.text}', Active: {healthText.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[CombatantView] healthText is NULL! Cannot update text.", this);
        }
    }
    public void Damage(int damageAmount)
    {
        Debug.Log($"[{name}] Menerima {damageAmount} damage. Shield: {Shield}, HP sebelum: {CurrentHealth}");
        
        // Show hit indicator untuk Hero
        if (this is HeroView heroView)
        {
            heroView.ShowHitIndicator();
        }
        
        // 1. Shield menyerap damage terlebih dahulu
        if (Shield > 0)
        {
            int damageToShield = Mathf.Min(Shield, damageAmount);
            Shield -= damageToShield;
            damageAmount -= damageToShield;
            Debug.Log($"[{name}] Shield menyerap {damageToShield} damage. Shield tersisa: {Shield}");
        }
        
        // 2. Sisa damage masuk ke HP
        if (damageAmount > 0)
        {
            CurrentHealth -= damageAmount;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
            Debug.Log($"[{name}] HP berkurang {damageAmount}. HP sesudah: {CurrentHealth}");
        }

        // Animasi getar saat terkena hit
        transform.DOShakePosition(0.2f, 0.5f);
        UpdateHealthText();
        UpdateShieldText();
        
        // Kembali ke idle setelah animasi
        if (this is HeroView hero)
        {
            DOVirtual.DelayedCall(0.5f, () => hero.ShowIdleIndicator());
        }
    }

    public void GainShield(int amount)
    {
        Shield += amount;
        Debug.Log($"[{name}] Mendapat {amount} Shield. Total Shield: {Shield}");
        UpdateShieldText();
    }

    public void RemoveAllShield()
    {
        if (Shield > 0)
        {
            Debug.Log($"[{name}] Shield {Shield} hilang (turn selesai)");
            Shield = 0;
            UpdateShieldText();
        }
    }

    private void UpdateShieldText()
    {
        if (shieldVisual != null)
        {
            if (Shield > 0)
            {
                shieldVisual.SetActive(true);
                
                // Update angka shield jika ada text component
                if (shieldAmountText != null)
                {
                    shieldAmountText.text = Shield.ToString();
                }
            }
            else
            {
                shieldVisual.SetActive(false);
            }
        }
    }
}