using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttractionBevahiour", menuName = "Behaviour/Parts/Attraction")]
public class AttractionBehaviour : PartBehaviour
{
    [SerializeField, Tooltip("Points in the world that the ant will move toward")]
    private Attractor[] attractors;
    
    public override float GetAngularVelocity(Ant ant, World _) {
        if (attractors.Length == 0) return 0;

        float sum = 0;
        int active = 0;
        foreach (Attractor attractor in attractors) {
            Vector2 diff = attractor.origin - ant.position;
            if (diff.sqrMagnitude < attractor.minDistanceSqr) continue;
            float angle = Vector2.SignedAngle(ant.forward, diff);
            sum += angle * diff.magnitude;
            active++;
        }
        if (active == 0) return 0;
        return sum / active;
    }
    
    public override void DrawStaticGizmos() {
        if (attractors == null) return;
        Gizmos.color = Color.red;
        foreach (Attractor attractor in attractors) {
            Gizmos.DrawWireSphere(attractor.origin, attractor.minDistance);
        }
    }

    [Serializable]
    public struct Attractor
    {
        public Vector2 origin;
        public float minDistance;
        public float minDistanceSqr => minDistance * minDistance;
    }
}
