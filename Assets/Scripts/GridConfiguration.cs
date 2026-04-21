using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GridConfiguration 
{
    public static float cellSize = 0.2f;
    
    public static Vector2Int ToGridPosition(Vector2 worldPosition) {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.y / cellSize));
    }
    
    public static Vector2 ToWorldPosition(Vector2Int gridPosition) {
        return new Vector2(gridPosition.x * cellSize, gridPosition.y * cellSize);
    }

    public static IEnumerable<T> Search<T>(Dictionary<Vector2Int, T> grid, Vector2Int origin, int size) {
        foreach ((Vector2Int, T) tuple in SearchWithCoord(grid, origin, size)) {
            yield return tuple.Item2;
        }
    }
    
    public static IEnumerable<(Vector2Int, T)> SearchWithCoord<T>(Dictionary<Vector2Int, T> grid, Vector2Int origin, int size) {
        Vector2Int min = origin - new Vector2Int(size, size);
        Vector2Int max = origin + new Vector2Int(size, size);
        
        for (int x = min.x; x < max.x; x++) {
            for (int y = min.y; y < max.y; y++) {
                Vector2Int coord = new Vector2Int(x, y);
                if (grid.TryGetValue(coord, out T value)) yield return (coord, value);
            }
        }
    }

    public static int Distance(this Vector2Int from, Vector2Int to) {
        Vector2Int diff = from - to;
        return Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.y));
    }
}
