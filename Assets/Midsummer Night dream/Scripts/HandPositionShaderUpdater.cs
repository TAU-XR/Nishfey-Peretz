using UnityEngine;

public class HandPositionShaderUpdater : MonoBehaviour
{
    // Public references to the two GameObjects
    public GameObject rightHand;
    public GameObject leftHand;

    // Reference to the Renderer component of the object with the shader
    private Renderer objectRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component of the object this script is attached to
        objectRenderer = GetComponent<Renderer>();

        // Check if the Renderer component is found
        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component not found on the object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if both GameObjects are assigned
        if (rightHand != null && leftHand != null)
        {
            // Get the positions of the GameObjects
            Vector3 rightHandPos = rightHand.transform.position;
            Vector3 leftHandPos = leftHand.transform.position;

            // Set the positions in the shader
            objectRenderer.material.SetVector("_RightHandPos", rightHandPos);
            objectRenderer.material.SetVector("_LeftHandPos", leftHandPos);
        }
        else
        {
            Debug.LogWarning("Please assign both Right Hand and Left Hand GameObjects in the inspector.");
        }
    }
}
