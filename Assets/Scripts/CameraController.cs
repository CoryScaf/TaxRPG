using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // Player's Transform
    public Vector2 offset;    // Offset from the player
    public float smoothSpeed = 0.125f;  // Dampening factor for smooth movement

    private bool shouldClampPosition = false;

    private void FixedUpdate()
    {
        Vector2 desiredPosition = (Vector2)target.position + offset;
        Vector2 smoothedPosition = Vector2.Lerp((Vector2)transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
        shouldClampPosition = true;
    }

    private void LateUpdate()
    {
        if(shouldClampPosition)
        {
            transform.position = PixelPerfectUtility.ClampToPixelGrid(transform.position);
            shouldClampPosition = false;
        }
    }

    private float CalculateUnitsPerPixel()
    {
        float cameraHeight = Camera.main.orthographicSize * 2; 
        return cameraHeight / Screen.height;
    }

    private Vector3 ClampToPixelGrid(Vector3 position)
    {
        float unitsPerPixel = CalculateUnitsPerPixel();
        float x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
        float y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
        return new Vector3(x, y, position.z);
    }
}
