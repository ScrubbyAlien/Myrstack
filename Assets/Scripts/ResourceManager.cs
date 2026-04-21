using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private World world;
    private Camera mCamera;
    private Vector2 size => new Vector2(mCamera.orthographicSize * mCamera.aspect, mCamera.orthographicSize);
    [SerializeField]
    private Food foodPrefab;
    [SerializeField]
    private float minDistanceFromCenter;
    [SerializeField]
    private float foodSpawnTimeMin, foodSpawnTimeMax;
    private float nextSpawnTime;
    
    private Dictionary<Vector2Int, FoodCluster> foodGrid;

    private void Start() {
        world.RegisterResourceManager(this);
        mCamera = Camera.main;
        foodGrid = new();
        nextSpawnTime = Time.time + Random.Range(foodSpawnTimeMin, foodSpawnTimeMax);
    }

    private void Update() {
        // if (Time.time > nextSpawnTime) {
        //     nextSpawnTime = Time.time + Random.Range(foodSpawnTimeMin, foodSpawnTimeMax);
        //     CreateFood();
        // }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, minDistanceFromCenter);
        if (foodGrid == null) return;
        foreach (Vector2Int gridPosition in foodGrid.Keys) {
            Gizmos.DrawWireSphere(GridConfiguration.ToWorldPosition(gridPosition), 0.1f);
        }
    }

    private bool GetRandomPosition(out Vector2 random) {
        random = Vector2.zero;
        for (int i = 0; i < 30; i++) {
            random = new Vector2(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y));
            if (random.sqrMagnitude > minDistanceFromCenter * minDistanceFromCenter) return true;
        }
        return false;
    }

    private FoodCluster CreateCluster(Vector2 origin, float radius, int amount) {
        FoodCluster cluster = new();
        for (int i = 0; i < amount; i++) {
            Vector3 position = origin + Random.insideUnitCircle * radius;
            Food food = Instantiate(foodPrefab, position, Quaternion.identity);
            food.transform.parent = transform;
            cluster.AddFood(food);
        }
        return cluster;
    }

    public void InstantiateFoodCluster() {
        GetRandomPosition(out Vector2 position);
        Vector2Int gridPosition = GridConfiguration.ToGridPosition(position);
        FoodCluster newCluster = CreateCluster(position, 1, 300);
        foodGrid.Add(gridPosition, newCluster);
    }
    
    public bool FindFoodCluster(Vector2 origin, int size, out Vector2Int closestPosition) {
        Vector2Int originInGrid = GridConfiguration.ToGridPosition(origin);
        closestPosition = new Vector2Int(size + 1, size + 1);
        int closestDistance = int.MaxValue;
        
        foreach ((Vector2Int coord, FoodCluster _) in GridConfiguration.SearchWithCoord(foodGrid, originInGrid, size)) {
            int distance = coord.Distance(originInGrid);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestPosition = coord;
            }
        }

        if (closestPosition.x == size + 1) return false;
        else return true;
    }

    public Food PickFoodFromCluster(Vector2Int clusterCoord) {
        if (foodGrid.TryGetValue(clusterCoord, out FoodCluster cluster)) {
            if (cluster.GetFood(out Food food)) {
                if (cluster.empty) foodGrid.Remove(clusterCoord);
                return food;
            }
        }
        return null;
    }

    private class FoodCluster
    {
        private Stack<Food> foodStack;
        public bool empty => foodStack.Count == 0;
        
        public FoodCluster() {
            foodStack = new();
        }

        public void AddFood(Food food) {
            foodStack.Push(food);
        }

        public bool GetFood(out Food food) {
            food = null;
            if (foodStack.Count == 0) return false;
            else food = foodStack.Pop();
            return true;
        }
    }
}
