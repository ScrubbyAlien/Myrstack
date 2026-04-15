using UnityEngine;

public abstract class PartBehaviour : ScriptableObject
{
    public abstract float GetAngularVelocity(Ant ant, World world);
    public virtual void DrawStaticGizmos() { }
}
