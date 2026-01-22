using UnityEngine;

// Ensure there's a SphereCollider component on the game object this script is attached to
[RequireComponent(typeof(SphereCollider))]
public class SphereColliderDetection : MonoBehaviour
{
    // This will hold the reference to the game object that has the IKLerpTowards script
    public GameObject ikLerpTowardsObject;

    private void Start()
    {
        // Optionally, ensure the Sphere Collider is marked as a trigger if you haven't already done so in the Unity editor
        GetComponent<SphereCollider>().isTrigger = true;
    }

    // This function is called when another collider makes contact with the sphere collider (marked as trigger)
    // This function is called when another collider makes contact with the sphere collider (marked as trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Attempt to access the IKLerpTowards script on the specified game object
        IKLerpTowards ikLerpTowards = ikLerpTowardsObject.GetComponent<IKLerpTowards>();

        if (ikLerpTowards != null)
        {
            // Adjusted line: Set its TargetObject variable to the Transform of the game object that collided with the sphere
            ikLerpTowards.TargetObject = other.gameObject.transform;
        }
        else
        {
            Debug.LogError("IKLerpTowards script not found on the assigned game object.");
        }
    }

}
