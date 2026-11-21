using System.Collections;
using System.Collections.Generic;
using System.Linq; // WAJIB: Agar fungsi .Where() dan .FirstOrDefault() jalan
using UnityEngine;
using UnityEngine.Splines; // WAJIB: Pastikan package "Splines" sudah diinstall
using DG.Tweening; // WAJIB: Pastikan "DOTween" sudah diinstall

public class HandView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SplineContainer splineContainer; // Referensi ke GameObject Spline
    [SerializeField] private float cardSpacing = 0.1f; // Jarak antar kartu
    [SerializeField] private float animationDuration = 0.25f; // Kecepatan animasi

    // List internal untuk menyimpan kartu yang ada di tangan
    [SerializeField] private List<CardView> cards = new List<CardView>();

    // -------------------------------------------------------------------------
    // PUBLIC METHODS (Dipanggil oleh CardSystem)
    // -------------------------------------------------------------------------

    // Menambahkan kartu baru ke tampilan tangan
    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);

        // Atur posisi awal (opsional: agar kartu muncul dari tumpukan draw)
        // cardView.transform.position = ... (sudah diatur di CardSystem biasanya)

        // Update posisi semua kartu agar rapi
        yield return UpdateCardPositions();
    }

    // Menghapus kartu dari tampilan tangan (saat di-discard)
    public CardView RemoveCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError("HandView.RemoveCard: Card is NULL!");
            return null;
        }

        CardView cardView = GetCardView(card);

        if (cardView == null)
        {
            Debug.LogWarning($"HandView: Kartu '{card.Title}' tidak ditemukan di tangan! Total kartu di tangan: {cards.Count}");
            // Debug: Print semua kartu yang ada
            for (int i = 0; i < cards.Count; i++)
            {
                Debug.Log($"  Kartu di tangan [{i}]: {(cards[i].Card != null ? cards[i].Card.Title : "NULL")}");
            }
            return null;
        }

        cards.Remove(cardView);

        // Update posisi sisa kartu agar celahnya tertutup
        StartCoroutine(UpdateCardPositions());

        return cardView;
    }

    // -------------------------------------------------------------------------
    // PRIVATE METHODS (Logika Internal)
    // -------------------------------------------------------------------------

    // Mencari GameObject CardView berdasarkan data Card (Pakai LINQ)
    private CardView GetCardView(Card card)
    {
        return cards.Where(cardView => cardView.Card == card).FirstOrDefault();
    }

    // Logika untuk menata kartu horizontal rata
    private IEnumerator UpdateCardPositions()
    {
        // Jika tidak ada kartu atau Spline belum dipasang, jangan lakukan apa-apa
        if (cards.Count == 0 || splineContainer == null) yield break;

        // Ambil spline pertama dari container
        Spline spline = splineContainer.Spline;
        if (spline == null) yield break;

        // 1. Hitung Lebar Total
        float totalWidth = (cards.Count - 1) * cardSpacing;

        // 2. Tentukan Titik Awal (Tengah Spline adalah 0.5f)
        float startT = 0.5f - (totalWidth / 2f);

        // Ambil Y dari tengah spline untuk posisi rata
        Vector3 centerPos = splineContainer.EvaluatePosition(0.5f);
        float fixedY = centerPos.y;

        for (int i = 0; i < cards.Count; i++)
        {
            // Hitung posisi T untuk kartu ke-i
            float t = startT + (i * cardSpacing);

            // Pastikan T tidak keluar batas (0.0 sampai 1.0)
            t = Mathf.Clamp01(t);

            // 3. Ambil X dari Spline, tapi Y tetap sama (rata)
            Vector3 splinePos = splineContainer.EvaluatePosition(t);
            Vector3 worldPos = new Vector3(splinePos.x, fixedY, splinePos.z + (i * -0.01f));

            // 4. Rotasi tetap lurus (tidak miring)
            Quaternion targetRotation = Quaternion.identity;

            // 5. Eksekusi Animasi dengan DOTween
            cards[i].transform.DOMove(worldPos, animationDuration).SetEase(Ease.OutQuad);
            cards[i].transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.OutQuad);
            
            Debug.Log($"[HandView] Card {i}: t={t:F3}, worldPos={worldPos}");
        }

        yield return new WaitForSeconds(animationDuration);
    }
}