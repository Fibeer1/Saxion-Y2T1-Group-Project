using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
}
