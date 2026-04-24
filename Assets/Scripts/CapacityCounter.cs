using TMPro;
using UnityEngine;

public class CapacityCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private World world;

    private float currentCapacity;
    private float currentAnts;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        world.hill.CapacityChanged += OnCapacityChanged;
        world.AntsChanged += OnAntsChanged;
        UpdateTextField(0f, world.hill.hillCapacity);
    }
    
    private void UpdateTextField(float current, float capacity) {
        textField.text = $"{current:0}/{capacity:0}";
    }

    private void OnCapacityChanged(float newCapacity) {
        currentCapacity = newCapacity;
        UpdateTextField(currentAnts, currentCapacity);
    }

    private void OnAntsChanged() {
        currentAnts = world.allNonEnemyAnts.Length;
        UpdateTextField(currentAnts, currentCapacity);
    }

}
