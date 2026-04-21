using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PheromoneManager : MonoBehaviour
{
    [SerializeField]
    private World world;
    
    private Dictionary<Pheromone, PheromoneGrid> pheromoneGrids;
    
    [SerializeField]
    private float dissipation;
    
    private void Start() {
        world.RegisterPheromoneManager(this);

        pheromoneGrids = new();
        foreach (Pheromone pheromone in Enum.GetValues(typeof(Pheromone))) {
            pheromoneGrids.Add(pheromone, new PheromoneGrid(dissipation));
        }
    }

    private void Update() {
        foreach (PheromoneGrid grid in pheromoneGrids.Values) {
            grid.Update(Time.deltaTime);
        }
    }

    private void OnDrawGizmos() {
        if (pheromoneGrids == null) return;
        foreach (PheromoneGrid grid in pheromoneGrids.Values) {
            foreach ((Vector2 location, float concentration) in grid.worldConcentrations) {
                Gizmos.DrawWireSphere((Vector3)location, concentration * 0.1f);
            }
        }
    }

    public void Excrete(Pheromone pheromone, Vector2 location, float amount) {
        pheromoneGrids[pheromone].Excrete(location, amount);
    }

    public float Sample(Pheromone pheromone, Vector2 location, int size) {
        return pheromoneGrids[pheromone].Sample(location, size);
    }
}

public enum Pheromone
{
    None, Leaving, Returning
}

public class PheromoneGrid
{
    private Dictionary<Vector2Int, float> pheromoneConcentrations;
    private float dissipation;
    
    public PheromoneGrid(float dissipation) {
        pheromoneConcentrations = new();
        this.dissipation = dissipation;
    }

    public float Sample(Vector2 continuousLocation, int size) {
        Vector2Int origin = GridConfiguration.ToGridPosition(continuousLocation);
        
        float sum = 0;
        foreach (float concentration in GridConfiguration.Search(pheromoneConcentrations, origin, size)) {
            sum += concentration;
        }
        
        // average concentration of sample area
        return sum / (size * size);
    }
    
    public void Excrete(Vector2 continuousLocation, float amount) {
        Vector2Int location = GridConfiguration.ToGridPosition(continuousLocation);
        
        if (pheromoneConcentrations.TryGetValue(location, out float concentration)) {
            pheromoneConcentrations[location] += amount;
        }
        else {
            pheromoneConcentrations.Add(location, amount);
        }
    }

    public void Update(float timeStep) {
        Vector2Int[] keys = pheromoneConcentrations.Keys.ToArray();

        foreach (Vector2Int location in keys) {
            pheromoneConcentrations[location] *= Mathf.Pow(dissipation, timeStep);
            if (pheromoneConcentrations[location] <= 0) pheromoneConcentrations.Remove(location);
        }
        
    }

    public IEnumerable<(Vector2, float)> worldConcentrations {
        get { return pheromoneConcentrations.Select(p => (GridConfiguration.ToWorldPosition(p.Key), p.Value)); }
    }
}
