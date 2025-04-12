using System.Collections;
using UnityEngine;

public class VisualCardScript : MonoBehaviour
{
    private Transform instantiatingObject; // The object that instantiated this prefab
    public float moveLerpSpeed = 5f; // Lerp speed for smooth movement
    public float rotateLerpSpeed = 15f; // Lerp speed for smooth rotation
    public float tiltFactor = 10f; // Factor to control the tilt amount
    private Vector3 previousPosition; // To store the previous frame's position

    // Shake parameters
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    private bool isShaking = false;
    private float shakeTimeRemaining;
    private Vector3 shakeOffset;

    // Scaling parameters
    public float toScaleUp = 1.5f;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isScaling = false;
    private float scaleLerpSpeed = 5f;

    // Rotation parameters
    public float rotationSpeed = .3f;
    [HideInInspector]
    public bool idleRotate3d = false;
    [HideInInspector]
    public bool hoverRotate3d = false;

    // Method to set the instantiating object
    public void SetInstantiatingObject(Transform instantiator)
    {
        instantiatingObject = instantiator;
        previousPosition = transform.position; // Initialize previous position
        originalScale = transform.localScale; // Store the original scale
    }

    // Update is called once per frame
    void Update()
    {
        if (instantiatingObject != null)
        {
            // Calculate the target position for smooth movement
            Vector3 targetPosition = Vector3.Lerp(transform.position, instantiatingObject.position, Time.deltaTime * moveLerpSpeed);

            // Apply the z offset
            targetPosition.z = instantiatingObject.position.z - 1f;

            // Calculate the direction and magnitude of movement
            Vector3 movement = targetPosition - previousPosition;
            float movementMagnitude = movement.magnitude;
            Vector3 direction = movement.normalized;

            // Calculate the tilt based on the direction and magnitude of movement
            float tiltAngle = direction.x * tiltFactor * movementMagnitude;

            // Calculate the target rotation based on the tilt angle
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -tiltAngle);

            // Smoothly interpolate to the target rotation using Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateLerpSpeed);

            // Update the previous position for the next frame
            previousPosition = transform.position;

            // Handle shaking
            if (isShaking)
            {
                shakeTimeRemaining -= Time.deltaTime;
                if (shakeTimeRemaining > 0)
                {
                    shakeOffset = new Vector3(
                        Random.Range(-shakeMagnitude, shakeMagnitude),
                        Random.Range(-shakeMagnitude, shakeMagnitude),
                        0f);
                }
                else
                {
                    isShaking = false;
                    shakeOffset = Vector3.zero;
                }
            }

            // Update the position of the instantiated prefab to match the target position with shake offset
            transform.position = targetPosition + shakeOffset;

            // Handle scaling
            if (isScaling)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
            }

            if (idleRotate3d)
            {
                // Rotate around the 3D axis
                float sine = Mathf.Sin(Time.time * rotationSpeed);
                float cosine = Mathf.Cos(Time.time * rotationSpeed);
                transform.rotation = Quaternion.Euler(sine * 20f, cosine * 20f, 0f);
            }
            else
            {
                // Lerp the position back to no rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotateLerpSpeed);
            }
            /*
            if (hoverRotate3d)
            {
                // Get the mouse position in world space
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = transform.position.z; // Ensure it's on the same z-plane as the object

                // Calculate the offset vector between the object's position and the mouse position
                Vector3 offset = mousePos - transform.position;

                // Calculate the tilt angles based on the offset
                float tiltX = Mathf.Atan2(offset.y, offset.z) * Mathf.Rad2Deg;
                float tiltY = -Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;

                // Create the target rotation based on the calculated tilt angles
                targetRotation = Quaternion.Euler(tiltX * 3f, tiltY * 3f, 0f);

                // Smoothly interpolate to the target rotation using Slerp
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateLerpSpeed);
            }
            else
            {
                // Lerp the position back to no rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotateLerpSpeed);
            }
            */
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }

    // Method to start the shake effect
    public void StartShake()
    {
        shakeTimeRemaining = shakeDuration;
        isShaking = true;
    }

    // Method to start scaling up
    public void StartScaleUp()
    {
        targetScale = originalScale * toScaleUp;
        isScaling = true;
    }

    // Method to start scaling down
    public void StartScaleDown()
    {
        targetScale = originalScale;
        isScaling = true;
    }
}
