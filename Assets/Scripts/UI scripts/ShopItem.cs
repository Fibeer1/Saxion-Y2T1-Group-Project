using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int stock;
    [SerializeField] private TextMeshProUGUI stockText;
    public InventoryItem itemBeingSold;
    public GameObject soldOutIndicator;

    void Start()
    {
        
    }

    private void Update()
    {
        if (stockText != null)
        {
            stockText.text = "(Stock: " + stock + ")";
        }
    }
}
