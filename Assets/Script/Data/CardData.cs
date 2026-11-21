using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Card Data")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }

    // --- PERBAIKAN UTAMA DI SINI ---
    // Tambahkan "= new List<Effect>();" agar list ini tidak pernah Null
    [field: SerializeField] public Effect ManualTargetEffect { get; private set; } = null;
    [field: SerializeField] public List<AutoTargetEffect> OtherEffects { get; private set; } = new List<AutoTargetEffect>();
    [field: SerializeField] public Sprite[] AnimationSprites { get; private set; } = null; // Sprite array untuk animasi
    [field: SerializeField] public float AnimationSpeed { get; private set; } = 0.3f; // 0.3 detik per frame = lebih lambat dan jelas terlihat
}