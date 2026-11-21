using System.Collections;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    private const int MAX_MANA = 3;
    private int currentMana = MAX_MANA;

    void Start()
    {
        // Initialize UI at start
        if (manaUI != null)
        {
            manaUI.UpdateManaText(currentMana);
        }
        else
        {
            Debug.LogError("ManaUI belum di-assign di Inspector!", this);
        }
    }

    void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        // Validasi: Pastikan tidak spend lebih dari mana yang ada
        if (spendManaGA.Amount > currentMana)
        {
            Debug.LogWarning($"[ManaSystem] Tidak cukup mana! Butuh: {spendManaGA.Amount}, Ada: {currentMana}");
            yield break;
        }

        currentMana -= spendManaGA.Amount;
        Debug.Log($"[ManaSystem] Spend {spendManaGA.Amount} mana. Sisa: {currentMana}");
        
        if (manaUI != null)
        {
            manaUI.UpdateManaText(currentMana);
        }
        
        yield return null;
    }

    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        currentMana = MAX_MANA;
        Debug.Log($"[ManaSystem] Mana di-refill ke {MAX_MANA}");
        
        if (manaUI != null)
        {
            manaUI.UpdateManaText(currentMana);
        }
        
        yield return null;
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        RefillManaGA refillManaGA = new();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}