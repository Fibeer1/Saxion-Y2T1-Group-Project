using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Image itemSprite;
    public int itemCount;
    [SerializeField] private TextMeshProUGUI itemCountText;
    public GameObject objectToPlace;

    private void Start()
    {
        
    }

    private void Update()
    {
        itemCountText.text = itemCount.ToString();
    }

    public void PrepareItemToPlace()
    {

    }
}
