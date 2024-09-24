using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        if (animationName == "Disarm")
        {
            animator.Play("Armature|disarming");
        }
        else if (animationName == "TriggerTrap")
        {
            animator.Play("Armature|Snap!");
        }
    }
}
