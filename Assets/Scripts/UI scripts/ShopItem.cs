using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int stock;
    [SerializeField] private TextMeshProUGUI stockText;
    public GameObject itemBeingSold;
    public GameObject soldOutIndicator;

    private void Update()
    {
        if (stockText != null)
        {
            stockText.text = "(Stock: " + stock + ")";
        }
    }
}
