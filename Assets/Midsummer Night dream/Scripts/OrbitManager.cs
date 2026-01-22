using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    public List<Renderer> objects; // List of objects to modify
    private List<Renderer> removedObjects = new List<Renderer>(); // List of removed objects
    public GameObject fullOrbit; // Object to activate when all objects are removed
    public float speed = 1f; // Speed of the WetnessGlow effect
    public float waitTime = 1f; // Time to wait before moving to the next object

    private Coroutine wetnessCoroutine;

    void Start()
    {
        // Set Full Orbit's wetness to 1
        if (fullOrbit != null)
        {
            Renderer fullOrbitRenderer = fullOrbit.GetComponent<Renderer>();
            if (fullOrbitRenderer != null)
            {
                fullOrbitRenderer.material.SetFloat("_Wetness", 1f);
            }
        }

        wetnessCoroutine = StartCoroutine(ApplyWetnessGlow());
    }

    public void RemoveObject(Renderer obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
            removedObjects.Add(obj);

            // Check if all objects have been removed
            if (objects.Count == 0)
            {
                StopCoroutine(wetnessCoroutine);
                DisableAllObjects();
                if (fullOrbit != null)
                {
                    fullOrbit.SetActive(true);
                    FlowManager.Instance.FinishedGrassCircle = true; // Set FinishedGrassCircle to true
                }
            }
        }
    }

    void DisableAllObjects()
    {
        foreach (Renderer obj in removedObjects)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ApplyWetnessGlow()
    {
        while (true)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                Renderer obj = objects[i];
                if (obj != null)
                {
                    StartCoroutine(WetnessGlow(obj));
                }
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    IEnumerator WetnessGlow(Renderer obj)
    {
        float elapsedTime = 0f;
        float duration = 1f / speed; // Duration of the effect
        float wetness = 0f;

        // Lerp from 0 to -1
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            wetness = Mathf.Lerp(0f, -1f, elapsedTime / duration);
            obj.material.SetFloat("_Wetness", wetness);
            yield return null;
        }

        // Reset elapsedTime for reverse lerp
        elapsedTime = 0f;

        // Lerp back from -1 to 0
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            wetness = Mathf.Lerp(-1f, 0f, elapsedTime / duration);
            obj.material.SetFloat("_Wetness", wetness);
            yield return null;
        }
    }
}
