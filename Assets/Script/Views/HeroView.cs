using UnityEngine;

public class HeroView : CombatantView
{
    [Header("Status Indicator")]
    [SerializeField] private SpriteRenderer statusIndicator; // Sprite di atas hero
    [SerializeField] private Sprite idleSprite; // Sprite saat idle/diam
    [SerializeField] private Sprite playingCardSprite; // Sprite saat main kartu
    [SerializeField] private Sprite attackSprite; // Sprite saat menyerang
    [SerializeField] private Sprite hitSprite; // Sprite saat kena damage
    
    private Sprite originalSprite; // Sprite asli hero
    private Animator animator; // Animator untuk idle animation

    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, heroData.Image);
        
        // Ambil animator dari GameObject ini atau child-nya
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                Debug.Log($"[HeroView] Animator found: {animator.name}");
            }
            else
            {
                Debug.LogWarning("[HeroView] No Animator component found!");
            }
        }
        
        // Simpan sprite asli dari spriteRenderer (inherited dari CombatantView)
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
        
        ShowIdleIndicator();
    }

    public void ShowIdleIndicator()
    {
        if (statusIndicator != null && idleSprite != null)
        {
            statusIndicator.sprite = idleSprite;
            statusIndicator.gameObject.SetActive(true);
        }
    }

    public void ShowPlayingCardIndicator()
    {
        if (statusIndicator != null && playingCardSprite != null)
        {
            statusIndicator.sprite = playingCardSprite;
            statusIndicator.gameObject.SetActive(true);
        }
    }

    public void ShowAttackIndicator()
    {
        if (statusIndicator != null && attackSprite != null)
        {
            statusIndicator.sprite = attackSprite;
            statusIndicator.gameObject.SetActive(true);
        }
    }

    public void ShowHitIndicator()
    {
        if (statusIndicator != null && hitSprite != null)
        {
            statusIndicator.sprite = hitSprite;
            statusIndicator.gameObject.SetActive(true);
        }
    }

    public void HideIndicator()
    {
        if (statusIndicator != null)
        {
            statusIndicator.gameObject.SetActive(false);
        }
    }

    public void PlaySpriteAnimation(Sprite[] sprites, float frameTime)
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("[HeroView] PlaySpriteAnimation called with null or empty sprites!");
            return;
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("[HeroView] SpriteRenderer is NULL! Cannot play animation!");
            return;
        }

        Debug.Log($"[HeroView] Starting animation with {sprites.Length} frames, {frameTime}s per frame");
        StartCoroutine(PlaySpriteAnimationCoroutine(sprites, frameTime));
    }

    private System.Collections.IEnumerator PlaySpriteAnimationCoroutine(Sprite[] sprites, float frameTime)
    {
        Debug.Log($"[HeroView] Animation coroutine started");
        
        // DISABLE Animator supaya tidak override sprite kita!
        if (animator != null)
        {
            Debug.Log("[HeroView] Disabling Animator");
            animator.enabled = false;
        }
        
        // Play semua frame
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
            {
                Debug.LogError($"[HeroView] Frame {i+1} sprite is NULL!");
                continue;
            }
            
            Debug.Log($"[HeroView] Playing frame {i+1}/{sprites.Length}");
            spriteRenderer.sprite = sprites[i];
            yield return new WaitForSeconds(frameTime);
        }

        Debug.Log("[HeroView] Animation complete, restoring original sprite");
        
        // Kembali ke sprite asli
        if (originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
        
        // RE-ENABLE Animator supaya idle animation jalan lagi
        if (animator != null)
        {
            Debug.Log("[HeroView] Re-enabling Animator");
            animator.enabled = true;
        }
    }
}
