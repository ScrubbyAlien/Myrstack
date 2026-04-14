using UnityEngine;

public abstract class PartBehaviour : ScriptableObject
{
    public abstract Vector2 GetVelocity(Ant ant, World world);
    public virtual void DrawInstanceGizmos(Ant ant, World world) { }
    public virtual void DrawStaticGizmos() { }
}
