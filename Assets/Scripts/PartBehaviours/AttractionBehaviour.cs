using UnityEngine;

[CreateAssetMenu(fileName = "NewAttractionBevahiour", menuName = "Behaviour/Parts/Attraction")]
public class AttractionBehaviour : PartBehaviour
{
    [SerializeField, Tooltip("Points in the world that the ant will move toward")]
    private Vector2[] attractors;
    
    public override Vector2 GetVelocity(Ant ant, World _) {
        if (attractors.Length == 0) return Vector2.zero;
        
        Vector2 sum = Vector2.zero;
        foreach (Vector2 attractor in attractors) {
            sum += (attractor - ant.position);
        }
        return sum / attractors.Length;
    }
    
    public override void DrawStaticGizmos() {
        if (attractors == null) return;
        Gizmos.color = Color.red;
        foreach (Vector2 attractor in attractors) {
            Gizmos.DrawSphere(attractor, 0.05f);
        }
    }
}
