using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flavorText;
    [SerializeField] private FlavorTextData textData;
    [SerializeField] private Slider slider;

    private float currentSliderValue, count;

    private List<string> availableData = new List<string>();

    private void OnEnable()
    {
        NewMapGenerator.OnRoomGenerated += UpdateSlider;
    }

    private void OnDisable()
    {
        NewMapGenerator.OnRoomGenerated -= UpdateSlider;
    }

    private void Start() 
    {
        Initialize();
    }

    public void Initialize()
    {
        slider.value = 0;
        currentSliderValue = 0;
        count = 0;
        availableData = new List<string>();
        foreach(string text in textData.text)
        {
            availableData.Add(text);
        }    
        ChangeFlavorText();
    }
    private void UpdateSlider()
    {
        currentSliderValue++;
        count++;
        slider.value = currentSliderValue;
        if(count > 12) 
        {
            ChangeFlavorText();
            count = 0;
        }
    }

    private void ChangeFlavorText()
    {
        int randText = Random.Range(0, availableData.Count-1);
        flavorText.text = availableData[randText];
        if(availableData.Count-1 != 0) availableData.Remove(availableData[randText]);
    }


}
