using UnityEngine;

[CreateAssetMenu(fileName = "DepositFoodBehaviour", menuName = "Behaviour/Parts/Deposit Food")]
public class DepositFoodBehaviour : PartBehaviour
{
    public override Vector2 GetVelocity(Ant ant, World world) {
        if (Vector3.SqrMagnitude(world.hill.transform.position - ant.transform.position) < world.hill.radius) {
            ant.DepositFood();
        }       
        return Vector2.zero;
    }
}
