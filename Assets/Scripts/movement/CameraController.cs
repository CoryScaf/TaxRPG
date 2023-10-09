using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;  // Player's Transform
    public Vector2 offset;    // Offset from the player
    public float smoothSpeed = 0.125f;  // Dampening factor for smooth movement
    public float lookAheadFactor = 0.5f;  // How much the camera should look ahead
    private Vector2 lookAhead;
    private Vector2 targetLookAhead;
    private Vector2 currentVelocity;

    private bool shouldClampPosition = false;

    private void FixedUpdate()
    {
        Vector2 desiredPosition = (Vector2)target.position + offset + lookAhead;
        Vector2 smoothedPosition = Vector2.SmoothDamp((Vector2)transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
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

        // Look ahead logic
        Vector2 moveDirection = target.GetComponent<Rigidbody2D>().velocity.normalized;
        targetLookAhead = moveDirection * lookAheadFactor;
        lookAhead = Vector2.Lerp(lookAhead, targetLookAhead, smoothSpeed);
    }
}
