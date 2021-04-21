using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spr;
    private Player player;

    private bool isDead = false;
    private bool isChasing = false;
    private bool playerClose = false;

    public int maxHealth = 100;
    private int health = 0;

    public float forcePushBack = 5f;

    public float deadBodyDispawnTime = 5f;

    public float detectionRangeDay = 5f;
    public float chasingRangeDay = 2.5f;
    public float chasingRangeNight = 2.5f;
    public float attackRange = 0.7f;

    public float chasingSpeed = 2.5f;

    

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        if (health == 0)
            health = maxHealth;
    }

    private void Update()
    {
        if (!isDead && !isChasing)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (GameManager.gameManager.isDay)
            {
                if ((!playerClose && distance <= detectionRangeDay) || (playerClose && distance > detectionRangeDay))
                {
                    playerClose = !playerClose;
                    animator.SetTrigger("Angry");
                }

                if (!isChasing && distance <= chasingRangeDay)
                {
                    isChasing = true;
                }
            }
            else
            {
                AnimatorStateInfo ast = animator.GetCurrentAnimatorStateInfo(0);
                if (!ast.IsName("Zombie_Angry_Idle"))
                {
                    playerClose = true;
                    animator.SetTrigger("Angry");
                }

                if (playerClose && !isChasing && distance <= chasingRangeNight)
                {
                    isChasing = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isChasing)
        {
            Vector2 direction = player.transform.position - transform.position;

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.MovePosition(rb.position + direction.normalized * chasingSpeed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Call Hit animation

        // Push Back
        Vector2 nVec = (transform.position - player.transform.position).normalized;
        rb.AddForce(nVec * forcePushBack * (isChasing ? chasingSpeed : 1f));

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void TakeDamage(int damage, GameObject bullet)
    {
        health -= damage;

        // Call Hit animation

        // Push Back
        rb.AddForce(bullet.transform.right * forcePushBack * (isChasing ? chasingSpeed : 1f));

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(deadBodyDispawnTime);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(50, collision.gameObject);
            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRangeDay);
        Gizmos.DrawWireSphere(transform.position, chasingRangeDay);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chasingRangeNight);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
