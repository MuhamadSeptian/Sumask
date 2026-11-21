using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDrawCardsEffect", menuName = "Effects/Draw Cards Effect")]
public class DrawCardsEffect : Effect
{
    [SerializeField] private int drawAmount = 2;

    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        Debug.Log($"[DrawCardsEffect] Membuat DrawCardsGA untuk draw {drawAmount} kartu");
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
}
