using System;
using System.Collections;
using UnityEngine;

public class Druid : Enemy
{
    private bool isRunningAway = false;

    [SerializeField] private float dangerRange = 5f;
    
    [Header("Resurection Spell")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private string[] nonResurectableEnnemies;
    [SerializeField] private float resurectRange = 10f;
    [SerializeField] private float timeCastResurectSpell = 15f;
    private float timerCastResurectSpell = 0f;
    private bool isResurecting = false;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (!isRunningAway && distance <= dangerRange)
            {
                isRunningAway = true;
            }
            else if (isRunningAway && distance > dangerRange)
            {
                isRunningAway = false;
            }

            if (timerCastResurectSpell > 0f)
                timerCastResurectSpell -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isRunningAway && !isResurecting)
        {
            direction = transform.position - player.transform.position;
            Flip();

            animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.velocity = direction.normalized * stats.speed * Time.fixedDeltaTime;
        }
        else if (!isDead && !isRunningAway)
        {
            animator.SetFloat("Speed", 0f);
        }

        if (!isDead)
        {
            if (timerCastResurectSpell <= 0f && !isResurecting)
            {
                Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, resurectRange);

                bool isCasting = false;
                
                if (col.Length > 0)
                {
                    foreach (Collider2D collider in col)
                    {
                        Enemy enemy = collider.gameObject.GetComponent<Enemy>();

                        if (enemy != null && enemy.IsDead())
                        {
                            bool isResurectable = true;

                            if (nonResurectableEnnemies != null && nonResurectableEnnemies.Length > 0)
                            {
                                foreach (string t in nonResurectableEnnemies)
                                {
                                    if (enemy.GetType() == Type.GetType(t))
                                    {
                                        isResurectable = false;
                                        break;
                                    }
                                }
                            }

                            if (isResurectable)
                            {
                                ResurectEnemy(enemy);
                                isCasting = true;
                            }
                        }
                    }
                }

                if (isCasting)
                {
                    animator.SetTrigger("Resurect");
                    timerCastResurectSpell = timeCastResurectSpell;
                    isResurecting = true;
                }
                else
                {
                    timerCastResurectSpell = 2f;
                }
            }
        }
    }

    private void ResurectEnemy(Enemy enemy)
    {
        GameObject skeleton = Instantiate(skeletonPrefab, enemy.transform.position, Quaternion.identity);
        
        enemy.DestroyDeadBody();
    }

    private void EndResurectEnemy()
    {
        isResurecting = false;
    }

    protected override IEnumerator Die()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger("Death");

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, dangerRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, resurectRange);
    }
}
