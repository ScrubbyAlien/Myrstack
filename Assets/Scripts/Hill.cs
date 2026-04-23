using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public event Action<float> FoodCollected;

    [SerializeField]
    private ResourceManager resourceManager;
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
    
    public void SetNoneAntBehaviours(string mode, int number) {
        if (Enum.TryParse<BehaviourMode>(mode, out BehaviourMode parsed)) {
            Ant[] uninitialisedAnts = world.allAnts.Where(a => !a.initialised).ToArray();
            number = Mathf.Min(uninitialisedAnts.Length, number);
            for (int i = 0; i < number; i++) {
                uninitialisedAnts[i].SetBehaviour(parsed);
            }
        }
        else {
            Debug.LogError($"Behaviour mode {mode} is not a valid enumeration of BehaviourMode", this);
        }
    }

    public void SetOneNoneAntBehaviour(string mode) {
        SetNoneAntBehaviours(mode, 1);
    }

    public void CollectFood(float amount) {
        collectedFood += amount;
        FoodCollected?.Invoke(collectedFood);
    }

    public bool LoseFood(out Food food) {
        if (collectedFood >= 1f) {
            food = resourceManager.InstantiateFood(transform.position);
            collectedFood -= 1;
            FoodCollected?.Invoke(collectedFood);
            return true;
        }
        food = null;
        return false;
    }
    
}
