using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private bool canMove = true;   

    [Header("Object Selection")]
    public Interactable currentObject;
    public GameObject currentSelectionCircle;
    [SerializeField] private GameObject selectionCirclePrefab;    
    [SerializeField] private GameObject rangerBackground;
    [SerializeField] private GameObject moveIndicator;

    [Header("Object Placing")]
    public GameObject currentObjectToPlace;
    public Material currentObjectToPlaceSelectedMaterial;
    private Material currentObjectToPlaceOriginalMaterial;

    [Header("Camera Movement")]
    [SerializeField] private float cameraSpeedDamper = 5;    
    private Rigidbody rb;
    private Vector3 movement;
    private bool isMoving;

    [Header("Inventory")]
    public InventoryItem[] inventory = new InventoryItem[3];


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rangerBackground.SetActive(false);
    }

    private void Update()
    {
        HandleObjectInputs();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            return;
        }
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            return;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        bool[] movementChecks = 
            { 
                movement.x > 0 && Physics.Raycast(transform.position, Vector3.right, 10), //Right
                movement.z > 0 && Physics.Raycast(transform.position, Vector3.forward, 10), //Forward
                movement.x < 0 && Physics.Raycast(transform.position, -Vector3.right, 10), //Left
                movement.z < 0 && Physics.Raycast(transform.position, -Vector3.forward, 10), //Back
            };
        foreach (var check in movementChecks)
        {
            if (check)
            {
                return;
            }
        }
        movement.Normalize();
        movement /= cameraSpeedDamper;
        rb.MovePosition(rb.position + movement);
    }

    public void AddItemToInventory(InventoryItem item)
    {
        int firstEmptyIndex = -1; //The first empty panel in the inventory
        int firstOccupiedIndex = -1; //The first occupied panel with the same item as in this method
        for (int i = 0; i < inventory.Length; i++)
        {
            InventoryItem inventorySpace = inventory[i];
            if (inventorySpace == null)
            {
                //if the space is not occupied, assign the firstEmptyIndex variable to it
                firstEmptyIndex = i;
            }
            else if (inventorySpace.objectToPlace == item.objectToPlace)
            {
                //if the space is occupied, but the item is the same, assign the firstOccupiedIndex variable to it
                firstOccupiedIndex = i;
                break;
            }
        }
        if (firstOccupiedIndex != -1)
        {
            //If there is already an item like the one called in the method in the inventory, add it
            inventory[firstOccupiedIndex].itemCount++;
        }
        else if (firstEmptyIndex != -1)
        {
            inventory[firstEmptyIndex] = item;
        }
        else
        {
            TextPopup.PopUpText("Cannot add item!\nInventory is full.", 0.5f, 1.5f);
        }

    }

    private void HandleObjectInputs()
    {
        if (currentObject == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.tag == "Ground")
                {
                    DeselectObject();
                    return;
                }
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectObject();
            return;
        }


        if (currentObject.GetComponent<Ranger>() != null)
        {
            Ranger ranger = currentObject.GetComponent<Ranger>();
            if (Input.GetKeyDown(KeyCode.Mouse1) && !ranger.duringAnimation)
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (ranger.target != null)
                    {
                        ranger.DeselectTarget();
                    }                    
                    if (hit.transform.tag == "Ground")
                    {
                        Instantiate(moveIndicator, hit.point, Quaternion.identity);
                        ranger.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                        ranger.GetComponent<NavMeshAgent>().speed = ranger.defaultSpeed;
                    }
                    else if (hit.transform.GetComponent<Poacher>() != null)
                    {
                        ranger.SelectTarget(hit.transform, "Chase");
                    }
                    else if (hit.transform.tag == "Trap")
                    {
                        ranger.SelectTarget(hit.transform, "DisarmTrap");
                    }
                    else if (hit.transform.name.Contains("MotionSensor"))
                    {
                        ranger.SelectTarget(hit.transform, "Remove Object", false);
                    }
                }
            }
        }
    }

    public void SelectObject(Interactable clickedObject)
    {
        if (currentObject != null)
        {
            DeselectObject();
        }
        currentObject = clickedObject;

        currentSelectionCircle = Instantiate(selectionCirclePrefab, currentObject.transform.position, Quaternion.identity, currentObject.transform);
        if (currentObject.GetComponent<Ranger>() != null)
        {
            rangerBackground.SetActive(true);
            rangerBackground.GetComponent<RangerBackground>().target = currentObject.transform;
            rangerBackground.GetComponent<RangerBackground>().SyncRangerInventoryWithPlayerInventory();
            rangerBackground.GetComponent<RangerBackground>().ranger = currentObject.GetComponent<Ranger>();
        }       
    }

    public void DeselectObject()
    {
        Destroy(currentSelectionCircle);
        currentSelectionCircle = null;
        currentObject = null;
        rangerBackground.SetActive(false);
    }

    public IEnumerator MoveTowardsPosition(Vector3 position, float duration)
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;
        float elapsedTime = 0;
        position.z -= 20;
        position.y += 20;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (Vector3.Distance(transform.position, position) < 0.1f)
            {
                isMoving = false;
                yield break;
            }
            transform.position = Vector3.Slerp(transform.position, position, elapsedTime / duration);           
            bool[] movementChecks =
            {
                movement.x > 0 && Physics.Raycast(transform.position, Vector3.right, 10), //Right
                movement.z > 0 && Physics.Raycast(transform.position, Vector3.forward, 10), //Forward
                movement.x < 0 && Physics.Raycast(transform.position, -Vector3.right, 10), //Left
                movement.z < 0 && Physics.Raycast(transform.position, -Vector3.forward, 10), //Back
            };
            foreach (var check in movementChecks)
            {
                if (check)
                {
                    isMoving = false;
                    yield break;
                }
            }
            yield return null;
        }
        isMoving = false;
    }

}
