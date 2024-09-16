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
    [SerializeField] private TextMeshProUGUI poacherCount;
    [SerializeField] private TextMeshProUGUI animalCount;


    public int money;
    private bool hasGameEnded = false;

    private void Start()
    {
        rangers = FindObjectsOfType<Ranger>().ToList();
        poachers = FindObjectsOfType<Poacher>().ToList();
        animals = FindObjectsOfType<Animal>().ToList();
    }


    private void Update()
    {
        if (hasGameEnded)
        {
            return;
        }
        poacherCount.text = "Poachers left: " + poachers.Count;
        rangerCount.text = "Rangers: " + rangers.Count;
        animalCount.text = "Animals left: " + animals.Count;

        if (poachers.Count == 0)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You win! \n You caught all poachers.", 0.5f, 0.1f);
        }
        if (animals.Count == 0)
        {
            hasGameEnded = true;
            EndScreen.ToggleEndScreen("You lose! \n The poachers killed all animals.", 0.5f, 0.1f);
        }
    }
}
