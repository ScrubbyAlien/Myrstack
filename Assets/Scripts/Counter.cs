using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private Hill hill;
    [SerializeField]
    private TMP_Text textField;
    
    void Awake() {
        UpdateTextField(0f);
        hill.FoodCollected += UpdateTextField;
    }
    
    private void UpdateTextField(float amount) {
        textField.text = $"{amount:0}";
        float numberOfDigits = textField.text.Length;
    }
}
