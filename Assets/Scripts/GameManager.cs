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
    [SerializeField] private TMP_Dropdown upkeep;


    [SerializeField] private float moneyChangeTimer = 20;
    private float moneyChangeTime = 20;


    public int money;
    private int donationMoney = 200;
    private int rangerFee = -100;
    public static int poacherValue = 1000;
    public static int animalValue = -700;

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
        options.Add(new TMP_Dropdown.OptionData("Passive income: " + donationMoney + "$"));
        options.Add(new TMP_Dropdown.OptionData("Rangers' fee: " + rangerFee + "$"));
        options.Add(new TMP_Dropdown.OptionData("Poacher bounty: " + poacherValue + "$"));
        options.Add(new TMP_Dropdown.OptionData("Animal death cost: " + animalValue + "$"));
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

    public void UpdateUpkeepValues()
    {
        
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
