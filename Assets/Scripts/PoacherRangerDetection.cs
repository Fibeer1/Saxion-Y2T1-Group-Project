using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoacherRangerDetection : MonoBehaviour
{
    [SerializeField] private Poacher poacher;

    private void Start()
    {
        poacher = GetComponentInParent<Poacher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ranger>() != null)
        {
            poacher.TargetRanger(other.GetComponent<Ranger>());
        }
        else if (other.GetComponent<Animal>() != null)
        {
            poacher.ShootAnimal(other.GetComponent<Animal>());
        }
    }
}
