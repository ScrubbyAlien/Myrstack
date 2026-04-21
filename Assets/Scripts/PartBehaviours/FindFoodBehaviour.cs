using UnityEngine;

[CreateAssetMenu(fileName = "FindFoodBehaviour", menuName = "Behaviour/Parts/Find Food")]
public class FindFoodBehaviour : PartBehaviour
{
    [SerializeField]
    private int searchSize;
    
    public override Vector2 GetVelocity(Ant ant, World world) {
        if (world.resourceManager.FindFoodCluster(ant.position, searchSize, out Vector2Int clusterPosition)) {
            if (clusterPosition == GridConfiguration.ToGridPosition(ant.position)) {
                Food food = world.resourceManager.PickFoodFromCluster(clusterPosition);
                if (food) ant.PickUpFood(food);
                return Vector2.zero;
            }
            else {
                Vector2 clusterWorldPosition = GridConfiguration.ToWorldPosition(clusterPosition);
                Vector2 toCluster = clusterWorldPosition - ant.position;
                return toCluster.normalized;
            }
        }
        else return Vector2.zero;
    }

    public override bool Include(Ant ant, World world) {
        if (world.resourceManager.FindFoodCluster(ant.position, searchSize, out Vector2Int _)) return true;
        return false;
    }
}
