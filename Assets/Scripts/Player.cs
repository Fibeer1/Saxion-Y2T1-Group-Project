using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private bool canMove = true;
    public Interactable currentObject;
    [SerializeField] private GameObject selectionCirclePrefab;
    public GameObject currentSelectionCircle;
    [SerializeField] private float cameraSpeedDamper = 5;
    [SerializeField] private GameObject moveIndicator;
    private Rigidbody rb;
    private Vector3 movement;
    private bool isMoving;

    public InventoryItem[] inventory = new InventoryItem[3];


    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
                        ranger.SelectTarget(hit.transform);
                        ranger.GetComponent<NavMeshAgent>().speed = ranger.runSpeed;
                    }
                    else if (hit.transform.tag == "Trap")
                    {
                        ranger.SelectTarget(hit.transform);
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
    }

    public void DeselectObject()
    {
        Destroy(currentSelectionCircle);
        currentSelectionCircle = null;
        currentObject = null;
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
