using UnityEngine;

public class ProjectileLaunch : MonoBehaviour
{
    public Transform Target; // Public variable to assign your target GameObject.
    public float launchAngle = 45.0f; // Launch angle in degrees.
    public float noiseAmount = 1.0f; // The amount of noise to add to the target position on the X and Z axes.

    private Rigidbody rb; // Rigidbody component of the sphere.

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to this sphere.

        LaunchProjectile();
    }

    void LaunchProjectile()
    {
        if (Target != null)
        {
            // Calculate a noise offset for X and Z based on the noiseAmount.
            float noiseOffsetX = Random.Range(-noiseAmount, noiseAmount);
            float noiseOffsetZ = Random.Range(-noiseAmount, noiseAmount);

            // Apply the noise offset to the target's position.
            Vector3 targetPositionWithNoise = Target.position + new Vector3(noiseOffsetX, 0, noiseOffsetZ);

            // Distance to target with noise
            float distanceToTarget = Vector3.Distance(transform.position, targetPositionWithNoise);

            // Convert launch angle to radians
            float launchAngleRadians = launchAngle * Mathf.Deg2Rad;

            // Calculate initial velocity required to land the projectile on the target point.
            float gravity = Physics.gravity.magnitude; // Get the gravity value from Unity Physics.
            float velocity = Mathf.Sqrt(distanceToTarget * gravity / Mathf.Sin(2 * launchAngleRadians));

            // Calculate velocity vector
            Vector3 velocityVector = new Vector3(0, velocity * Mathf.Sin(launchAngleRadians), velocity * Mathf.Cos(launchAngleRadians));

            // Rotate the velocity vector towards the target with noise
            velocityVector = Quaternion.LookRotation(targetPositionWithNoise - transform.position) * velocityVector;

            // Apply the calculated velocity (projectile motion)
            rb.velocity = velocityVector;
        }
    }
}
