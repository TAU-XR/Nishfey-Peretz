using UnityEngine;
using UnityEngine.Events;

public class SphereRoller : MonoBehaviour
{
    public float sphereRadius = 1.0f;
    public float velocityThreshold = 1.0f;
    public UnityEvent onStartRotating;
    public UnityEvent onStopRotating;

    private Rigidbody parentRb;
    private Transform childTransform;
    private Animator childAnimator;
    private bool isRotating = false;
    private Vector3 previousPosition;
    private Vector3 velocity;
    private bool shouldSmoothReturn = false; // Indicates if we should start smoothing back to (0,0,0)
    private float smoothReturnSpeed = 2f; // Speed at which the object returns to (0,0,0) rotation

    void Start()
    {
        parentRb = GetComponent<Rigidbody>();
        childTransform = transform.childCount > 0 ? transform.GetChild(0) : null;
        previousPosition = transform.position;

        if (childTransform != null)
        {
            childAnimator = childTransform.GetComponent<Animator>();
            if (childAnimator == null)
            {
                Debug.LogError("No Animator component found on the child object! Please add one with a 'Curled' layer.");
            }
        }
        else
        {
            Debug.LogError("No child object found! Please attach a child object to this parent.");
        }
    }

    void FixedUpdate()
    {
        if (parentRb != null && !parentRb.isKinematic)
        {
            velocity = parentRb.velocity;
        }
        else
        {
            Vector3 currentPosition = transform.position;
            velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
            previousPosition = currentPosition;
        }
    }

    void Update()
    {
        float speed = velocity.magnitude;
        Debug.Log("Velocity: " + velocity);

        if (childTransform != null)
        {
            if (speed > velocityThreshold)
            {
                if (!isRotating)
                {
                    onStartRotating.Invoke();
                    isRotating = true;
                    shouldSmoothReturn = false;
                    if (childAnimator != null) childAnimator.SetLayerWeight(childAnimator.GetLayerIndex("Curled"), 1);
                }

                float rotationAmount = (speed / (2 * Mathf.PI * sphereRadius)) * 360;
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, velocity).normalized;
                childTransform.Rotate(rotationAxis, rotationAmount * Time.deltaTime, Space.World);
            }
            else if (isRotating)
            {
                onStopRotating.Invoke();
                isRotating = false;
                shouldSmoothReturn = true; // Start the smooth return to (0,0,0)
                if (childAnimator != null) childAnimator.SetLayerWeight(childAnimator.GetLayerIndex("Curled"), 0);
            }

            // If rotation should smoothly return to (0,0,0)
            if (shouldSmoothReturn && !isRotating)
            {
                childTransform.rotation = Quaternion.Lerp(childTransform.rotation, Quaternion.identity, Time.deltaTime * smoothReturnSpeed);
                if (Quaternion.Angle(childTransform.rotation, Quaternion.identity) < 1.0f)
                {
                    childTransform.rotation = Quaternion.identity; // Snap to identity to avoid overshooting
                    shouldSmoothReturn = false;
                }
            }
        }
    }
}
