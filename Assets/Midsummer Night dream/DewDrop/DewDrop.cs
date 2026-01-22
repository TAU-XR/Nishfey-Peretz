using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DewDrop : APinchable
{
    public UnityEvent OnGrab;
    public UnityEvent OnDrop;
    private Transform originParent;
    private GameObject[] landingPoints;
    public float lerpSpeed = 2f; // Speed of the lerp (units per second)
    public AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1); // Lerp curve
    public AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 1); // Y-axis curve
    public float yCurveInfluence = 1f; // Influence of the y-axis curve
    public bool enableHovering = true; // Enable or disable hovering
    public float hoverSpeed = 1f; // Speed of the hovering effect
    public float hoverAmplitude = 0.5f; // Amplitude of the hovering effect
    public bool testTriggerLerp = false; // For testing without VR headset
    private bool hasLanded = false; // Flag to check if the object has landed
    private Vector3 originalPosition; // Original position of the object

    public override void OnPinchEnter(PinchManager pinchManager)
    {
        if (hasLanded)
            return; // Do nothing if the object has landed

        base.OnPinchEnter(pinchManager);
        transform.parent = pinchManager.Pincher.transform;
        OnGrab.Invoke();
    }

    public override void OnPinchExit()
    {
        if (hasLanded)
            return; // Do nothing if the object has landed

        base.OnPinchExit();
        // Call the LerpToNearestLandingPoint function
        LerpToNearestLandingPoint();
        OnDrop.Invoke();
    }

    void Start()
    {
        originParent = transform.parent;
        originalPosition = transform.position; // Store the original position
        // Find all landing points at the start
        landingPoints = GameObject.FindGameObjectsWithTag("Landing Point");
    }

    void Update()
    {
        // Handle hovering effect
        if (!hasLanded && enableHovering && transform.parent == originParent)
        {
            float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = originalPosition + new Vector3(0, hoverOffset, 0);
        }

        // Trigger lerp for testing without VR headset
        if (testTriggerLerp && !hasLanded)
        {
            testTriggerLerp = false; // Reset the variable to prevent multiple calls
            LerpToNearestLandingPoint();
        }
    }

    public void LerpToNearestLandingPoint()
    {
        // Find the nearest object with the tag "Landing Point"
        GameObject nearestLandingPoint = null;
        float minDistance = float.MaxValue;

        foreach (GameObject landingPoint in landingPoints)
        {
            float distance = Vector3.Distance(transform.position, landingPoint.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestLandingPoint = landingPoint;
            }
        }

        if (nearestLandingPoint != null)
        {
            float distance = Vector3.Distance(transform.position, nearestLandingPoint.transform.position);
            float duration = distance / lerpSpeed; // Calculate duration based on distance and speed
            StartCoroutine(LerpPosition(nearestLandingPoint.transform.position, nearestLandingPoint.transform, duration));
        }
    }

    private IEnumerator LerpPosition(Vector3 targetPosition, Transform newParent, float duration)
    {
        float timeElapsed = 0;
        Vector3 startPosition = transform.position;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float lerpValue = lerpCurve.Evaluate(t);
            float yLerpValue = yCurve.Evaluate(t) * yCurveInfluence; // Evaluate the y-axis curve with influence

            float newY = Mathf.Lerp(startPosition.y, targetPosition.y, lerpValue) + yLerpValue; // Apply y-axis curve with influence

            transform.position = new Vector3(
                Mathf.Lerp(startPosition.x, targetPosition.x, lerpValue),
                newY,
                Mathf.Lerp(startPosition.z, targetPosition.z, lerpValue)
            );

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.parent = newParent; // Set the new parent after lerping
        hasLanded = true; // Mark the object as landed

        // Traverse up the hierarchy to find the Animator component
        Animator animator = newParent.GetComponentInParent<Animator>();
        if (animator != null)
        {
            // Convert the name of the landing point to an integer
            int flowerNumber;
            if (int.TryParse(newParent.name, out flowerNumber))
            {
                // Set the int parameter "Flower Number" in the Animator
                animator.SetInteger("Flower Number", flowerNumber);
            }
            else
            {
                Debug.LogWarning("Landing point name is not a valid integer: " + newParent.name);
            }
        }
        else
        {
            Debug.LogWarning("Animator component not found in parent hierarchy of landing point: " + newParent.name);
        }
    }
}
