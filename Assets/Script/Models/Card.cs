using UnityEngine;
using System.Collections.Generic;
public class Card
{
    public CardData CardData { get; private set; }
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;
    public Sprite[] AnimationSprites => data.AnimationSprites;
    public float AnimationSpeed => data.AnimationSpeed;
    public int Mana { get; private set; }

    private readonly CardData data;

    public Card(CardData cardData)
    {
        data = cardData;
        Mana = cardData.Mana;
    }
}