using UnityEngine;

public class ObjectMoverByHandPinch : MonoBehaviour
{
    public Transform TargetObject;
    public float moveSpeed = 5f;
    public float momentumDuration = 0.5f;
    public LineRenderer lineRenderer; // Assign this in the Unity Inspector.

    public enum CollisionLayer
    {
        Default = 0,
        TransparentFX = 1,
        Water = 4,
        UI = 5,
        CustomLayer1 = 8,
        CustomLayer2 = 9
    }
    public CollisionLayer collisionLayer = CollisionLayer.Default;

    public enum HandControlMode
    {
        BothHands,
        LeftHand,
        RightHand
    }
    public HandControlMode controlMode = HandControlMode.BothHands;

    private Vector3 lastPosition;
    private Vector3 momentum;
    private float momentumTimer;
    private bool isLeftHandPinching = false;
    private bool isRightHandPinching = false;

    void Update()
    {
        isLeftHandPinching = controlMode != HandControlMode.RightHand && TXRPlayer.Instance.HandLeft.PinchManager.IsHandPinchingThisFrame();
        isRightHandPinching = controlMode != HandControlMode.LeftHand && TXRPlayer.Instance.HandRight.PinchManager.IsHandPinchingThisFrame();

        Transform activeHand = null;
        Vector3 handForward = Vector3.zero;

        if (isLeftHandPinching && !isRightHandPinching)
        {
            //activeHand = TXRPlayer.Instance.LeftHand.transform;
            activeHand = TXRPlayer.Instance.HandLeft.PinchManager.Pincher.transform;
            handForward = activeHand.up;
        }
        else if (isRightHandPinching && !isLeftHandPinching)
        {
            activeHand = TXRPlayer.Instance.RightHand.transform;
            activeHand = TXRPlayer.Instance.HandRight.PinchManager.Pincher.transform;
            handForward = -activeHand.up;
        }

        if (activeHand != null)
        {
            Vector3 handPosition = activeHand.position;
            int layerMask = 1 << (int)collisionLayer;
            Debug.DrawRay(handPosition, handForward * 10f, Color.green);

            Ray ray = new Ray(handPosition, handForward);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Vector3 movementSinceLastFrame = hit.point - lastPosition;
                momentum = movementSinceLastFrame / Time.deltaTime;
                momentumTimer = momentumDuration;
                lastPosition = hit.point;

                // Set the positions for the LineRenderer to draw the line.
                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, handPosition);
                    lineRenderer.SetPosition(1, hit.point);
                }
            }
            else if (lineRenderer != null)
            {
                // Optionally, hide the line if no hit is detected.
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
            }
        }

        if (!isLeftHandPinching && !isRightHandPinching && momentumTimer > 0)
        {
            lastPosition += momentum * Time.deltaTime;
            momentumTimer -= Time.deltaTime;
        }

        if (TargetObject != null && lastPosition != Vector3.zero)
        {
            TargetObject.position = Vector3.Lerp(TargetObject.position, lastPosition, Time.deltaTime * moveSpeed);
        }
    }
}
