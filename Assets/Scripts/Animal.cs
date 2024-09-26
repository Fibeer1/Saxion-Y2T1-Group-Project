using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Wanderer
{
    public bool isDead = false;
    private GameObject trapTriggered;
    [SerializeField] private AudioClip[] idleSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource3D;
    [SerializeField] private AudioSource audioSource2D;
    private int animalValue = 100; //Placeholder value

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shouldKeepTrackOfTrails = true;
        animalValue = GameManager.animalValue;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        HandleWandering();
    }

    void IdleSounds()
    {
        if (GetComponent<FOVEntity>().isBeingSeen && idleSounds.Length > 0)
        {
            audioSource3D.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
        }
    }

    public IEnumerator Die()
    {
        if (isDead)
        {
            yield break;
        }

        isDead = true;
        navMeshAgent.enabled = false;
        GameManager.animals.Remove(this);
        yield return new WaitForSeconds(1.5f);
        if (trapTriggered != null)
        {
            Destroy(trapTriggered);
        }
        audioSource2D.PlayOneShot(deathSound);
        if (name.Contains("Elephant"))
        {
            animator.Play("Armature|ElephantDeath");
        }
        else
        {
            animator.Play("Armature|death");
        }
        FOVDebug.FindFOVEntities();
        FindObjectOfType<GameManager>().HandleMoneyChange("One of your animals has died.\nMoney deducted: " + 
            animalValue + "$", animalValue);
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Trap>() != null)
        {
            other.GetComponent<Trap>().PlayAnimation("TriggerTrap");
            trapTriggered = other.gameObject;
            StartCoroutine(Die());
        }
    }
}
