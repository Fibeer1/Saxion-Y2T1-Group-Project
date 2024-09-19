using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SensorDropdown : MonoBehaviour
{
    public TMP_Dropdown sensorList;
    public List<FieldOfViewTrigger> sensors;

    private void Start()
    {
        sensors = FindObjectsOfType<FieldOfViewTrigger>().ToList();
        TMP_Dropdown.OptionDataList sensorsList = new TMP_Dropdown.OptionDataList();

        sensors = sensors.Where(sensor => sensor.type != "Vision").ToList();

        sensors = sensors.OrderByDescending(sensor =>
        {
            string sensorName = sensor.sensorName.text;
            int number;

            // Attempt to extract the number from the sensor name, default to a large number if it fails
            string numberPart = sensorName.Split(' ').Last(); // Get the last part of the string
            if (int.TryParse(numberPart, out number))
            {
                return number;
            }
            return int.MaxValue;
        }).ToList();

        foreach (var sensor in sensors)
        {
            TMP_Dropdown.OptionData sensorData = new TMP_Dropdown.OptionData(sensor.sensorName.text);
            sensorsList.options.Add(sensorData);
        }
        sensorList.ClearOptions();
        sensorList.AddOptions(sensorsList.options);
    }

    public void SwitchToSensor()
    {
        Player playerCam = FindObjectOfType<Player>();

        playerCam.StartCoroutine(playerCam.MoveTowardsPosition(sensors[sensorList.value].transform.position, 3));
    }

    public void OnPointerClick()
    {
        Toggle[] dropdownSensors = GetComponentsInChildren<Toggle>();
        for (int i = 0; i < sensors.Count; i++)
        {
            if (sensors[i].sensorCollisions.Count != 0)
            {
                //todo: make triggered sensors appear yellow in the dropdown
                //dropdownSensors[i].colors.normalColor = Color.yellow;
            }
        }
    }
}
