using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Entity
{
    [SerializeField] private protected Player player;

    private void OnMouseDown()
    {
        if (player.currentObject != null)
        {
            player.DeselectObject();
        }

        Ranger ranger = GetComponent<Ranger>();
        Drone drone = GetComponent<Drone>();

        if (ranger != null && !ranger.duringAnimation)
        {
            player.SelectObject(ranger);
        }
        else if (drone != null)
        {
            player.SelectObject(drone);
        }
    }

}
