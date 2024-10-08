using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FOVDebug : MonoBehaviour
{
    public static FOVDebug instance;
    private List<FOVEntity> fovEntities = new List<FOVEntity>();
    private bool shouldEnableEntities = true;

    private void Awake()
    {
        instance = this;
        OnFindFOVEntities();

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            shouldEnableEntities = !shouldEnableEntities;            
        }
        //Toggle visibility of all entities
        foreach (var entity in fovEntities)
        {
            if (entity == null)
            {
                fovEntities.Remove(entity);
                break;
            }

            //If the entity is not visible by a ranger/sensor, disable its renderers
            if (!entity.isBeingSeen)
            {
                List<Renderer> entityRenderers = entity.GetComponentsInChildren<Renderer>().ToList();
                foreach (var renderer in entityRenderers)
                {
                    renderer.enabled = !shouldEnableEntities;
                }
            }
            entity.enabled = shouldEnableEntities;
        }
    }

    private void OnFindFOVEntities()
    {
        fovEntities.Clear();
        fovEntities = FindObjectsOfType<FOVEntity>().ToList();
    }

    public static void FindFOVEntities()
    {
        instance.OnFindFOVEntities();
    }
}
