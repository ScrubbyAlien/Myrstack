using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ant : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField, Tooltip("If the ant spends this much time away from the hill they will perish")]
    private float lifeTime;
    private float timer;
    [SerializeField]
    private AntBehaviour noneBehaviour, exploringBehaviour, returningBehaviour, defendingBehaviour, attackingBehaviour;
    public AntBehaviour currentBehaviour { get; private set; }
    public BehaviourMode currentMode { get; private set; }
    public bool initialised;
    
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
    public bool holdingFood => heldFood;
    
    public Vector2 spawnPosition { get; private set; }
    public Vector2 velocity { get; private set; }
    public Vector2 position => (Vector2)transform.position;
    public Vector2 forward => (Vector2)transform.up;
    public Vector2 right => (Vector2)transform.right;
    public float angle => Vector2.SignedAngle(velocity, Vector2.up);

    private bool turnAround;

    private float hitPoints = 1f;
    
    
    public (Vector2, int)[] sensors {
        get {
            return samplePoints.Select(p => {
                Vector2 rotated = Quaternion.AngleAxis(-angle, Vector3.forward) * p;
                return ((Vector2)transform.position + rotated, sampleSize);
            }).ToArray();
        }
    }

    private void Start() {
        spawnPosition = transform.position;
        world.RegisterAnt(this);
        velocity = Random.insideUnitCircle;
        if (!initialised) SetBehaviour(BehaviourMode.None);
    }
    
    private void FixedUpdate()
    {
        velocity += currentBehaviour.GetWeightedSum(this, world);
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        if (turnAround) {
            velocity = -velocity;
            turnAround = false;
        }
        if (velocity.magnitude < minSpeed) velocity = velocity.normalized * minSpeed;

        if (heldFood && currentMode != BehaviourMode.Attacking) {
            float sqrDistanceToHill = Vector3.SqrMagnitude(world.hill.transform.position - transform.position);
            if (sqrDistanceToHill < world.hill.radius * world.hill.radius) {
                DepositFood();
            }    
        }
    }

    private void Update() {
        UpdatePosition(velocity);
        if (!world.hill) return;
        
        if (Vector3.SqrMagnitude((Vector2)world.hill.transform.position - position) < world.hill.radius * world.hill.radius) {
            timer = 0;
            
            if (currentMode == BehaviourMode.Attacking && !holdingFood) {
                StealFood();
                if (holdingFood) TurnAround();
            }  
        }
        else if (currentMode != BehaviourMode.Attacking) {
            timer += Time.deltaTime;
            if (timer > lifeTime) {
                world.DeregisterAnt(this);
                Destroy(gameObject);
            }
        }

        if (currentMode == BehaviourMode.Attacking && !holdingFood) {
            if (WithinGridDistanceOfOtherAnt(1, out Ant ant, BehaviourMode.Exploring, BehaviourMode.Returning, BehaviourMode.Defending)) {
                if (ant.Kill(out Food food)) {
                    PickUpFood(food);
                    turnAround = false; // undo TurnAround() call in PickUpFood()
                }
            }
        }

        if (currentMode == BehaviourMode.Attacking && holdingFood
            && Vector3.SqrMagnitude(spawnPosition - position) < 4f) 
        {
            world.DeregisterAnt(this);
            Destroy(gameObject);
        }

        if (currentMode == BehaviourMode.Defending) {
            if (WithinGridDistanceOfOtherAnt(1, out Ant ant, BehaviourMode.Attacking)) {
                if (ant.Kill(out Food food)) {
                    food.Annihilate();
                }
            }
        }
        
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
                (Vector3)position, 
                Vector3.one * (size + 1) * GridConfiguration.cellSize
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
        if (mode == BehaviourMode.None) initialised = false;
        else initialised = true;

        currentMode = mode;
        
        currentBehaviour = mode switch {
            BehaviourMode.None => noneBehaviour,
            BehaviourMode.Exploring => exploringBehaviour ?? noneBehaviour,
            BehaviourMode.Returning => returningBehaviour ?? noneBehaviour,
            BehaviourMode.Defending => defendingBehaviour ?? noneBehaviour,
            BehaviourMode.Attacking => attackingBehaviour ?? noneBehaviour,
            _ => noneBehaviour,
        };

        if (mode == BehaviourMode.Defending) hitPoints = 3f;

        GetComponent<SpriteRenderer>().color = currentBehaviour.antColor;
    }

    public void PickUpFood(Food food) { 
        food.transform.parent = transform;
        food.transform.localPosition = holdPosition;
        heldFood = food;
        if (currentMode == BehaviourMode.Exploring) SetBehaviour(BehaviourMode.Returning);
        TurnAround();
    }

    public void DepositFood() {
        if (heldFood) Destroy(heldFood.gameObject);
        heldFood = null;
        world.hill.CollectFood(1f);
        if (currentMode == BehaviourMode.Returning) SetBehaviour(BehaviourMode.Exploring);
        TurnAround();
    }

    private void StealFood() {
        if (world.hill.LoseFood(out Food food)) {
            PickUpFood(food);
        }
    }

    private bool Kill(out Food food) {
        hitPoints -= 1f;
        if (hitPoints <= 0) {
            bool wasHoldingFood = false;
            food = null;
            if (heldFood) {
                heldFood.transform.parent = null;
                food = heldFood;
                heldFood = null;
                wasHoldingFood = true;
            }
            world.DeregisterAnt(this);
            Destroy(gameObject);
            return wasHoldingFood;
        }
        else {
            food = null;
            return false;
        }
    }

    private void TurnAround() {
        turnAround = true;
    }

    private bool WithinGridDistanceOfOtherAnt(int distance, out Ant ant, params BehaviourMode[] filter) {
        Vector2Int thisAntCoord = GridConfiguration.ToGridPosition(position);
        foreach ((Ant candidateAnt, Vector2Int coord) in world.allAntGridCoords) {
            if (candidateAnt == this) continue;
            if (!filter.Contains(candidateAnt.currentMode)) continue;
            if (GridConfiguration.Distance(thisAntCoord, coord) <= distance) {
                ant = candidateAnt;
                return true;
            }
        }
        ant = null;
        return false;
    }
}

[System.Flags]
public enum BehaviourMode
{
    None = 0,
    Exploring = 1 << 1, 
    Returning = 1 << 2, 
    Defending = 1 << 3, 
    Attacking = 1 << 4,
    All = Exploring | Returning | Defending | Attacking
}