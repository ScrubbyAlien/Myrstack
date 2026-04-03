using UnityEngine;

[CreateAssetMenu(fileName = "NewWanderBehaviour", menuName = "Behaviour/Wander")]
public class WanderBehaviour : AntBehaviour
{
    [SerializeField, Tooltip("The distance in front of the ant where the origin of the circle is projected")]
    private float offset;
    [SerializeField, Tooltip("The radius of the projected circle")]
    private float radius;
    [SerializeField, Tooltip("The length of step around projected circle")]
    private float delta;
    [SerializeField, Tooltip("How strongly the attractor points will attract ants")]
    private float attractorStrength = 1;
    [SerializeField, Tooltip("Points in the world that the ant will move toward")]
    private Vector2[] attractors;

    public override Vector2 GetVelocity(Ant ant, World world) {
        WanderData data = ant.GetBehaviourData<WanderData>();

        Vector2 fromOrigin = new Vector2(Mathf.Cos(data.radians), Mathf.Sin(data.radians)) * radius;
        Vector2 wanderVelocity = (ant.forward * offset) + fromOrigin;
        
        // step around circle and store data in agent
        float randomgSign = Random.Range(-1, 2);
        data.Step(delta * randomgSign, radius);
        ant.StoreBehaviourData(data);
        
        // attract 
        Vector2 sum = Vector2.zero;
        foreach (Vector2 attractor in attractors) {
            sum += (attractor - ant.position) * attractorStrength;
        }
        Vector2 attractorVelocity = sum / attractors.Length;
        
        return (wanderVelocity + attractorVelocity) / 2;
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

    private class WanderData
    {
        public float radians;
        
        public WanderData() {
            radians = Random.Range(0, 2 * Mathf.PI);
        }

        public void Step(float delta, float radius) {
            radians += delta / radius;
        }
    }
}
