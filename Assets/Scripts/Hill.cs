using System;
using UnityEngine;

public class Hill : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private Ant antPrefab;
    [SerializeField, Min(0)]
    private float capacity;
    [SerializeField, Min(0)]
    private float rate;
    private float secondsPerAnt => 1 / rate;
    private float nextSpawnTime;

    [SerializeField]
    private float areaPerCapacity;
    private const float oneOverSqrtPI = 0.564189584f;
    private float targetRadius => Mathf.Sqrt(capacity * areaPerCapacity) * oneOverSqrtPI;
    
    private void UpdateScale() {
        transform.localScale = Vector3.one * targetRadius * 2;
    }
    
    private void OnValidate() {
        UpdateScale();
    }

    private void Update() {
        // if the hill is at capacity then don't spawn any more
        if (world.allAnts.Length + 1 > capacity) return;    
        
        if (Time.time >= nextSpawnTime) {
            Spawn();
            nextSpawnTime = Time.time + secondsPerAnt;
        }
    }

    private void Spawn() {
        Instantiate(antPrefab, transform.position, Quaternion.identity);
    }
    
    

    public void AddCapacity(float additional) {
        capacity += additional;
        UpdateScale();
    }
}
