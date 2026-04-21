using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ant : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private AntBehaviour noneBehaviour, exploringBehaviour, returningBehaviour, defendingBehaviour;
    private AntBehaviour currentBehaviour;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float minSpeed;

    [SerializeField]
    private int sampleSize;
    [SerializeField]
    private Vector2[] samplePoints;
    
    [SerializeField]
    private float amount, rate;
    private float secondsBetweenExcretion => 1f / rate;
    private float nextExcretionTime = 0;

    [SerializeField]
    private Vector2 holdPosition;
    private Food heldFood;
    
    
    public Vector2 velocity { get; private set; }
    public Vector2 position => (Vector2)transform.position;
    public Vector2 forward => (Vector2)transform.up;
    public Vector2 right => (Vector2)transform.right;
    public float angle => Vector2.SignedAngle(velocity, Vector2.up); 

    public (Vector2, int)[] sensors {
        get {
            return samplePoints.Select(p => {
                Vector2 rotated = Quaternion.AngleAxis(-angle, Vector3.forward) * p;
                return (rotated, sampleSize);
            }).ToArray();
        }
    }

    private void Start()
    {
        world.RegisterAnt(this);
        velocity = Random.insideUnitCircle;
        SetBehaviour(BehaviourMode.None);
    }
    
    private void FixedUpdate()
    {
        velocity += currentBehaviour.GetWeightedSum(this, world);
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        if (velocity.magnitude < minSpeed) velocity = velocity.normalized * minSpeed;
    }

    private void Update() {
        UpdatePosition(velocity);

        if (currentBehaviour.pheromone == Pheromone.None) return;
        if (Time.time >= nextExcretionTime) {
            nextExcretionTime = Time.time + secondsBetweenExcretion;
            world.pheromoneManager.Excrete(currentBehaviour.pheromone, position, amount);
        }
    }

    private void OnDrawGizmos()
    {
        if (currentBehaviour) currentBehaviour.DrawInstanceGizmos(this, world);
        foreach ((Vector2 position, int size) in sensors) {
            Gizmos.DrawWireCube(
                transform.position + (Vector3)position, 
                Vector3.one * size * GridConfiguration.cellSize
            );
        }
        Gizmos.color = Color.green;
        Vector2 rotatedHoldPosition = Quaternion.AngleAxis(-angle, Vector3.forward) * holdPosition;
        Gizmos.DrawWireSphere(transform.position + (Vector3)rotatedHoldPosition, 0.02f);
    }

    private void UpdatePosition(Vector2 velocity) {
        transform.localEulerAngles = new Vector3(0, 0, -angle);
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void SetBehaviour(BehaviourMode mode) {
        
        currentBehaviour = mode switch {
            BehaviourMode.None => noneBehaviour,
            BehaviourMode.Exploring => exploringBehaviour ?? noneBehaviour,
            BehaviourMode.Returning => returningBehaviour ?? noneBehaviour,
            BehaviourMode.Defending => defendingBehaviour ?? noneBehaviour,
            _ => noneBehaviour,
        };
    }

    public void PickUpFood(Food food) { 
        Debug.Log("pickup");
        food.transform.parent = transform;
        food.transform.localPosition = holdPosition;
        heldFood = food;
        if (currentBehaviour == exploringBehaviour) currentBehaviour = returningBehaviour;
    }

    public void DepositFood() {
        if (heldFood) Destroy(heldFood.gameObject);
        heldFood = null;
        world.hill.CollectFood(1f);
        if (currentBehaviour == returningBehaviour) currentBehaviour = exploringBehaviour;
    }
}

public enum BehaviourMode
{
    None, Exploring, Returning, Defending
}