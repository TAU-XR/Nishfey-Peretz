using UnityEngine;

public class IKLerpTowards : MonoBehaviour
{
    public Transform TargetObject; // The target object to move towards.
    public Transform ReferenceObject; // The object used to calculate distance to the target.
    public float DistanceThreshold = 5f; // Distance threshold for the lerping to occur.
    public float LerpSpeed = 1f; // Speed at which to lerp.

    private Vector3 originalPosition; // To store the original position of the IK object.

    void Start()
    {
        // Capture the original position of the IK object.
        originalPosition = transform.position;
    }

    void Update()
    {
        // Check if the ReferenceObject and TargetObject have been set and are active.
        bool referenceAndTargetValid = ReferenceObject != null && TargetObject != null && ReferenceObject.gameObject.activeInHierarchy && TargetObject.gameObject.activeInHierarchy;

        // If the target object or reference object is not active or is null, lerp back to the original position.
        if (!referenceAndTargetValid)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, LerpSpeed * Time.deltaTime);
            return;
        }

        // Calculate distance from the ReferenceObject to the target object.
        float distance = Vector3.Distance(ReferenceObject.position, TargetObject.position);

        // Determine the target position based on the distance.
        Vector3 targetPosition = (distance < DistanceThreshold) ? TargetObject.position : originalPosition;

        // Lerp towards the calculated target position.
        transform.position = Vector3.Lerp(transform.position, targetPosition, LerpSpeed * Time.deltaTime);
    }
}
