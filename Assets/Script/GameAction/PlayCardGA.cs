using UnityEngine;
using System.Collections.Generic;

public class PlayCardGA : GameAction
{
    // Target musuh manual (misal: dipilih pakai panah tadi)
    public EnemyView ManualTarget { get; private set; }
    
    // Multiple targets (untuk sistem drag & drop baru)
    public List<CombatantView> Targets { get; private set; }

    // Data kartu yang dimainkan
    public Card Card { get; set; }

    // Constructor 1: Dipanggil jika kartu dimainkan TANPA target (misal: skill area)
    public PlayCardGA(Card card)
    {
        Card = card;
        ManualTarget = null;
        Targets = null;
    }

    // Constructor 2: Dipanggil jika kartu dimainkan DENGAN target spesifik (old system)
    public PlayCardGA(Card card, EnemyView target)
    {
        Card = card;
        ManualTarget = target;
        Targets = null;
    }
    
    // Constructor 3: Dipanggil dengan multiple targets (new system)
    public PlayCardGA(Card card, List<CombatantView> targets)
    {
        Card = card;
        ManualTarget = targets != null && targets.Count > 0 ? targets[0] as EnemyView : null;
        Targets = targets;
    }
}
