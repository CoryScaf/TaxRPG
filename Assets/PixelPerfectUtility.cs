using UnityEngine;

public static class PixelPerfectUtility
{
    public static float CalculateUnitsPerPixel()
    {
        float cameraHeight = Camera.main.orthographicSize * 2; 
        return cameraHeight / Screen.height;
    }

    public static Vector3 ClampToPixelGrid(Vector3 position)
    {
        float unitsPerPixel = CalculateUnitsPerPixel();
        float x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
        float y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
        return new Vector3(x, y, position.z);
    }

    public static Vector2 ClampToPixelGrid(Vector2 position)
    {
        float unitsPerPixel = CalculateUnitsPerPixel();
        float x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
        float y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
        return new Vector2(x, y);
    }
}
