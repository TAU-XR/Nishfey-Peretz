using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FlowManager : MonoBehaviour
{
    public static FlowManager Instance { get; private set; }

    public bool StartGrassCircle = false;
    public bool FinishedGrassCircle = false;
    public bool FinishedScalingEnv = false;
    public GameObject Cowslips;
    public GameObject EnviromentPivot;
    public float EnviromentScaleTarget = 5f;
    private MeshRenderer OverlayScreen;

    // New Public Variables for Material Modification
    public List<Material> targetMaterials; // List of materials to modify
    public List<string> propertyNames;     // List of property names (must match shader properties)
    public float multiplier = 1.5f;         // Multiplier for the properties when scaling up

    // We'll store the original property values so we can revert later.
    private List<Dictionary<string, float>> originalMaterialValuesList = new List<Dictionary<string, float>>();

    private void Awake()
    {
        // Check if there is already an instance of FlowManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the OverlayScreen is correctly assigned
        OverlayScreen = TXRPlayer.Instance.colorOverlayMR;

        // Store the original values for each target material.
        if (targetMaterials != null && targetMaterials.Count > 0)
        {
            originalMaterialValuesList.Clear();
            foreach (Material mat in targetMaterials)
            {
                Dictionary<string, float> propValues = new Dictionary<string, float>();
                foreach (string prop in propertyNames)
                {
                    if (mat.HasProperty(prop))
                    {
                        propValues[prop] = mat.GetFloat(prop);
                    }
                }
                originalMaterialValuesList.Add(propValues);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameFlow();
    }

    private void SetBowAnimation()
    {
        Animator[] animators = Cowslips.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.SetBool("Bow", true);
        }
    }

    /// <summary>
    /// Modified version of ModifyMaterialsProperties that applies a given multiplier.
    /// When targetMultiplier == multiplier, properties are scaled up.
    /// When targetMultiplier == 1, properties revert to original values.
    /// </summary>
    /// <param name="targetMultiplier">Multiplier to apply (default uses the public multiplier if not specified).</param>
    public void ModifyMaterialsProperties(float targetMultiplier = -1f)
    {
        if (targetMaterials == null || propertyNames == null)
        {
            Debug.LogError("Materials and property names must be assigned.");
            return;
        }

        // Use the defined multiplier if no value is provided.
        if (targetMultiplier < 0) targetMultiplier = multiplier;

        for (int i = 0; i < targetMaterials.Count; i++)
        {
            Material originalMat = targetMaterials[i];
            if (originalMat == null)
            {
                Debug.LogWarning($"Material at index {i} is null and will be skipped.");
                continue;
            }

            // Create a runtime instance of the material.
            Material instanceMaterial = new Material(originalMat);

            // Replace references to the original material in all renderers.
            Renderer[] allRenderers = FindObjectsOfType<Renderer>();
            foreach (Renderer renderer in allRenderers)
            {
                Material[] mats = renderer.sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j] == originalMat)
                    {
                        mats[j] = instanceMaterial;
                    }
                }
                renderer.sharedMaterials = mats;
            }

            // Use the stored original values (if available) to set the new values.
            if (i < originalMaterialValuesList.Count)
            {
                foreach (string prop in propertyNames)
                {
                    if (instanceMaterial.HasProperty(prop))
                    {
                        if (originalMaterialValuesList[i].ContainsKey(prop))
                        {
                            float origValue = originalMaterialValuesList[i][prop];
                            instanceMaterial.SetFloat(prop, origValue * targetMultiplier);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Material {instanceMaterial.name} does not have property {prop}");
                    }
                }
            }

            // Update the list so further modifications work on the new instance.
            targetMaterials[i] = instanceMaterial;
        }
    }

    /// <summary>
    /// Scales the environment to the target size and modifies materials with the default multiplier.
    /// </summary>
    public void ScaleEnv()
    {
        StartCoroutine(ScaleObject(EnviromentPivot, EnviromentScaleTarget, 2.0f, () => FinishedScalingEnv = true));
        ModifyMaterialsProperties(); // uses the default multiplier value
    }

    /// <summary>
    /// Coroutine to smoothly scale an object over time.
    /// </summary>
    private IEnumerator ScaleObject(GameObject obj, float targetScale, float duration, System.Action onComplete)
    {
        Vector3 initialScale = obj.transform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScaleVector, elapsedTime / duration);
            yield return null;
        }

        // Ensure the final scale is set.
        obj.transform.localScale = targetScaleVector;
        onComplete?.Invoke();
    }

    public void LerpAlphaOverTime(float targetAlpha, float duration)
    {
        StartCoroutine(LerpAlphaCoroutine(targetAlpha, duration));
    }

    private IEnumerator LerpAlphaCoroutine(float targetAlpha, float duration)
    {
        Color initialColor = OverlayScreen.material.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            OverlayScreen.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        OverlayScreen.material.color = targetColor;
    }

    private async UniTask GameFlow()
    {
        await UniTask.WaitUntil(() => StartGrassCircle);
        await UniTask.WaitUntil(() => FinishedGrassCircle);

        LerpAlphaOverTime(1, 1);
        await UniTask.WaitUntil(() => OverlayScreen.material.color.a > 0.99);

        ScaleEnv();
        await UniTask.WaitUntil(() => FinishedScalingEnv);

        LerpAlphaOverTime(0, 1);
        await UniTask.WaitUntil(() => OverlayScreen.material.color.a < 0.01);
        await UniTask.WaitUntil(() => TXRPlayer.Instance.FocusedObject.name == "CowSlip Collider");
        SetBowAnimation();
    }

    /// <summary>
    /// Toggle function that checks the current environment scale.
    /// If at 1, scales up to the target scale and applies the multiplier.
    /// Otherwise, scales back to 1 and reverts material properties.
    /// </summary>
    public void ToggleScaleEnv()
    {
        // Assume uniform scaling: check the x component.
        float currentScale = EnviromentPivot.transform.localScale.x;
        float newTargetScale;
        float materialTargetMultiplier;

        // If the current scale is approximately 1, then scale up.
        if (Mathf.Approximately(currentScale, 1f))
        {
            newTargetScale = EnviromentScaleTarget;
            materialTargetMultiplier = multiplier; // scale materials using the multiplier
        }
        else
        {
            // Otherwise, scale back down to 1.
            newTargetScale = 1f;
            materialTargetMultiplier = 1f; // revert materials to their original values
        }

        FinishedScalingEnv = false;
        StartCoroutine(ScaleObject(EnviromentPivot, newTargetScale, 2.0f, () => FinishedScalingEnv = true));
        ModifyMaterialsProperties(materialTargetMultiplier);
    }
}
