using UnityEngine;

public class SpawnPrefabOnEnter : MonoBehaviour
{
    // Public variable to assign the prefab you want to instantiate
    public GameObject prefabToSpawn;

    // Public variable to set the offset for instantiation
    public Vector3 spawnOffset;

    // Public variable for the target that the instantiated prefab's ProjectileLaunch script will use
    public Transform target;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the collision is the player or the object you're interested in
        if (other.CompareTag("Player"))
        {
            // Instantiate the prefab at the object's position with the specified offset
            GameObject instance = Instantiate(prefabToSpawn, transform.position + spawnOffset, Quaternion.identity);

            // Get the ProjectileLaunch script on the instantiated prefab
            ProjectileLaunch projectileLaunchScript = instance.GetComponent<ProjectileLaunch>();

            // Check if the ProjectileLaunch script is found and assign the target
            if (projectileLaunchScript != null)
            {
                projectileLaunchScript.Target = target;
            }
            else
            {
                Debug.LogError("ProjectileLaunch script not found on the instantiated prefab.");
            }
        }
    }
}
