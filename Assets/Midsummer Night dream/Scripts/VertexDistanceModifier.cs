using UnityEngine;
using System.Collections.Generic;


public class VertexDistanceModifier : MonoBehaviour
{
    public Transform targetMeshTransform;  // The mesh to check distance from
    public float distanceThreshold = 1.0f; // Distance threshold
    public float dataIncrement = 0.1f;     // Increment value for custom data

    void Start()
    {
        if (targetMeshTransform == null)
        {
            Debug.LogError("Target mesh transform is not assigned.");
            return;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Create an array to store custom data for each vertex, initialized to zero
        Vector3[] vertices = mesh.vertices;
        Vector4[] customData = new Vector4[vertices.Length];

        for (int i = 0; i < customData.Length; i++)
        {
            customData[i] = Vector4.zero; // Initialize custom data to zero
        }

        // Check distance of each vertex from the target mesh
        Vector3 targetPosition = targetMeshTransform.position;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], targetPosition);
            if (distance < distanceThreshold)
            {
                customData[i] += new Vector4(dataIncrement, dataIncrement, dataIncrement, dataIncrement);
            }
        }

        // Add the custom data to the mesh as UV2 or any other available attribute
        mesh.SetUVs(1, new List<Vector4>(customData));
        meshFilter.mesh = mesh;
    }
}
