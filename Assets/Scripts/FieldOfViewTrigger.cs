using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<FOVEntity>() != null)
        {
            other.GetComponent<FOVEntity>().isBeingSeen = false;
        }
    }

}
