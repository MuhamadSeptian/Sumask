using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }

    // Performers
    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        // --- PENGAMAN 1: Cek apakah Effect-nya ada? ---
        if (performEffectGA.Effect == null)
        {
            Debug.LogWarning("[EffectSystem] Ada kartu dimainkan tapi Efek-nya NULL (Kosong). Cek CardData di Inspector!");
            yield break; // Batalkan, jangan lanjut biar gak error
        }

        Debug.Log($"[EffectSystem] Menjalankan effect: {performEffectGA.Effect.GetType().Name}");
        GameAction effectAction = performEffectGA.Effect.GetGameAction(performEffectGA.Targets);

        // --- PENGAMAN 2: Cek apakah Action yang dihasilkan valid? ---
        if (effectAction == null)
        {
            Debug.LogWarning($"[EffectSystem] Effect {performEffectGA.Effect.GetType().Name} tidak menghasilkan GameAction!");
            yield break;
        }

        Debug.Log($"[EffectSystem] Effect menghasilkan action: {effectAction.GetType().Name}");
        ActionSystem.Instance.AddReaction(effectAction);
        yield return null;
    }
}