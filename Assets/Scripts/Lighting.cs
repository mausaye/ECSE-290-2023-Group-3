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
    private PlayerInfo playerInformation = PlayerInfo.Instance;
    private Quaternion targetRotation;
    public float rotationSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        float currentTime = Time.time; 
        light.intensity = 1;
        light.pointLightOuterRadius = 500;
    }

    private void Update()
    {
        Direction direction = playerInformation.getLastDirection();
        //Debug.Log(direction);

        switch (direction)
        {
            case Direction.IDLE_LEFT:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 90))
                    targetRotation = Quaternion.Euler(0f, 0f, 90);
                break;
            case Direction.MOVE_LEFT:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 90))
                    targetRotation = Quaternion.Euler(0f, 0f, 90);
                break;
            case Direction.IDLE_RIGHT:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, -90))
                    targetRotation = Quaternion.Euler(0f, 0f, -90);
                break;
            case Direction.MOVE_RIGHT:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, -90))
                    targetRotation = Quaternion.Euler(0f, 0f, -90);
                break;
            case Direction.IDLE_UP:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 0))
                    targetRotation = Quaternion.Euler(0f, 0f, 0);
                break;
            case Direction.MOVE_UP:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 0))
                    targetRotation = Quaternion.Euler(0f, 0f, 0);
                break;
            case Direction.IDLE_DOWN:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 180))
                    targetRotation = Quaternion.Euler(0f, 0f, 180);
                break;
            case Direction.MOVE_DOWN:
                if (light.transform.rotation != Quaternion.Euler(0f, 0f, 180))
                    targetRotation = Quaternion.Euler(0f, 0f, 180);
                break;
        }
        light.transform.rotation = Quaternion.Lerp(light.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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
            light.pointLightOuterAngle = 85;
            light.pointLightInnerAngle = 0;
        }
        else
        {
            light.pointLightOuterRadius = 500;
            light.pointLightOuterAngle = 360;
            light.pointLightInnerAngle = 360;
        }
    }
}
