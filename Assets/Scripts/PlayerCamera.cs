using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCamera : MonoBehaviour
{
    private bool canMove = true;
    public Interactable currentObject;
    public GameObject currentObjectLight;
    [SerializeField] private float cameraSpeed = 3;
    [SerializeField] private GameObject moveIndicator;
    [SerializeField] private GameObject selectionLight;


    void Start()
    {
        
    }

    private void Update()
    {
        HandleMovement();
        HandleObjectInputs();

    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward / cameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right / cameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward / cameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right / cameraSpeed;
        }
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
        }


        if (currentObject.GetComponent<Ranger>() != null)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.tag == "Ground")
                    {
                        Instantiate(moveIndicator, hit.point, Quaternion.identity);
                        currentObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);                        
                    }
                }
                DeselectObject();
            }
        }
    }

    public void SelectObject(Interactable clickedObject)
    {
        currentObject = clickedObject;
        Vector3 intendedObjectPosition = currentObject.transform.position + Vector3.up * 2;
        currentObjectLight = Instantiate(selectionLight, intendedObjectPosition, Quaternion.identity);
    }

    public void DeselectObject()
    {
        currentObject = null;
        Destroy(currentObjectLight);
    }
}
