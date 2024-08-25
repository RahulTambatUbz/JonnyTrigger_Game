using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    public Transform leftFootIKTarget;
    public Transform rightFootIKTarget;
    public Transform leftFoot;
    public Transform rightFoot;
    public LayerMask groundLayer;
    public float raycastDistance = 2f;
    public float footOffset = 0.1f; // Offset to avoid sinking the foot into the ground

    private void Update()
    {
        AdjustFootTarget(leftFoot, leftFootIKTarget);
        AdjustFootTarget(rightFoot, rightFootIKTarget);
    }

    private void AdjustFootTarget(Transform foot, Transform ikTarget)
    {
        RaycastHit hit;
        Vector3 rayOrigin = foot.localPosition + Vector3.up * raycastDistance;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // Ground detected, place foot on the ground
            Vector3 targetPosition = hit.point;
            targetPosition.y += footOffset; // Adjust the height by footOffset
            ikTarget.localPosition = targetPosition;
        }
        else
        {
            // No ground detected, maintain current IK position or extend leg toward ground
            Vector3 airTargetPosition = ikTarget.localPosition;
            airTargetPosition.y -= raycastDistance; // Or use any logic to extend toward ground
            ikTarget.localPosition = airTargetPosition;
        }
    }
}
