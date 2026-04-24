using UnityEngine;

[CreateAssetMenu(fileName = "StayOnScreen", menuName = "Behaviour/Parts/Stay On Screen")]
public class StayOnScreenBehaviour : PartBehaviour
{
    public override Vector2 GetVelocity(Ant ant, World world) {
        (Vector2 min, Vector2 max) = GetCameraBounds(world.mainCamera);

        Vector2 clampedPosition = ClampPositionToBounds(min, max, ant.position);
        if (ant.position == clampedPosition) return Vector2.zero;

        Vector2 turnAroundVelocity = clampedPosition - ant.position;
        return turnAroundVelocity;
    }

    public override bool Include(Ant ant, World world) {
        (Vector2 min, Vector2 max) = GetCameraBounds(world.mainCamera);
        return ClampPositionToBounds(min, max, ant.position) != ant.position;
    }

    private (Vector2, Vector2) GetCameraBounds(Camera camera) {
        float ySize = camera.orthographicSize;
        float xSize = ySize * camera.aspect;
        Vector2 min = new Vector2(-xSize, -ySize);
        Vector2 max = new Vector2(xSize, ySize);
        return (min, max);
    }

    private Vector2 ClampPositionToBounds(Vector2 min, Vector2 max, Vector2 position) {
        float clampedX = Mathf.Clamp(position.x, min.x, max.x);
        float clampedY = Mathf.Clamp(position.y, min.y, max.y);
        return new Vector2(clampedX, clampedY);
    }

}
