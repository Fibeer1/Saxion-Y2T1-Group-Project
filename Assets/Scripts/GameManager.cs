using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static List<Ranger> rangers = new List<Ranger>();
    public static List<Poacher> poachers = new List<Poacher>();
    public static List<Animal> animals = new List<Animal>();
    [SerializeField] private TextMeshProUGUI rangerCount;
    [SerializeField] private TextMeshProUGUI animalCount;
    [SerializeField] private TextMeshProUGUI moneyCount;
    [SerializeField] private TMP_Dropdown upkeep;
    [SerializeField] private Sprite moneyGainBackground;
    [SerializeField] private Sprite moneyLossBackground;


    [SerializeField] private float moneyChangeTimer = 20;
    private float moneyChangeTime = 20;

    [SerializeField] private Transform[] poacherSpawnPositions;
    [SerializeField] private GameObject poacherPrefab;
    [SerializeField] private float poacherSpawnTimer;
    private float poacherSpawnTimeMin = 30;
    private float poacherSpawnTimeMax = 60;
    private float maxPoachers = 3;

    public int villageUpgradeCount = 0;
    public int poacherSpawnReductionPercentage = 0;

    public int money;
    private int donationMoney = 0;
    private int donationMin = 0;
    private int donationMax = 300;
    private int rangerFee = -50;
    public static int poacherValue = 500;
    public static int animalValue = -750;
    public static int trapValue = 100;

    private bool hasGameEnded = false;

    private void Start()
    {
        rangers = FindObjectsOfType<Ranger>().ToList();
        poachers = FindObjectsOfType<Poacher>().ToList();
        animals = FindObjectsOfType<Animal>().ToList();

        rangerFee *= rangers.Count;
        money = 1600; //Starting amount
        poacherSpawnTimer = Random.Range(poacherSpawnTimeMin, poacherSpawnTimeMax);

        upkeep.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("Monthly:"));
        options.Add(new TMP_Dropdown.OptionData("Donations: " + donationMoney + "$", moneyGainBackground));
        options.Add(new TMP_Dropdown.OptionData("Rangers' fee: " + rangerFee + "$", moneyLossBackground));
        options.Add(new TMP_Dropdown.OptionData(" "));
        options.Add(new TMP_Dropdown.OptionData("One time:"));
        options.Add(new TMP_Dropdown.OptionData("Poacher bounty: " + poacherValue + "$", moneyGainBackground));
        options.Add(new TMP_Dropdown.OptionData("Trap dismantling: " + trapValue + "$", moneyGainBackground));
        options.Add(new TMP_Dropdown.OptionData("Animal death cost: " + animalValue + "$", moneyLossBackground));
        upkeep.AddOptions(options);
    }


    private void Update()
    {
        if (hasGameEnded)
        {
            return;
        }
        

        moneyCount.text = "$ " + money;
        rangerCount.text = rangers.Count + "x";
        animalCount.text = animals.Count + "x";

        HandleIncomeAndFees();
        HandlePoacherSpawning();


        if (villageUpgradeCount >= 6)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You win!\nYou have eliminated poaching.", 0.5f, 0.1f);
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

    private void HandlePoacherSpawning()
    {
        poacherSpawnTimer -= Time.deltaTime;
        if (poacherSpawnTimer <= 0 && poachers.Count < maxPoachers)
        {
            poacherSpawnTimer = Random.Range(poacherSpawnTimeMin, poacherSpawnTimeMax);
            poacherSpawnTimer += poacherSpawnTimer * (poacherSpawnReductionPercentage / 100);
            Transform chosenSpawnPos = poacherSpawnPositions[Random.Range(0, poacherSpawnPositions.Length)];
            Poacher spawnedPoacher = Instantiate(poacherPrefab, chosenSpawnPos.position, Quaternion.identity).GetComponent<Poacher>();
            poachers.Add(spawnedPoacher);
            FOVDebug.FindFOVEntities();
        }
    }

    public void HandleUpkeepColors()
    {
        //Toggle[] dropdownItems = GetComponentsInChildren<Toggle>();
        //Debug.Log("Open");
        //foreach (var item in dropdownItems)
        //{
        //    Debug.Log(item.name);
        //    TextMeshProUGUI itemName = item.GetComponentInChildren<TextMeshProUGUI>();
        //    Image itemBackground = item.GetComponentInChildren<Image>();
        //    if (!itemName.text.Contains("$"))
        //    {
        //        continue;
        //    }
        //    if (itemName.text.Contains("-"))
        //    {
        //        itemBackground.color = new Color(1, 0.5f, 0.5f);
        //    }
        //    else
        //    {
        //        itemBackground.color = new Color(0.5f, 1, 0.5f);
        //    }
        //}
    }

    private void HandleIncomeAndFees()
    {

        moneyChangeTimer -= Time.deltaTime;
        if (moneyChangeTimer <= 0)
        {
            moneyChangeTimer = moneyChangeTime;
            donationMoney = Random.Range(donationMin, donationMax);
            HandleMoneyChange("Rangers' fee.\nMoney deducted: " + rangerFee + "$", rangerFee);
            HandleMoneyChange("Donation received!\nMoney gained: " + donationMoney + "$", donationMoney);
        }
    }
    public void HandleMoneyChange(string text, int moneyChange)
    {
        TextPopup.PopUpText(text, 0.5f, 2);
        money += moneyChange;
    }

    public void BuyVillageUpgrade(VillageUpgrade villageUpgrade)
    {
        int moneyafterPurchase =  money - villageUpgrade.price;
        if (moneyafterPurchase <= 0)
        {
            TextPopup.PopUpText("Not enough money!", 0.5f, 2);
        }
        else
        {
            money -= villageUpgrade.price;
            poacherSpawnReductionPercentage += villageUpgrade.poacherPercentageReduction;
            if (poacherSpawnReductionPercentage >= 50)
            {
                maxPoachers = 2;
            }
            villageUpgradeCount++;
            villageUpgrade.boughtIndicator.SetActive(true);
        }
    }
}
