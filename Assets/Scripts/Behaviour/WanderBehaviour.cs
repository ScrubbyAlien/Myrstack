using UnityEngine;

[CreateAssetMenu(fileName = "NewWanderBehaviour", menuName = "Behaviour/Wander")]
public class WanderBehaviour : AntBehaviour
{
    [Header("Wander")]
    [SerializeField]
    private float wanderWeight = 1f;
    [SerializeField, Min(0)]
    private float speed;
    [SerializeField, Range(0f, 1f), Tooltip("How willing the ant is to change direction")]
    private float curiosity;

    [Header("Avoidance")]
    [SerializeField, Tooltip("How much will the ant avoid other ants")]
    private float avoidanceWeight = 1f;
    [SerializeField, Tooltip("Ant will only avoid other ants within radius")]
    private float avoidanceRadius;
    
    [Header("Attractor")]
    [SerializeField]
    private float attractorWeight = 1f;
    [SerializeField, Tooltip("Points in the world that the ant will move toward")]
    private Vector2[] attractors;

    public override Vector2 GetVelocity(Ant ant, World world) {
        Vector2 wanderVelocity = GetWanderVelocity(ant);
        Vector2 avoidanceVelocity = GetAvoidanceVelocity(ant, world);
        Vector2 attractorVelocity = GetAttractionVelocity(ant);

        Vector2 weightedSum = GetWeightedSum(
            new Vector2Weight(wanderVelocity, wanderWeight),
            new Vector2Weight(avoidanceVelocity, avoidanceWeight),
            new Vector2Weight(attractorVelocity, attractorWeight)
        );
        
        return weightedSum.normalized * speed;
    }

    private Vector2 GetWanderVelocity(Ant ant) {
        return ant.forward + Random.insideUnitCircle * (curiosity);
    }

    private Vector2 GetAttractionVelocity(Ant ant) {
        Vector2 sum = Vector2.zero;
        foreach (Vector2 attractor in attractors) {
            sum += (attractor - ant.position);
        }
        return sum / attractors.Length;
    }

    private Vector2 GetAvoidanceVelocity(Ant ant, World world) {
        Vector2 sum = Vector2.zero;
        foreach (Ant otherAnt in world.allAnts) {
            if (ant == otherAnt) continue;
            Vector2 diff = ant.position - otherAnt.position;
            if (diff.sqrMagnitude > avoidanceRadius * avoidanceRadius) continue;
            sum += diff / diff.sqrMagnitude;
        }
        return sum;
    }

    public override void DrawInstanceGizmos(Ant ant, World world) {
        Gizmos.DrawLine(ant.position, ant.position + ant.velocity);
    }

    public override void DrawStaticGizmos() {
        Gizmos.color = Color.red;
        foreach (Vector2 attractor in attractors) {
            Gizmos.DrawSphere(attractor, 0.05f);
        }
    }
}
