using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "State/World")]
public class World : ScriptableObject
{
    private List<Ant> ants;
    public Ant[] allAnts { get; private set; }
    public Ant[] allNonEnemyAnts { get; private set; }
    public (Ant ant, Vector2Int coord)[] allAntGridCoords { get; private set; } 
    
    public PheromoneManager pheromoneManager { get; private set; }
    public ResourceManager resourceManager { get; private set; }
    public Hill hill { get; private set; }
    
    private void OnEnable()
    {
        ants = new();
        hill = null;
        pheromoneManager = null;
        resourceManager = null;
        allAntGridCoords = Array.Empty<(Ant, Vector2Int)>();
        allNonEnemyAnts = Array.Empty<Ant>();
        allAnts = Array.Empty<Ant>();
    }

    public void RegisterAnt(Ant ant)
    {
        ants.Add(ant);
        UpdateAntCollections();
    }
    public void DeregisterAnt(Ant ant) {
        ants.Remove(ant);
        UpdateAntCollections();
    }
    public void RegisterHill(Hill hill) => this.hill = hill;
    public void RegisterPheromoneManager(PheromoneManager manager) => pheromoneManager = manager;
    public void RegisterResourceManager(ResourceManager manager) => resourceManager = manager;

    private void UpdateAntCollections() {
        allAnts = ants.ToArray();
        allNonEnemyAnts = ants.Where(ant => ant.currentMode != BehaviourMode.Attacking).ToArray();
        allAntGridCoords = ants.Select(ant => {
            Vector2Int coord = GridConfiguration.ToGridPosition(ant.position);
            return (ant, coord);
        }).ToArray();
    }
}

