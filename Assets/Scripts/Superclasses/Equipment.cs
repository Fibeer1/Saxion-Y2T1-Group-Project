using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public GameObject itemUIPrefab;
    public InventoryItem originItem; //This reference tracks where this object was instantiated from
                                     //It will be used to deduct from the item count or remove it from the inventory entirely
}
