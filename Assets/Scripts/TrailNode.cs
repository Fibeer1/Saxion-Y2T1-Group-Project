using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailNode : Entity
{
    public Animal animal;
    public TrailNode nextTrailNode;
    [SerializeField] private float lifeTime = 15;

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            if (animal != null)
            {
                animal.trail.Remove(this);
            }            
            Destroy(gameObject);
        }
    }
}
