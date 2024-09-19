using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RangerBackground : UIMarker
{
    [SerializeField] private InventoryItem[] inventory;
    [SerializeField] private Image[] inventoryPanels;
    public Player player;
    public Ranger ranger;

    private void Update()
    {
        HandleTargetTracking();
    }

    public void AddItemToInventory(GameObject item)
    {
        InventoryItem itemInventoryComponent = item.GetComponent<InventoryItem>();
        int firstEmptyIndex = -1; //The first empty panel in the inventory
        int firstOccupiedIndex = -1; //The first occupied panel with the same item as in this method
        for (int i = 0; i < inventory.Length; i++)
        {
            InventoryItem inventorySpace = inventory[i];
            if (inventorySpace == null && firstEmptyIndex == -1)
            {
                //if the first available space is not occupied, assign the firstEmptyIndex variable to it
                firstEmptyIndex = i;
            }
            else if (inventorySpace != null && inventorySpace.objectToPlace == itemInventoryComponent.objectToPlace)
            {
                //if the space is occupied, but the item is the same, assign the firstOccupiedIndex variable to it
                firstOccupiedIndex = i;
                break;
            }
        }
        if (firstOccupiedIndex != -1)
        {
            //If there is already an item like the one called in the method in the inventory, add it and destroy the current one
            inventory[firstOccupiedIndex].itemCount++;
            Destroy(item);
            return;
        }
        else if (firstEmptyIndex != -1)
        {
            inventory[firstEmptyIndex] = itemInventoryComponent;
            item.transform.SetParent(inventoryPanels[firstEmptyIndex].transform);
            item.transform.localPosition = Vector3.zero;
        }
        else
        {
            TextPopup.PopUpText("Cannot add item!\nInventory is full.", 0.5f, 1.5f);
        }
    }

    public void SetUpRangerBG(Player pPlayer)
    {
        player = pPlayer;
        target = player.currentObject.transform;        
        playerCam = player.GetComponent<Camera>();
        ranger = player.currentObject.GetComponent<Ranger>();
    }
}
