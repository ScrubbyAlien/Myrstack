using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFollowPheromoneBehaviour", menuName = "Behaviour/Parts/Follow Pheromone")]
public class FollowPheromoneBehaviour : PartBehaviour
{
    [SerializeField]
    private Pheromone pheromone;

    public override Vector2 GetVelocity(Ant ant, World world) {

        float highestConcentration = 0f;
        Vector2 highestConcentrationDirection = Vector2.zero;
        
        for (int i = 0; i < ant.sensors.Length; i++) {
            (Vector2 sensorPosition, int size) = ant.sensors[i];
            float concentration = world.pheromoneManager.Sample(pheromone, sensorPosition, size);
            if (highestConcentration < concentration) {
                highestConcentration = concentration;
                highestConcentrationDirection = (sensorPosition - ant.position).normalized;
            }
        }
        
        return highestConcentrationDirection * highestConcentration;
    }
}
