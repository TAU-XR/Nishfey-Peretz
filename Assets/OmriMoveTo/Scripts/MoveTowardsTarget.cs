using UnityEngine;

public class MoveTowardsTarget : MonoBehaviour
{
    // Public variable for the target object the script will move towards.
    public Transform target;

    // Public variable to control the speed of the movement.
    public float speed = 5f;

    // Public variable for the rotation speed towards the target.
    public float rotationSpeed = 5f;

    // Public variable for the distance threshold from the target object.
    public float stopDistance = 1f;

    // Public boolean to decide if the target position should always be updated.
    public bool alwaysUpdateTargetPosition = false;

    // Variable to hold the updated target position.
    private Vector3 updatedTargetPosition;

    // General variable to decide if it should update the target position.
    private bool shouldUpdateTargetPosition = false;

    void Start()
    {
        // Initialize updatedTargetPosition with the target's current position at the start.
        updatedTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
    }

    void Update()
    {
        // Check the condition for updating the target position.
        CheckUpdateCondition();

        // Update the target position based on the condition.
        if (alwaysUpdateTargetPosition || shouldUpdateTargetPosition)
        {
            updatedTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        }

        // Always move towards the updated target position.
        MoveTowardsUpdatedTarget();
    }

    void MoveTowardsUpdatedTarget()
    {
        // Check the distance to the updated target position.
        float distanceToTarget = Vector3.Distance(transform.position, updatedTargetPosition);

        // If the object is further than the stop distance, move towards the target.
        if (distanceToTarget > stopDistance)
        {
            // Calculate the step size based on speed and frame time.
            float step = speed * Time.deltaTime;

            // Move the object closer to the updated target position.
            transform.position = Vector3.MoveTowards(transform.position, updatedTargetPosition, step);

            // Calculate the direction vector from the current position to the updated target position.
            Vector3 targetDirection = updatedTargetPosition - transform.position;

            // Ensure we only rotate if there's a valid direction to rotate towards.
            if (targetDirection != Vector3.zero)
            {
                // Create a quaternion (rotation) based on looking down the vector from the object to the target.
                Quaternion lookRotation = Quaternion.LookRotation(targetDirection);

                // Rotate the object to face the target object smoothly over time.
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
    
    void CheckUpdateCondition()
    {
        // Example condition: space bar is pressed. This can be replaced with any condition.
        //shouldUpdateTargetPosition = Input.GetKey(KeyCode.Space);
        shouldUpdateTargetPosition = (TXRPlayer.Instance.HandLeft.PinchManager.IsHandPinchingThisFrame()) || (TXRPlayer.Instance.HandRight.PinchManager.IsHandPinchingThisFrame());

        
    }
}
