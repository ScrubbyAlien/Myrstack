using UnityEngine;

[CreateAssetMenu(fileName = "NewAvoidanceBehaviour", menuName = "Behaviour/Parts/Avoidance")]
public class AvoidanceBehaviour : PartBehaviour
{
    [SerializeField, Tooltip("Ant will only avoid other ants within radius")]
    private float avoidanceRadius;
    [SerializeField, Range(0f, 360f), Tooltip("The width of the field of view of the ant, 360 means all the way around")]
    private float fieldOfView;
    private float halfFov => fieldOfView / 2f;

    public override float GetAngularVelocity(Ant ant, World world) {
        float sum = 0;
        foreach (Ant otherAnt in world.allAnts) {
            if (ant == otherAnt) continue;
            
            Vector2 diff = otherAnt.position - ant.position;
            if (diff.sqrMagnitude > avoidanceRadius * avoidanceRadius) continue; // too far away
            Vector2 toOther = diff.normalized;
            
            // positive angle is counter clockwise
            float angle = Vector2.SignedAngle(ant.forward, toOther);
            if (angle < -halfFov || angle > halfFov) continue; // outside fov

            // scale by distance and how directly ant is pointing to other
            float imminence = Mathf.Clamp(Vector2.Dot(ant.forward, toOther), 0f, 1f) / diff.magnitude;
            
            // negate angle to turn away
            sum += -angle * imminence;
        }
        return sum;
    }
}
