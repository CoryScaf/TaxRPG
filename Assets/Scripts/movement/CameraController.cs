using System.Collections;
using System.Collections.Generic;
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
    public float shakeDuration = 0.5f;  // Duration of the shake effect
    public float shakeMagnitude = 0.5f;  // Intensity of the shake effect
    private float shakeInterval = 0.05f;  // Delay between each shake

    private bool shouldClampPosition = false;
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
    }
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
    public void TriggerShake()
    {
        StartCoroutine(ShakeCoroutine());
    }
    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;

        Vector3 originalPosition = transform.position;

        while (elapsed < shakeDuration)
        {
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(shakeInterval);  // Wait for the shakeInterval duration before the next shake
        }

        transform.position = originalPosition;
    }
}
