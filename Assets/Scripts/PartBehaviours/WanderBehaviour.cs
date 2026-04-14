using UnityEngine;

[CreateAssetMenu(fileName = "NewWanderBehaviour", menuName = "Behaviour/Parts/Wander")]
public class WanderBehaviour : PartBehaviour
{
    [SerializeField, Range(0f, 1f), Tooltip("How willing the ant is to change direction")]
    private float curiosity;

    public override Vector2 GetVelocity(Ant ant, World world) {
        return Random.insideUnitCircle * (curiosity);
    }
    
    public override void DrawInstanceGizmos(Ant ant, World world) {
        Gizmos.DrawLine(ant.position, ant.position + ant.velocity);
    }
}
