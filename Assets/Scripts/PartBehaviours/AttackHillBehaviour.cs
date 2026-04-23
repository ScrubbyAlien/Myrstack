using UnityEngine;

[CreateAssetMenu(fileName = "AttackHill", menuName = "Behaviour/Parts/Attack Hill")]
public class AttackHillBehaviour : PartBehaviour
{
    [SerializeField]
    private bool limitStrength;
    [SerializeField]
    private float maxStrength;
    
    public override Vector2 GetVelocity(Ant ant, World world) {
        Vector2 toTarget = ant.holdingFood switch {
            true => ant.spawnPosition - ant.position,
            false => (Vector2)world.hill.transform.position - ant.position,
        };
        
        return limitStrength ? Vector2.ClampMagnitude(toTarget, maxStrength) : toTarget;
    }
}
