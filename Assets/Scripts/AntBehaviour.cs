using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ant Behaviour", menuName = "Behaviour/Ant Behaviour")]
public class AntBehaviour : ScriptableObject
{
    [SerializeField]
    protected WeightedPart[] parts;
    
    public Vector2 GetWeightedSum(Ant ant, World world) {
        return GetWeightedSum(ant, world, parts);
    }

    protected Vector2 GetWeightedSum(Ant ant, World world, params WeightedPart[] weightedParts) {
        float totalWeight = weightedParts.Select(v => v.weight).Sum();
        
        Vector2 sum = Vector2.zero;
        foreach (WeightedPart weightedPart in weightedParts) {
            sum += weightedPart.part.GetVelocity(ant, world) * (weightedPart.weight / totalWeight);
        }

        return sum;
    }

    public void DrawInstanceGizmos(Ant ant, World world) {
        foreach (WeightedPart part in parts) {
            if (part.hideGizmos) continue;
            part.part.DrawInstanceGizmos(ant, world);
        }
    }
    
    [Serializable]
    protected struct WeightedPart
    {
        public string name => part.name;
        public float weight;
        public PartBehaviour part;
        public bool hideGizmos;
    }
}
