using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "State/World")]
public class World : ScriptableObject
{
    private List<Ant> ants;
    public Ant[] allAnts { get; private set; }
    public PheromoneManager pheromoneManager { get; private set; }
    public ResourceManager resourceManager { get; private set; }
    public Hill hill { get; private set; }
    
    private void OnEnable()
    {
        ants = new();
        hill = null;
        pheromoneManager = null;
        resourceManager = null;
        allAnts = Array.Empty<Ant>();
    }

    public void RegisterAnt(Ant ant)
    {
        ants.Add(ant);
        allAnts = ants.ToArray();
    }
    public void DeregisterAnt(Ant ant) {
        ants.Remove(ant);
        allAnts = ants.ToArray();
    }
    public void RegisterHill(Hill hill) => this.hill = hill;
    public void RegisterPheromoneManager(PheromoneManager manager) => pheromoneManager = manager;
    public void RegisterResourceManager(ResourceManager manager) => resourceManager = manager;

}

