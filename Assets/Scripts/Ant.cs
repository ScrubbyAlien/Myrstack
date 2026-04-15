using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ant : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private AntBehaviour behaviour;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float maxAngularVelocity;
    private object behaviourData;

    public float angularVelocity { get; private set; }
    private float angle;
    public Vector2 velocity { get; private set; }
    public Vector2 position => (Vector2)transform.position;
    public Vector2 forward => (Vector2)transform.up;
    public Vector2 right => (Vector2)transform.right;
    
    private void Start()
    {
        world.RegisterAnt(this);
        angle = Random.Range(-180f, 180f);
    }
    
    private void FixedUpdate()
    {
        angularVelocity += behaviour.GetWeightedSum(this, world);
        angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        angle += angularVelocity * Time.fixedDeltaTime;
    }

    private void Update() {
        float radians = angle * Mathf.Deg2Rad;
        Vector2 directionVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        velocity = directionVector * speed;
        
        transform.up = directionVector;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        // draw velocity
    }
    
    public void StoreBehaviourData<T>(T data) {
        behaviourData = data;
    }
    
    public T GetBehaviourData<T>() where T : new() {
        return behaviourData is T casted ? casted : new T();
    }
}
