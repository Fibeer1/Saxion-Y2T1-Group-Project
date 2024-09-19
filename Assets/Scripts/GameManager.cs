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


    public int money;
    private int donationMoney = 150;
    private int rangerFee = -100;
    public static int poacherValue = 1000;
    public static int animalValue = -700;
    public static int trapValue = 100;

    private bool hasGameEnded = false;

    private void Start()
    {
        rangers = FindObjectsOfType<Ranger>().ToList();
        poachers = FindObjectsOfType<Poacher>().ToList();
        animals = FindObjectsOfType<Animal>().ToList();

        rangerFee *= rangers.Count;
        money = 100; //Starting amount

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
        

        moneyCount.text = "$" + money;
        rangerCount.text = "Rangers: " + rangers.Count;
        animalCount.text = "Animals left: " + animals.Count;

        HandleIncomeAndFees();


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
            HandleMoneyChange("Rangers' fee.\nMoney deducted: " + rangerFee + "$", rangerFee);
            HandleMoneyChange("Donation received!\nMoney gained: " + donationMoney + "$", donationMoney);
        }
    }
    public void HandleMoneyChange(string text, int moneyChange)
    {
        TextPopup.PopUpText(text, 0.5f, 2);
        money += moneyChange;
    }
}
