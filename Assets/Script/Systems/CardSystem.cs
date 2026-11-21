using System.Collections;
using System.Collections.Generic; // Fix: Huruf 'C' besar
using UnityEngine;
using DG.Tweening; // Fix: Tambahkan ini untuk DOMove/DOScale

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();

    void OnEnable()
    {
        // Subscribe / Attach
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    // Fix: Ubah menjadi OnDisable
    void OnDisable()
    {
        // Unsubscribe / Detach
        // Asumsi: Method untuk melepas adalah DetachPerformer
        ActionSystem.DetachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.DetachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    // --- Public ---

    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        
        // Shuffle deck awal
        Debug.Log($"[CardSystem] Setup deck dengan {drawPile.Count} kartu. Shuffling...");
        ShuffleDeck();
    }

    // --- Performers ---

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        Debug.Log($"[CardSystem] DrawCardsPerformer dipanggil! Draw {drawCardsGA.Amount} kartu. DrawPile: {drawPile.Count}, DiscardPile: {discardPile.Count}");
        
        for (int i = 0; i < drawCardsGA.Amount; i++)
        {
            // Jika draw pile habis, refill dari discard pile
            if (drawPile.Count == 0)
            {
                Debug.Log($"[CardSystem] Deck habis! Refill dari discard pile ({discardPile.Count} kartu)");
                
                if (discardPile.Count == 0)
                {
                    Debug.LogWarning($"[CardSystem] Tidak bisa draw kartu ke-{i + 1}. Deck dan discard pile kosong!");
                    yield break; // Hentikan, tidak ada kartu lagi
                }
                
                RefillDeck();
            }
            
            // Draw 1 kartu
            yield return DrawCard();
        }
        
        Debug.Log($"[CardSystem] Selesai draw {drawCardsGA.Amount} kartu. Hand: {hand.Count}");
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
            CardView cardView = handView.RemoveCard(card);

            // Pastikan cardView tidak null sebelum animasi
            if (cardView != null)
            {
                yield return DiscardCard(cardView);
            }
        }
        hand.Clear();
    }

    // Ganti method PlayCardPerformer yang lama dengan yang ini:
    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        // Validasi: Pastikan card ada
        if (playCardGA == null || playCardGA.Card == null)
        {
            Debug.LogError("PlayCardGA atau Card adalah NULL!");
            yield break;
        }

        // Validasi: Cek apakah mana cukup
        if (!ManaSystem.Instance.HasEnoughMana(playCardGA.Card.Mana))
        {
            Debug.LogWarning($"[CardSystem] Tidak cukup mana untuk mainkan '{playCardGA.Card.Title}'! Butuh: {playCardGA.Card.Mana}");
            yield break; // Batalkan play card
        }

        // 1. Spend Mana terlebih dahulu
        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);

        // 1.5. Show playing card indicator
        HeroSystem.Instance.HeroView.ShowPlayingCardIndicator();
        
        // 1.6. Play hero sprite animation jika ada - TUNGGU sampai selesai
        if (playCardGA.Card.AnimationSprites != null && playCardGA.Card.AnimationSprites.Length > 0)
        {
            Debug.Log($"[CardSystem] Playing animation for '{playCardGA.Card.Title}' - {playCardGA.Card.AnimationSprites.Length} frames at {playCardGA.Card.AnimationSpeed}s per frame");
            HeroSystem.Instance.HeroView.PlaySpriteAnimation(playCardGA.Card.AnimationSprites, playCardGA.Card.AnimationSpeed);
            
            // Tunggu sampai animasi selesai
            float animationDuration = playCardGA.Card.AnimationSprites.Length * playCardGA.Card.AnimationSpeed;
            yield return new WaitForSeconds(animationDuration);
        }
        else
        {
            Debug.Log($"[CardSystem] No animation for '{playCardGA.Card.Title}' - Sprites: {(playCardGA.Card.AnimationSprites == null ? "null" : playCardGA.Card.AnimationSprites.Length.ToString())}");
        }

        // 2. Proses Buang Kartu (Visual & Data)
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);

        // Validasi: Pastikan cardView tidak null sebelum discard
        if (cardView != null)
        {
            yield return DiscardCard(cardView);
        }
        else
        {
            Debug.LogWarning("CardView tidak ditemukan saat PlayCard. Kartu sudah tidak ada di tangan?");
        }

        // 3. Proses Manual Target Effect
        if (playCardGA.Card.ManualTargetEffect != null && playCardGA.Targets != null && playCardGA.Targets.Count > 0)
        {
            PerformEffectGA performEffectGA = new PerformEffectGA(playCardGA.Card.ManualTargetEffect, playCardGA.Targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        else if (playCardGA.Card.ManualTargetEffect == null && playCardGA.Targets != null && playCardGA.Targets.Count > 0)
        {
            Debug.LogWarning($"[CardSystem] Kartu '{playCardGA.Card.Title}' tidak memiliki ManualTargetEffect!");
        }
        
        // 4. Proses OtherEffects
        if (playCardGA.Card.OtherEffects != null && playCardGA.Card.OtherEffects.Count > 0)
        {
            foreach (var effectWrapper in playCardGA.Card.OtherEffects)
            {
                if (effectWrapper.Effect != null)
                {
                    List<CombatantView> targets = null;
                    if (effectWrapper.TargetMode != null)
                    {
                        targets = effectWrapper.TargetMode.GetTargets();
                        
                        // Jika targets kosong dan effect untuk Hero (GainShield, Heal, dll), set Hero sebagai target
                        if (targets == null || targets.Count == 0)
                        {
                            // Cek jika effect adalah GainShieldEffect atau effect lain yang target Hero
                            if (effectWrapper.Effect is GainShieldEffect)
                            {
                                targets = new List<CombatantView> { HeroSystem.Instance.HeroView };
                            }
                        }
                    }
                    
                    PerformEffectGA performEffectGA = new PerformEffectGA(effectWrapper.Effect, targets);
                    ActionSystem.Instance.AddReaction(performEffectGA);
                }
                else
                {
                    Debug.LogWarning($"[CardSystem] OtherEffect NULL di kartu '{playCardGA.Card.Title}'!");
                }
            }
        }

        // 5. Kembali ke idle indicator
        yield return new WaitForSeconds(0.5f);
        HeroSystem.Instance.HeroView.ShowIdleIndicator();
    }

    // --- Reactions ---
    // Fix: Method ini harus menempel langsung di CardSystem, jangan dibungkus class EnemyTurnHandler lagi

    // Dipanggil sebelum giliran musuh dimulai
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    // Dipanggil setelah giliran musuh selesai
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // Hapus shield hero di awal turn baru
        RemoveAllShieldsGA removeShields = new();
        ActionSystem.Instance.AddReaction(removeShields);
        
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }

    // --- Helpers ---

    private IEnumerator DrawCard()
    {
        // Validasi: Pastikan drawPile tidak kosong
        if (drawPile.Count == 0)
        {
            Debug.LogError("[CardSystem] DrawCard dipanggil tapi drawPile kosong! Ini seharusnya tidak terjadi.");
            yield break;
        }

        Card card = drawPile.Draw(); // Pastikan extension method .Draw() ada
        
        // Validasi: Pastikan card tidak null
        if (card == null)
        {
            Debug.LogError("[CardSystem] Draw() mengembalikan null card!");
            yield break;
        }

        hand.Add(card);

        // Untuk game 2D, cukup kirim posisi tanpa rotation
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position);

        // Pastikan method di HandView namanya 'AddCard' atau 'AddCardView' (sesuaikan dengan script HandView Anda)
        yield return handView.AddCard(cardView);
    }

    private void RefillDeck()
    {
        if (discardPile.Count == 0)
        {
            Debug.LogWarning("[CardSystem] Refill dipanggil tapi discard pile kosong! Tidak ada kartu untuk di-reshuffle.");
            return;
        }

        Debug.Log($"[CardSystem] Reshuffle! Memindahkan {discardPile.Count} kartu dari discard ke draw pile.");
        
        // Pindahkan semua kartu dari discard ke draw pile
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        
        // Shuffle/Kocok kartu menggunakan Fisher-Yates algorithm
        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
        
        Debug.Log($"[CardSystem] Deck di-shuffle! Total kartu: {drawPile.Count}");
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        // Validasi: Pastikan cardView tidak null
        if (cardView == null)
        {
            Debug.LogWarning("DiscardCard: CardView is NULL, skipping discard animation.");
            yield break;
        }

        cardView.transform.DOScale(Vector3.zero, 0.25f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.25f);
        yield return tween.WaitForCompletion();

        Destroy(cardView.gameObject);
    }
}