using System;
using Sirenix.Utilities;
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

    private float collectedFood;
    private GameObject antsParent;

    [SerializeField]
    private float areaPerCapacity;
    public float radius => Mathf.Sqrt(capacity * areaPerCapacity / Mathf.PI);
    
    private void UpdateScale() {
        transform.localScale = Vector3.one * radius * 2;
    }
    
    private void OnValidate() {
        UpdateScale();
    }

    private void Start() {
        world.RegisterHill(this);
        antsParent = new GameObject("AntsParent");
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
        Ant ant = Instantiate(antPrefab, transform.position, Quaternion.identity);
        ant.transform.parent = antsParent.transform;
    }

    public void AddCapacity(float additional) {
        capacity += additional;
        UpdateScale();
    }

    public void SetAllAntBehaviours(string mode) {
        if (Enum.TryParse<BehaviourMode>(mode, out BehaviourMode parsed)) {
            foreach (Ant ant in world.allAnts) ant.SetBehaviour(parsed);
        }
        else {
            Debug.LogError($"Behaviour mode {mode} is not a valid enumeration of BehaviourMode", this);
        }
    }

    public void CollectFood(float amount) {
        collectedFood += amount;
        Debug.Log(collectedFood);
    }
    
}
