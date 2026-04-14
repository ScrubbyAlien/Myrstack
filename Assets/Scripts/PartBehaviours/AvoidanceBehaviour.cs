using UnityEngine;

[CreateAssetMenu(fileName = "NewAvoidanceBehaviour", menuName = "Behaviour/Parts/Avoidance")]
public class AvoidanceBehaviour : PartBehaviour
{
    [SerializeField, Tooltip("Ant will only avoid other ants within radius")]
    private float avoidanceRadius;
    [SerializeField, Tooltip("The radius below which distances will be treated as if they were at minimum this radius")]
    private float minRadius;
    [SerializeField, Range(0f, 360f), Tooltip("The width of the field of view of the ant, 360 means all the way around")]
    private float fieldOfView;
    private float halfFov => fieldOfView / 2f;

    public override Vector2 GetVelocity(Ant ant, World world) {
        Vector2 sum = Vector2.zero;
        foreach (Ant otherAnt in world.allAnts) {
            if (ant == otherAnt) continue;
            
            Vector2 diff = otherAnt.position - ant.position;
            if (diff.sqrMagnitude > avoidanceRadius * avoidanceRadius) continue; // too far away
            float angle = Vector2.SignedAngle(ant.forward, diff);
            if (angle < -halfFov || angle > halfFov) continue; // outside fov
            
            // make sure the distance is no less than minRadius;
            float magniutde = Mathf.Max(diff.magnitude, minRadius);
            // positive if to the left, negative if to the right 
            float avoidanceDirection = -Mathf.Sign(Vector2.Dot(ant.right, diff));
            
            sum += ant.right * avoidanceDirection / magniutde;
        }
        return sum;
    }

    public override void DrawInstanceGizmos(Ant ant, World _) {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ant.position, avoidanceRadius);
    }
}
