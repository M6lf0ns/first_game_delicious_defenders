using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    private MenuMusic menuMusic;

    void Start()
    {
        slider = GetComponent<Slider>();
        menuMusic = MenuMusic.instance;

        if (menuMusic != null)
        {
            // Set slider value to initial volume
            slider.value = menuMusic.GetVolume();
        }

        // Add listener to respond to slider changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        // Adjust volume of MenuMusic based on slider value
        if (menuMusic != null)
        {
            menuMusic.SetVolume(value);
        }
    }
}
