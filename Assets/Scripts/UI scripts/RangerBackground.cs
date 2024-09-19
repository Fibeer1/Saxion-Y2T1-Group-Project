using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RangerBackground : MonoBehaviour
{
    [SerializeField] private Image[] inventoryPanels;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SyncRangerInventoryWithPlayerInventory()
    {
        for (int i = 0; i < player.inventory.Length; i++)
        {
            InventoryItem inventoryItem = player.inventory[i];
            if (inventoryItem == null)
            {
                continue;
            }
            GameObject itemDuplicate = Instantiate(inventoryItem.gameObject, inventoryPanels[i].transform, false);
        }
    }
}
