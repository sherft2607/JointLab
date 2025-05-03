using UnityEngine;

public class RobotJointGizmos : MonoBehaviour
{
    public Transform baseTransform; // Base of the robot
    public float joint1Range = 165f; // Degrees
    public float joint2Range = 110f; // Degrees
    public float joint3Range = 70f; // Degrees
    public float armReach = 1.2f; // Approximate max reach in meters

    private void OnDrawGizmos()
    {
        if (baseTransform == null) return;

        Gizmos.color = Color.green;

        // Draw a sphere to represent the maximum reach of the robot
        Gizmos.DrawWireSphere(baseTransform.position, armReach);

        // Draw lines to represent joint ranges
        Gizmos.color = Color.red;
        Vector3 forward = baseTransform.forward * armReach;
        Gizmos.DrawLine(baseTransform.position, baseTransform.position + Quaternion.Euler(0, joint1Range, 0) * forward);
        Gizmos.DrawLine(baseTransform.position, baseTransform.position + Quaternion.Euler(0, -joint1Range, 0) * forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(baseTransform.position, baseTransform.position + Quaternion.Euler(joint2Range, 0, 0) * forward);
        Gizmos.DrawLine(baseTransform.position, baseTransform.position + Quaternion.Euler(-joint2Range, 0, 0) * forward);
    }
}
