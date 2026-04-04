using System.Linq;
using UnityEngine;

public abstract class AntBehaviour : ScriptableObject
{
    public abstract Vector2 GetVelocity(Ant ant, World world);
    public virtual void DrawInstanceGizmos(Ant ant, World world) { }
    public virtual void DrawStaticGizmos() { }

    protected Vector2 GetWeightedSum(params Vector2Weight[] vectors) {
        float totalWeight = vectors.Select(v => v.weight).Sum();
        
        Vector2 sum = Vector2.zero;
        foreach (Vector2Weight vw in vectors) {
            sum += vw.v2 * (vw.weight / totalWeight);
        }

        return sum;
    }

    protected Vector2 GetWeightedSum(params Vector2[] vectors) {
        return GetWeightedSum(vectors.Select(v => new Vector2Weight(v, 1)).ToArray());
    }

    protected struct Vector2Weight
    {
        public Vector2 v2;
        public float weight;
        
        public Vector2Weight(Vector2 v2, float weight) {
            this.v2 = v2;
            this.weight = weight;
        }
    }
}
