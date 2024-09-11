using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private bool canMove = true;
    public Interactable currentObject;
    [SerializeField] private float cameraSpeedDamper = 5;
    [SerializeField] private GameObject moveIndicator;
    Vector3 movement;


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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
        movement.Normalize();
        movement /= cameraSpeedDamper;
        transform.position += movement / cameraSpeedDamper;
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
            Ranger ranger = currentObject.GetComponent<Ranger>();
            if (Input.GetKey(KeyCode.Mouse1))
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
                }
                DeselectObject();
            }
        }
    }

    public void SelectObject(Interactable clickedObject)
    {
        currentObject = clickedObject;
        currentObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", new Color(0.5f, 0.5f, 0.5f));
    }

    public void DeselectObject()
    {
        currentObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
        currentObject = null;
    }
}
