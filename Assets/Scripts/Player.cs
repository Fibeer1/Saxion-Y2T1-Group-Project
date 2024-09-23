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
    public InventoryItem currentObjectToPlaceSprite;
    public Material currentObjectToPlaceSelectedMaterial;
    [SerializeField] private List<Material> currentObjectToPlaceOriginalMaterials = new List<Material>();
    private Ranger rangerOrderedToPlaceObject;

    [Header("Camera Movement")]
    [SerializeField] private float cameraSpeedDamper = 5;    
    private Rigidbody rb;
    private Vector3 movement;
    private bool isMoving;


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

    private void HandleObjectInputs()
    {
        if (currentObject == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectObject();
            DeselectObjectToPlace(true);
            return;
        }

        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        bool hasRayHit = Physics.Raycast(ray, out RaycastHit hit);

        if (currentObjectToPlace != null && hasRayHit && rangerOrderedToPlaceObject == null)
        {
            currentObjectToPlace.transform.position = hit.point;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && hasRayHit)
        {
            if (hit.transform.tag == "Ground" && currentObjectToPlace != null)
            {
                //THIS BREAKS USING BUTTONS WHILE HAVING SELECTED A RANGER
                //TODO: Come up with a way to have this code uncommented and at the same time be able to click UI buttons
                //DeselectObject();                
                //return;
            }
        }


        if (currentObject.GetComponent<Ranger>() != null)
        {
            Ranger ranger = currentObject.GetComponent<Ranger>();
            if (ranger.duringAnimation)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && hasRayHit)
            {
                if (ranger.target != null)
                {
                    ranger.DeselectTarget();
                }
                if (hit.transform.tag == "Ground")
                {
                    Instantiate(moveIndicator, hit.point, Quaternion.identity);
                    if (ranger.actionToPerform == "PlaceObject")
                    {
                        DeselectObjectToPlace(true);
                    }
                    ranger.SelectTarget(null, hit.point, "Walk", false);                                   
                }
                else if (hit.transform.GetComponent<Poacher>() != null)
                {
                    ranger.SelectTarget(hit.transform, hit.transform.position, "ChasePoacher");
                }
                else if (hit.transform.tag == "Trap")
                {
                    ranger.SelectTarget(hit.transform, hit.transform.position, "DisarmTrap");
                }
                else if (hit.transform.name.Contains("MotionSensor"))
                {
                    ranger.SelectTarget(hit.transform, hit.transform.position, "PickUpEquipment");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && hasRayHit)
            {
                if (hit.transform.tag == "Ground" && currentObjectToPlace != null && rangerOrderedToPlaceObject == null)
                {
                    ranger.SelectTarget(currentObjectToPlace.transform, hit.point, "PlaceObject");
                    rangerOrderedToPlaceObject = ranger;
                }
            }
        }
    }

    public void SelectObjectToPlace(GameObject objectToPlace, InventoryItem itemSprite)
    {
        if (currentObjectToPlace != null)
        {
            return;
        }
        currentObjectToPlaceOriginalMaterials.Clear();
        currentObjectToPlace = objectToPlace;
        currentObjectToPlaceSprite = itemSprite;

        MeshRenderer[] renderers = currentObjectToPlace.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            currentObjectToPlaceOriginalMaterials.Add(renderer.material);
            renderer.material = currentObjectToPlaceSelectedMaterial;
        }
        Collider[] colliders = currentObjectToPlace.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        MonoBehaviour[] scripts = currentObjectToPlace.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }
    }

    public void DeselectObjectToPlace(bool shouldDestroyObject = false)
    {
        if (currentObjectToPlace == null)
        {
            return;
        }
        rangerOrderedToPlaceObject = null;        
        if (shouldDestroyObject)
        {
            Destroy(currentObjectToPlace);
            currentObjectToPlace = null;
            currentObjectToPlaceSprite = null;
            return;
        }

        Renderer[] renderers = currentObjectToPlace.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = currentObjectToPlaceOriginalMaterials[i];
        }
        Collider[] colliders = currentObjectToPlace.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        MonoBehaviour[] scripts = currentObjectToPlace.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = true;
        }
        currentObjectToPlace = null;
        currentObjectToPlaceSprite = null;
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

            rangerBackground.GetComponent<RangerBackground>().SetUpRangerBG(this);
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
