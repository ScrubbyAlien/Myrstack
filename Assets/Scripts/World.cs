using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "State/World")]
public class World : ScriptableObject
{
    private List<Ant> ants;
    public Ant[] allAnts { get; private set; }

    private Dictionary<Pheromone, float[,]> pheromoneGrids;
    [SerializeField]
    private int cellSize, gridSize;
    
    private void OnEnable()
    {
        ants = new();
        allAnts = Array.Empty<Ant>();
        pheromoneGrids = new();

        foreach (Pheromone pheromone in Enum.GetValues(typeof(Pheromone))) {
            pheromoneGrids.Add(pheromone, new float[gridSize,gridSize]);
        }
    }

    public void RegisterAnt(Ant ant)
    {
        ants.Add(ant);
        allAnts = ants.ToArray();
    }

}

public enum Pheromone
{
    Leaving, Returning
}
