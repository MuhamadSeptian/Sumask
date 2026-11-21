using UnityEngine;

// ManualTargetSystem - Sistem lama, tidak digunakan lagi
// Sekarang menggunakan drag & drop system di CardView
public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;

    public void StartTargeting(Vector3 startPosition)
    {
        // Sistem lama - tidak digunakan
        Debug.LogWarning("[ManualTargetSystem] Sistem ini sudah tidak digunakan. Gunakan drag & drop di CardView.");
    }

    public EnemyView EndTargeting(Vector3 endPosition)
    {
        // Sistem lama - tidak digunakan
        return null;
    }
}