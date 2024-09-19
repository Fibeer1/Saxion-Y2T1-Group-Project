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
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        itemCountText.text = itemCount.ToString() + "x";
    }

    public void PrepareItemToPlace()
    {
        if (player.currentObjectToPlace != null)
        {
            return;
        }
        GameObject newObject = Instantiate(objectToPlace, Vector3.zero, Quaternion.identity);
        player.SelectObjectToPlace(newObject, this);
    }
}
