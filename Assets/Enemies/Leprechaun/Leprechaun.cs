using System;
using System.Collections;
using UnityEngine;

public class Leprechaun : Enemy
{
    private bool isMoving = false;
    
    [SerializeField] private float detectionRange = 2.5f;
    [Space] 
    [SerializeField] private GameObject detectObj;

    private bool hasDetectedPlayer = false;
    private Vector2 runningPoint = Vector2.zero;
    private float timerDetection = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (!hasDetectedPlayer)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);

                if (distance <= detectionRange)
                {
                    hasDetectedPlayer = true;
                    isMoving = false;
                    detectObj.SetActive(true);
                    timerDetection = 1f;
                }
            }
            else if (timerDetection > 0f)
            {
                timerDetection -= Time.deltaTime;
            }
            else if (runningPoint == Vector2.zero)
            {
                // Set the edge of the map, inverse of the player direction
                detectObj.SetActive(false);
                isMoving = true;
                runningPoint = transform.position + transform.right * 2f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && hasDetectedPlayer && !isMoving)
        {
            direction = player.transform.position - transform.position;
            Flip();
        }
        else if (!isDead && hasDetectedPlayer && isMoving)
        {
            // Change with A* path
            direction = (Vector3)runningPoint - transform.position;
            Flip();
            
            animator.SetFloat("Speed", direction.normalized.magnitude);
            
            rb.velocity = direction.normalized * stats.speed * Time.fixedDeltaTime;
        }
        else if (!isDead && !isMoving)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    protected override IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Death");
        
        // Drop rare weapon

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
