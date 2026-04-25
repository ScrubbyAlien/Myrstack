using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "World", menuName = "State/World")]
public class World : ScriptableObject
{
    public event Action AntsChanged;
    public event Action<Hill> HillRegistered;
    
    private List<Ant> ants;
    public Ant[] allAnts { get; private set; }
    public Ant[] allNonEnemyAnts { get; private set; }
    public (Ant ant, Vector2Int coord)[] allAntGridCoords { get; private set; }
    
    public PheromoneManager pheromoneManager { get; private set; }
    public ResourceManager resourceManager { get; private set; }
    public Hill hill { get; private set; }
    public Camera mainCamera { get; private set; }
    
    public void OnEnable() {
        Reset();
        SceneManager.activeSceneChanged += ResetOnSceneLoad;
    }

    public void OnDisable() {
        SceneManager.activeSceneChanged -= ResetOnSceneLoad;
    }
    
    private void Reset() {
        Debug.Log("reset");
        
        mainCamera = Camera.main;
        
        ants = new();
        hill = null;
        pheromoneManager = null;
        resourceManager = null;
        allAntGridCoords = Array.Empty<(Ant, Vector2Int)>();
        allNonEnemyAnts = Array.Empty<Ant>();
        allAnts = Array.Empty<Ant>();
        AntsChanged = null;
        HillRegistered = null;
    }
    
    private void ResetOnSceneLoad(Scene _s1, Scene _s2) {
        Reset();
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
    public void RegisterHill(Hill hill) {
        this.hill = hill;
        HillRegistered?.Invoke(hill);
    }
    public void RegisterPheromoneManager(PheromoneManager manager) => pheromoneManager = manager;
    public void RegisterResourceManager(ResourceManager manager) => resourceManager = manager;

    private void UpdateAntCollections() {
        allAnts = ants.ToArray();
        allNonEnemyAnts = ants.Where(ant => ant.currentMode != BehaviourMode.Attacking).ToArray();
        allAntGridCoords = ants.Select(ant => {
            Vector2Int coord = GridConfiguration.ToGridPosition(ant.position);
            return (ant, coord);
        }).ToArray();
        AntsChanged?.Invoke();
    }
    
}

