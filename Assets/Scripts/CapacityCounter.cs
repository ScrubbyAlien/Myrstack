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
        world.AntsChanged += OnAntsChanged;
        if (world.hill) {
            world.hill.CapacityChanged += OnCapacityChanged;
            currentCapacity = world.hill.hillCapacity;
            UpdateTextField(0f, world.hill.hillCapacity);
        }
        else world.HillRegistered += (hill) => {
            hill.CapacityChanged += OnCapacityChanged;
            currentCapacity = hill.hillCapacity;
            UpdateTextField(0f, hill.hillCapacity);
        };
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
