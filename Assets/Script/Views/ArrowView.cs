using UnityEngine;

public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float arrowHeadOffset = 0.5f;

    private Camera mainCamera;
    private Transform cardTransform; // Track kartu (untuk mode dynamic)
    private Vector3 fixedStartPosition; // Posisi fixed (untuk mode static)
    private bool useFixedPosition = false; // Mode: true = fixed, false = follow card

    private void Awake()
    {
        mainCamera = Camera.main;
        
        if (arrowHead != null && arrowHead.transform.parent != null)
        {
            arrowHead.transform.SetParent(null);
        }
        
        // Setup LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
        }
    }

    private void Update()
    {
        if (lineRenderer == null) return;
        
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        // Gunakan posisi fixed atau posisi kartu yang bergerak
        Vector3 startPos;
        if (useFixedPosition)
        {
            startPos = fixedStartPosition;
        }
        else if (cardTransform != null)
        {
            startPos = cardTransform.position;
            startPos.z = 0;
        }
        else
        {
            return; // Tidak ada posisi yang valid
        }
        
        Vector3 direction = (mousePos - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Vector3 arrowPos = mousePos - (direction * arrowHeadOffset);
        
        arrowHead.transform.position = arrowPos;
        arrowHead.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Line dari start ke mouse
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, mousePos);
        
        // Debug log
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"[ArrowView] Start: {startPos}, Mouse: {mousePos}");
        }
    }

    // Setup dengan posisi FIXED (tidak follow kartu)
    public void SetupArrowFixed(Vector3 fixedPosition)
    {
        this.fixedStartPosition = fixedPosition;
        this.fixedStartPosition.z = 0;
        this.useFixedPosition = true;
        
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, this.fixedStartPosition);
            lineRenderer.SetPosition(1, this.fixedStartPosition);
        }
    }

    // Setup dengan Transform (follow kartu yang bergerak)
    public void SetupArrow(Transform cardTransform)
    {
        this.cardTransform = cardTransform;
        this.useFixedPosition = false;
        
        if (lineRenderer != null && cardTransform != null)
        {
            Vector3 cardPos = cardTransform.position;
            cardPos.z = 0;
            lineRenderer.SetPosition(0, cardPos);
            lineRenderer.SetPosition(1, cardPos);
        }
    }
    
    private void OnDestroy()
    {
        if (arrowHead != null)
        {
            Destroy(arrowHead);
        }
    }
}