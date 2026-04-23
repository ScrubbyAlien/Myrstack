using UnityEngine;

[CreateAssetMenu(fileName = "FindHillBehaviour", menuName = "Behaviour/Parts/Find Hill")]
public class FindHillBehaviour : PartBehaviour
{
    [SerializeField]
    private float searchSize;

    private float SqrDistanceToHill(Ant ant, World world) {
        return Vector2.SqrMagnitude((Vector2)world.hill.transform.position - ant.position);
    }
    
    public override Vector2 GetVelocity(Ant ant, World world) {
        if (SqrDistanceToHill(ant, world) < searchSize * searchSize) {
            return (Vector2)world.hill.transform.position - ant.position;
        }
        else return Vector2.zero;
    }

    public override bool Include(Ant ant, World world) {
        if (SqrDistanceToHill(ant, world) < searchSize * searchSize) return true;
        return false;
    }
}
