using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ant Behaviour", menuName = "Behaviour/Ant Behaviour")]
public class AntBehaviour : ScriptableObject
{
    [SerializeField]
    protected WeightedPart[] parts;
    
    public float GetWeightedSum(Ant ant, World world) {
        return GetWeightedSum(ant, world, parts);
    }

    protected float GetWeightedSum(Ant ant, World world, params WeightedPart[] weightedParts) {
        IEnumerable<WeightedPart> enabled = weightedParts.Where(v => v.enabled);
        float totalWeight = enabled.Select(v => v.weight).Sum();
        
        float sum = 0;
        foreach (WeightedPart weightedPart in enabled) {
            sum += weightedPart.part.GetAngularVelocity(ant, world) * (weightedPart.weight / totalWeight);
        }

        return sum;
    }
    
    [Serializable]
    protected struct WeightedPart
    {
        public string name {
            get {
                if (part) return part.name;
                else return "Undefined part behaviour";
            }
        }
        public float weight;
        public PartBehaviour part;
        public bool hideGizmos;
        public bool enabled;
        public bool disabled => !enabled;
    }
}
