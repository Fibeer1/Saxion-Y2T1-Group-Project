using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RangerBackground : MonoBehaviour
{
    [SerializeField] private InventoryItem[] inventory;
    [SerializeField] private Image[] inventoryPanels;
    [SerializeField] Slider fatigueSlider;
    [SerializeField] Image fatigueSliderBackground;
    private Color fatigueFillColor = new Color(0.9f, 0.9f, 0, 0.75f);
    private Color fatigueEmptyColor = new Color(0, 0.9f, 0, 0.75f);
    public Player player;
    public Ranger ranger;

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

    public void RemoveItemFromInventory(InventoryItem item)
    {
        item.itemCount--;
        if (item.itemCount <= 0)
        {
            Destroy(item.gameObject);
        }
    }

    public void SetUpRangerBG(Player pPlayer)
    {
        player = pPlayer;
        ranger = player.currentObject.GetComponent<Ranger>();
    }
}
