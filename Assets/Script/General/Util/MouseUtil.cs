using UnityEngine;

public static class MouseUtil
{
    private static Camera camera;

    // Metode 2D: Langsung convert mouse position ke world space
    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        if (camera == null)
            camera = Camera.main;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -camera.transform.position.z + zValue; // Jarak dari kamera ke plane Z
        return camera.ScreenToWorldPoint(mousePos);
    }

    // Alternatif: Return Vector2 untuk game 2D murni
    public static Vector2 GetMousePosition2D()
    {
        if (camera == null)
            camera = Camera.main;

        Vector3 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mousePos.x, mousePos.y);
    }
}
