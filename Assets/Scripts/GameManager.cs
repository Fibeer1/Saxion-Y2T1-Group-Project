using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static List<Ranger> rangers = new List<Ranger>();
    public static List<Poacher> poachers = new List<Poacher>();
    public static List<Animal> animals = new List<Animal>();
    public List<FieldOfViewTrigger> motionSensors = new List<FieldOfViewTrigger>();
    private bool hasGameEnded = false;

    private void Start()
    {
        rangers = FindObjectsOfType<Ranger>().ToList();
        poachers = FindObjectsOfType<Poacher>().ToList();
        animals = FindObjectsOfType<Animal>().ToList();
        motionSensors = FindObjectsOfType<FieldOfViewTrigger>().ToList();
    }


    private void Update()
    {
        if (hasGameEnded)
        {
            return;
        }

        if (poachers.Count == 0)
        {
            hasGameEnded = true;
            Debug.Log("You win!");
        }
        if (animals.Count == 0)
        {
            hasGameEnded = true;
            Debug.Log("You lose!");
        }
    }
}
