using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hill : MonoBehaviour
{
    public event Action<float> FoodCollected;
    public event Action<float> CapacityChanged;

    [SerializeField]
    private ResourceManager resourceManager;
    [SerializeField]
    private World world;
    [SerializeField]
    private Ant antPrefab;
    [SerializeField, Min(0)]
    private float capacity;
    public float hillCapacity => capacity;
    [SerializeField]
    private float addCapacity;
    private bool atCapacity => world.allNonEnemyAnts.Length >= capacity;

    [SerializeField]
    private float initialFood;
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

    private void Awake() {
        world.RegisterHill(this);
    }
    
    private void Start() {
        antsParent = new GameObject("AntsParent");
        collectedFood = initialFood;
        FoodCollected?.Invoke(collectedFood);
        CapacityChanged?.Invoke(capacity);
    }

    private void Spawn(out Ant ant) {
        if (atCapacity) {
            ant = null;
            return;
        }    
        ant = Instantiate(antPrefab, transform.position, Quaternion.identity);
        ant.transform.parent = antsParent.transform;
    }

    public void AddCapacity() {
        capacity += addCapacity;
        UpdateScale();
        CapacityChanged?.Invoke(capacity);
    }

    public void PayAndAddCapacity(float cost) {
        if (PayFood(cost)) {
            AddCapacity();
        }
    } 
    
    public void PayAndSpawnForager(float cost) {
        PayAndSpawnAnt(cost, BehaviourMode.Exploring);
    }
    
    public void PayAndSpawnDefender(float cost) {
        PayAndSpawnAnt(cost, BehaviourMode.Defending);
    }

    private void PayAndSpawnAnt(float cost, BehaviourMode mode) {
        if (!atCapacity && PayFood(cost)) {
            Spawn(out Ant ant);
            ant.SetBehaviour(mode);
        }
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

    private bool PayFood(float cost) {
        if (cost <= collectedFood) {
            collectedFood -= cost;
            FoodCollected?.Invoke(collectedFood);
            return true;
        }
        return false;
    }
    
}
