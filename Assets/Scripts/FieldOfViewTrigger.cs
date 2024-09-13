using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FieldOfViewTrigger : MonoBehaviour
{
    public string type; //Can be Sensor or Vision
    private TextMeshPro sensorName;
    [SerializeField] private List<GameObject> sensorCollisions = new List<GameObject>();
    [SerializeField] private Renderer circleRenderer;
    private Color originalColor;

    private void Start()
    {
        if (type == "Sensor")
        {
            originalColor = circleRenderer.material.color;
            sensorName = GetComponentInChildren<TextMeshPro>();
        }
    }

    private void Update()
    {
        if (type == "Sensor")
        {
            if (sensorCollisions.Count != 0)
            {
                circleRenderer.material.color = new Color(1, 1, 0, originalColor.a);
            }
            else
            {
                circleRenderer.material.color = originalColor;
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (type == "Sensor" && 
            other.gameObject.GetComponent<FOVEntity>() != null && 
            other.gameObject.GetComponent<TrailNode>() == null)
        {
            sensorCollisions.Add(other.gameObject);
            TextPopup.PopUpText("Movement detected in " + sensorName.text, 0.5f, 5);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (type == "Vision" && other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == "Vision" && other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = false;
        }
        if (type == "Sensor" && sensorCollisions.Contains(other.gameObject))
        {
            sensorCollisions.Remove(other.gameObject);
        }
    }

}
