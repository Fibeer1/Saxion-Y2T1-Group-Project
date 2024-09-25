using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FieldOfViewTrigger : MonoBehaviour
{
    public string type; //Can be Sensor, Camera, Drone or Vision
    public TextMeshPro sensorName;
    public List<GameObject> sensorCollisions = new List<GameObject>();
    [SerializeField] private Light fovLight;
    private Color originalColor;
    [SerializeField] private Color triggeredColor;

    private void Start()
    {
        if (type == "Sensor" || type == "Camera" || type == "Drone")
        {
            originalColor = new Color(1, 1, 1, 0.03f);
            SensorDropdown sensorDropdown = FindObjectOfType<SensorDropdown>();
            if (!sensorDropdown.sensors.Contains(this))
            {
                sensorDropdown.sensors.Add(this);
                sensorName.text = type + " " + sensorDropdown.sensors.Count;
            }
            sensorDropdown.UpdateSensorDropdown();
        }
    }

    private void Update()
    {
        if (sensorCollisions.Count != 0)
        {
            for (int i = 0; i < sensorCollisions.Count; i++)
            {
                if (sensorCollisions[i] == null)
                {
                    sensorCollisions.Remove(sensorCollisions[i]);
                }
            }
        }
        else if (type == "Sensor" || type == "Camera" || type == "Drone")
        {
            fovLight.color = originalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((type == "Sensor" || type == "Drone" || type == "Camera") && 
            other.gameObject.GetComponent<FOVEntity>() != null && 
            other.gameObject.GetComponent<TrailNode>() == null)
        {
            sensorCollisions.Add(other.gameObject);
            TextPopup.PopUpText("Movement detected in " + sensorName.text, 0.5f, 5);
            fovLight.color = triggeredColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((type == "Vision" || type == "Camera" || type == "Drone") && other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((type == "Vision" || type == "Camera" || type == "Drone") && other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = false;
        }
        if ((type == "Sensor" || type == "Camera" || type == "Drone") && sensorCollisions.Contains(other.gameObject))
        {
            sensorCollisions.Remove(other.gameObject);
            if (sensorCollisions.Count == 0)
            {
                fovLight.color = originalColor;
            }           
        }
    }
}
