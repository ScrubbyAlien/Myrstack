using System;
using System.Collections.Generic;
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
        IEnumerable<WeightedPart> enabled = weightedParts.Where(v => !v.disabled);
        float totalWeight = enabled.Select(v => v.weight).Sum();
        
        Vector2 sum = Vector2.zero;
        foreach (WeightedPart weightedPart in enabled) {
            sum += weightedPart.part.GetVelocity(ant, world) * (weightedPart.weight / totalWeight);
        }

        return sum;
    }

    public void DrawInstanceGizmos(Ant ant, World world) {
        foreach (WeightedPart part in parts) {
            if (part.hideGizmos || part.disabled) continue;
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
        public bool disabled;
    }
}
