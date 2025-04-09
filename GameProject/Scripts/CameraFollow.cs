using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    private Vector3 currentVelocity;
    private Vector3 desiredPosition;
    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirX;
    private bool lookAheadStopped;

    void LateUpdate()
    {
        if (target == null)
            return;

        float moveX = target.GetComponent<Rigidbody2D>().velocity.x;

        // Update look ahead
        if (Mathf.Abs(moveX) > lookAheadMoveThreshold)
        {
            lookAheadDirX = Mathf.Sign(moveX);
            lookAheadStopped = false;
        }
        else
        {
            if (!lookAheadStopped)
            {
                lookAheadStopped = true;
                targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadFactor - currentLookAheadX) / lookAheadReturnSpeed;
            }
        }

        targetLookAheadX = lookAheadDirX * lookAheadFactor;
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref currentVelocity.x, smoothSpeed);

        // Calculate and set camera position
        desiredPosition = target.position + offset;
        desiredPosition.x += currentLookAheadX;
        
        // Keep camera above ground level
        if (desiredPosition.y < offset.y)
        {
            desiredPosition.y = offset.y;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
