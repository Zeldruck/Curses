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
    [Space]
    public float attackRate;
}

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    protected SpriteRenderer spr;
    protected Player player;

    protected Vector2 direction;

    protected float timerAttack = 0f;

    protected bool isDead = false;

    public Stats stats;

    public float forcePushBack = 5f;
    public float deadBodyDispawnTime = 5f;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        stats.health = stats.maxHealth;
    }

    protected virtual void Flip()
    {
        float angle = Vector2.SignedAngle(transform.right, direction.normalized);

        if (transform.localScale.x < 0f && (angle >= 90f || angle <= -90f))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.localScale.x > 0f && angle < 90f && angle > -90f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        
        stats.health -= damage;

        if (stats.health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public virtual void TakeDamage(int damage, GameObject bullet)
    {
        if (isDead) return;
        
        stats.health -= damage;

        if (stats.health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public bool IsDead() { return isDead; }

    protected virtual IEnumerator Die()
    {
        isDead = true;

        if (onDeath != null)
        {
            onDeath();
            onDeath = null;
        }

        yield return new WaitForSeconds(deadBodyDispawnTime);

        DestroyDeadBody();
    }

    public void DestroyDeadBody()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && !isDead)
        {
            TakeDamage(50, collision.gameObject);
            Destroy(collision.gameObject);
        }
    }
}
