using System;
using UnityEngine;

public class Ant : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private AntBehaviour behaviour;
    private object behaviourData;
    
    public Vector2 velocity { get; private set; }
    public Vector2 position => (Vector2)transform.position;
    public Vector2 forward => (Vector2)transform.up;
    
    private void Start()
    {
        world.RegisterAnt(this);
    }
    
    private void FixedUpdate()
    {
        velocity = behaviour.GetVelocity(this, world);
    }

    private void Update() {
        UpdatePosition(velocity);
    }

    private void OnDrawGizmos()
    {
        if (behaviour) behaviour.DrawInstanceGizmos(this, world);
    }

    private void UpdatePosition(Vector2 velocity) {
        
        float angle = Vector2.SignedAngle(velocity, Vector2.up);
        transform.localEulerAngles = new Vector3(0, 0, -angle);
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    public void StoreBehaviourData<T>(T data) {
        behaviourData = data;
    }
    
    public T GetBehaviourData<T>() where T : new() {
        return behaviourData is T casted ? casted : new T();
    }
}
