using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "State/World")]
public class World : ScriptableObject
{
    private List<Ant> ants;
    public Ant[] allAnts { get; private set; }

    private void OnEnable()
    {
        ants = new();
        allAnts = Array.Empty<Ant>();
    }

    public void RegisterAnt(Ant ant)
    {
        ants.Add(ant);
        allAnts = ants.ToArray();
    }

}
