using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public GameObject child;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();
    }

    public virtual void Attack()
    {
        
    }

    public virtual void AltAttack()
    {

    }
}
