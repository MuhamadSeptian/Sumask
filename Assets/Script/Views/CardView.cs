using UnityEngine;
using TMPro; // Penting: Tambahkan ini agar TMP_Text dikenali
using System.Collections.Generic;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR; 
    [SerializeField] private GameObject wrapper;    
    [SerializeField] private LayerMask dropLayer;
    [SerializeField] private GameObject arrowPrefab; // Prefab untuk arrow targeting

    public Card Card { get; private set; }
    private Vector3 dragStartPosition;
    private float dragStartRotationZ; // Untuk 2D cukup simpan rotasi Z
    private Vector3 dragOffset;
    private bool isDraggingAllowed = false; // Flag untuk track apakah drag diizinkan
    private GameObject activeArrow; // Instance arrow yang sedang aktif

    private void Awake()
    {
        // Validasi: Cek apakah semua field sudah di-assign di Inspector
        if (title == null) Debug.LogError("CardView: Field 'Title' belum di-assign di Inspector!", this);
        if (description == null) Debug.LogError("CardView: Field 'Description' belum di-assign di Inspector!", this);
        if (mana == null) Debug.LogError("CardView: Field 'Mana' belum di-assign di Inspector!", this);
        if (imageSR == null) Debug.LogError("CardView: Field 'ImageSR' belum di-assign di Inspector!", this);
        if (wrapper == null) Debug.LogError("CardView: Field 'Wrapper' belum di-assign di Inspector!", this);
    }

    public void Setup(Card card)
    {
        Card = card;

        if (title != null) title.text = card.Title;
        if (description != null) description.text = card.Description;
        if (mana != null) mana.text = card.Mana.ToString();
        if (imageSR != null) imageSR.sprite = card.Image;
    }

    // Tambahkan ini di dalam class CardView : MonoBehaviour
    private void OnMouseEnter() 
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        wrapper.SetActive(false);
        // Tentukan berapa jauh ke kiri (misal: 0.5f atau 1f)
        float offsetLeft = 2.0f;
        // Tentukan berapa jauh ke atas (karena Anda pakai 2D)
        float offsetUp = 3.5f;

        // Kurangi position.x dengan offsetLeft agar bergeser ke kiri
        Vector3 pos = new Vector3(transform.position.x - offsetLeft, transform.position.y + offsetUp, 0);

        CardViewHoverSystem.Instance.Show(Card, pos);
    }

    private void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true); // Memunculkan kembali tampilan kartu kecil
    }
    void OnMouseDown()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        // Cek apakah mana cukup untuk mainkan kartu ini
        if (!ManaSystem.Instance.HasEnoughMana(Card.Mana))
        {
            Debug.LogWarning($"Tidak cukup mana untuk mainkan '{Card.Title}'! Butuh: {Card.Mana}");
            isDraggingAllowed = false; // Tandai drag tidak diizinkan
            return; // Batalkan drag jika mana tidak cukup
        }

        isDraggingAllowed = true; // Tandai drag diizinkan
        Interactions.Instance.PlayerIsDragging = true;
        wrapper.SetActive(true);
        CardViewHoverSystem.Instance.Hide();
        dragStartPosition = transform.position;
        dragStartRotationZ = transform.eulerAngles.z; // Simpan rotasi Z saja untuk 2D
        transform.rotation = Quaternion.identity; // Reset rotasi
        Vector3 mousePos = MouseUtil.GetMousePositionInWorldSpace(0);
        dragOffset = transform.position - mousePos;

        // Spawn arrow untuk manual targeting (jika kartu punya ManualTargetEffect)
        Debug.Log($"[CardView] OnMouseDown - Card: {Card.Title}, ManualTargetEffect: {(Card.ManualTargetEffect != null ? Card.ManualTargetEffect.GetType().Name : "NULL")}, arrowPrefab: {(arrowPrefab != null ? arrowPrefab.name : "NULL")}");
        
        if (Card.ManualTargetEffect != null && arrowPrefab != null)
        {
            activeArrow = Instantiate(arrowPrefab);
            activeArrow.SetActive(true); // Pastikan aktif
            
            Debug.Log($"[CardView] Arrow instantiated: {activeArrow.name}, Active: {activeArrow.activeSelf}");
            
            // PENTING: Set ke position yang FIXED (dragStartPosition), bukan yang bergerak
            ArrowView arrowView = activeArrow.GetComponent<ArrowView>();
            if (arrowView != null)
            {
                // Gunakan position fixed agar arrow tetap dari posisi original kartu
                arrowView.SetupArrowFixed(dragStartPosition);
                Debug.Log($"[CardView] Arrow spawned with fixed position at {dragStartPosition}");
            }
            else
            {
                Debug.LogError($"[CardView] Arrow prefab tidak punya component ArrowView!");
            }
        }
        else
        {
            Debug.Log($"[CardView] Arrow TIDAK di-spawn - ManualTargetEffect: {Card.ManualTargetEffect != null}, arrowPrefab: {arrowPrefab != null}");
        }
    }
    void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (!isDraggingAllowed) return; // Jangan drag jika tidak diizinkan
        
        // Hanya block drag jika ada arrow aktif (sistem arrow targeting)
        if (activeArrow != null) return;
        
        Vector3 mousePos = MouseUtil.GetMousePositionInWorldSpace(0); // Untuk 2D, Z biasanya 0
        transform.position = mousePos + dragOffset;
    }
    void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (!isDraggingAllowed) return; // Jangan proses jika drag tidak diizinkan
        
        // Destroy arrow jika ada
        if (activeArrow != null)
        {
            Destroy(activeArrow);
            activeArrow = null;
        }

        // Deteksi di posisi mouse
        Vector3 mousePos = MouseUtil.GetMousePositionInWorldSpace(0);
        Collider2D[] allHits = Physics2D.OverlapCircleAll(mousePos, 2f);
        
        CombatantView targetEnemy = null;
        bool hasDropArea = false;
        
        // Cari CombatantView (enemy) dan DropArea
        foreach (var hit in allHits)
        {
            if (targetEnemy == null)
            {
                CombatantView combatant = hit.GetComponent<CombatantView>();
                if (combatant != null && combatant is EnemyView)
                {
                    targetEnemy = combatant;
                }
            }
            
            if (((1 << hit.gameObject.layer) & dropLayer.value) != 0)
            {
                hasDropArea = true;
            }
        }
        
        // Play card berdasarkan target
        if (targetEnemy != null && Card.ManualTargetEffect != null)
        {
            List<CombatantView> targets = new List<CombatantView> { targetEnemy };
            PlayCardGA playCardGA = new(Card, targets);
            ActionSystem.Instance.AddReaction(playCardGA);
        }
        else if (hasDropArea)
        {
            PlayCardGA playCardGA = new(Card);
            ActionSystem.Instance.AddReaction(playCardGA);
        }
        else
        {
            transform.position = dragStartPosition;
            transform.rotation = Quaternion.Euler(0, 0, dragStartRotationZ);
        }

        Interactions.Instance.PlayerIsDragging = false;
        isDraggingAllowed = false; // Reset flag
    }
}