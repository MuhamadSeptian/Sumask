using UnityEngine;
using DG.Tweening;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    // Untuk game 2D, tidak perlu Quaternion rotation
    public CardView CreateCardView(Card card, Vector3 position)
    {
        Debug.Log("Kartu sedang dibuat!");
        // Membuat instance kartu dari prefab (rotation default untuk 2D)
        CardView cardView = Instantiate(cardViewPrefab, position, Quaternion.identity);

        // Animasi muncul: dari skala 0 ke skala normal (1)
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(Vector3.one, 0.15f);
        cardView.Setup(card);
        return cardView;
    }
}