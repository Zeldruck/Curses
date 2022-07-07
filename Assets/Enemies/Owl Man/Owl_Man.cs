using System;
using System.Collections;
using UnityEngine;

public class Owl_Man : Enemy
{
    [SerializeField] private float detectionRange = 2.5f;
    [SerializeField] private AudioClip screechAudioClip;

    private bool hasDetectedPlayer = false;
    private bool isMoving = false;

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
                    animator.SetTrigger("Detection");
                    // Play audio clip screech
                }
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
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            if (screenPoint.x >= Screen.width + 1.5f || screenPoint.y >= Screen.height + 1.5f)
            {
                Destroy(gameObject);
            }
            else
            {
                rb.velocity = Vector2.up * stats.speed * Time.fixedDeltaTime;
            }
        }
    }

    private void StartFlyingEvent()
    {
        isMoving = true;
    }

    protected override IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
