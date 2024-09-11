using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Lifeform
{
    [SerializeField] private protected Player player;

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (player.currentObject != null)
        {
            player.DeselectObject();
        }

        Ranger ranger = GetComponent<Ranger>();

        if (ranger != null && !ranger.duringAnimation)
        {
            player.SelectObject(ranger);
        }
    }

}
