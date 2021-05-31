using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    public int maxHealth;
    public int health;
    public int damage;
    public float speed;
}

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    protected SpriteRenderer spr;
    protected Player player;

    protected bool isDead = false;
    

    public Stats stats;

    public float forcePushBack = 5f;
    public float deadBodyDispawnTime = 5f;

    public float detectionRangeDay = 5f;
    public float chasingRangeDay = 2.5f;
    public float chasingRangeNight = 2.5f;
    public float attackRange = 0.7f;

    // Start is called before the first frame update
    protected void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        stats.health = stats.maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        stats.health -= damage;

        // Call Hit animation


        if (stats.health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public virtual void TakeDamage(int damage, GameObject bullet)
    {
        stats.health -= damage;

        // Call Hit animation


        if (stats.health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    protected IEnumerator Die()
    {
        isDead = true;
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(deadBodyDispawnTime);

        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(50, collision.gameObject);
            Destroy(collision.gameObject);
        }
    }

    protected void OnDrawGizmos()
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
