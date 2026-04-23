using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private Hill hill;
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private float extraWidthPerDigit;
    private float baseWidth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        baseWidth = background.sizeDelta.x - extraWidthPerDigit;
        
        UpdateTextField(0f);
        hill.FoodCollected += UpdateTextField;
    }


    private void UpdateTextField(float amount) {
        textField.text = $"{amount:0}";
        float numberOfDigits = textField.text.Length;
        float newWidth = baseWidth + extraWidthPerDigit * numberOfDigits;
        Vector2 adjustedDelta = background.sizeDelta;
        adjustedDelta.x = newWidth;
        background.sizeDelta = adjustedDelta;
    }
}
