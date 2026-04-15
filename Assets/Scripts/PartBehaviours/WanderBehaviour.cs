using UnityEngine;

[CreateAssetMenu(fileName = "NewWanderBehaviour", menuName = "Behaviour/Parts/Wander")]
public class WanderBehaviour : PartBehaviour
{
    [SerializeField]
    private float minAngularVelocity, maxAngularVelocity;

    public override float GetAngularVelocity(Ant ant, World world) {
        return Random.Range(minAngularVelocity, maxAngularVelocity);
    }
}
