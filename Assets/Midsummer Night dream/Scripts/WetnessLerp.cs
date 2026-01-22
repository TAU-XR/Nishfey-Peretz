using UnityEngine;
using UnityEngine.Events;

public class WetnessLerp : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string wetnessProperty = "_Wetness";
    [SerializeField] private float lerpDuration = 2.0f;
    [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private UnityEvent onFirstTrigger;

    private float lerpTime;
    private bool isLerping = false;
    private bool hasTriggered = false;
    private float initialWetness;

    private void OnTriggerEnter(Collider other)
    {
        if (!isLerping && !hasTriggered)
        {
            isLerping = true;
            initialWetness = targetRenderer.material.GetFloat(wetnessProperty);
            lerpTime = 0;
            hasTriggered = true;
            onFirstTrigger.Invoke();  // Trigger the event only on the first trigger
        }
    }

    private void Update()
    {
        if (isLerping)
        {
            lerpTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(lerpTime / lerpDuration);
            float curveValue = lerpCurve.Evaluate(lerpFactor);
            float newWetness = Mathf.Lerp(initialWetness, 1, curveValue);

            targetRenderer.material.SetFloat(wetnessProperty, newWetness);

            if (lerpFactor >= 1)
            {
                isLerping = false;
            }
        }
    }
}
