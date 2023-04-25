using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // using text mesh for the clock display
using UnityEngine.UI;
using UnityEngine.Rendering; // used to access the volume component
using UnityEngine.Rendering.Universal;

public class Lighting : MonoBehaviour
{
    [SerializeField]private Light2D light;
    [SerializeField] private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        float currentTime = Time.time; 
        light.intensity = 1;
        light.pointLightOuterRadius = 500;
    }

    private void Update()
    {
        float currentTime = Time.time;
        bool isNight = false;

        // check if the current time is between 6:00 PM and 6:00 AM
        if (currentTime >= 18 * 3600 || currentTime < 6 * 3600)
        {
            isNight = true;
        }

        if (toggle.isOn)
        {
            isNight = true;
        }
        else
        {
            isNight = false;
        }

        if (isNight)
        {
            light.pointLightOuterRadius = 10;
        }
        else
        {
            light.pointLightOuterRadius = 500;
        }
    }
}
