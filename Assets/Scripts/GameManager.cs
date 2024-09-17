using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static List<Ranger> rangers = new List<Ranger>();
    public static List<Poacher> poachers = new List<Poacher>();
    public static List<Animal> animals = new List<Animal>();
    [SerializeField] private TextMeshProUGUI rangerCount;
    [SerializeField] private TextMeshProUGUI animalCount;
    [SerializeField] private TextMeshProUGUI moneyCount;
    [SerializeField] private TMP_Dropdown sensorList;


    [SerializeField] private float passiveIncomeTimer;
    private float passiveIncomeTimeMin = 25f;
    private float passiveIncomeTimeMax = 60f;


    public int money;
    private int donationMoneyMin = 50;
    private int donationMoneyMax = 250;

    private bool hasGameEnded = false;

    private void Start()
    {
        rangers = FindObjectsOfType<Ranger>().ToList();
        poachers = FindObjectsOfType<Poacher>().ToList();
        animals = FindObjectsOfType<Animal>().ToList();
        List<FieldOfViewTrigger> sensors = FindObjectsOfType<FieldOfViewTrigger>().ToList();
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
        passiveIncomeTimer = Random.Range(passiveIncomeTimeMin, passiveIncomeTimeMax);
        money = 100; //Starting amount
    }


    private void Update()
    {
        if (hasGameEnded)
        {
            return;
        }
        

        moneyCount.text = "$" + money;
        rangerCount.text = "Rangers: " + rangers.Count;
        animalCount.text = "Animals left: " + animals.Count;

        HandlePassiveIncome();


        if (poachers.Count == 0)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You win!\nYou caught all poachers.", 0.5f, 0.1f);
        }
        if (animals.Count == 0)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You lose!\nThe poachers killed all animals.", 0.5f, 0.1f);
        }
        if (money <= 0)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You lose!\nYou ran out of money.", 0.5f, 0.1f);
        }
    }

    private void HandlePassiveIncome()
    {
        passiveIncomeTimer -= Time.deltaTime;
        if (passiveIncomeTimer <= 0)
        {
            passiveIncomeTimer = Random.Range(passiveIncomeTimeMin, passiveIncomeTimeMax);
            int moneyReceived = Random.Range(donationMoneyMin, donationMoneyMax);
            HandleMoneyChange("Donation received!\nMoney gained: " + moneyReceived + "$", moneyReceived);
        }
    }
    public void HandleMoneyChange(string text, int moneyChange)
    {
        TextPopup.PopUpText(text, 0.5f, 2);
        money += moneyChange;
    }
}
