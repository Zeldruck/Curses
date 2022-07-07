using System;
using System.Collections;
using UnityEngine;

public class Fairy : Enemy
{
    private bool isMoving = false;
    
    [Space]
    [SerializeField] private LineRenderer beamLine;
    [SerializeField] private GameObject fairyShield;
    private GameObject enemyShield;
    
    [Header("Shield Spell")]
    [SerializeField] private float shieldCastRange = 10f;
    [SerializeField] private float enemiesDetectionRange = 10f;
    [SerializeField] private string[] nonShieldableEnemies;
    private bool isShielding = false;
    private Enemy enemyShielded;
    private float timerTryGetEnemies = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // TODO
            // isMoving to follow the enemy she is shielding or move freely if she's not shielding
            
            if (timerTryGetEnemies > 0f && !isShielding)
            {
                timerTryGetEnemies -= Time.deltaTime;
            }
            else if (!isShielding)
            {
                // TODO
                // Wave effect ?
                
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemiesDetectionRange);
                
                // TODO
                // Take the closer enemy / or the lower hp / or the fairy preference (cheated combo?)
                
                // Debug, currently taking the first collider enemy
                foreach (Collider2D collider in colliders)
                {
                    Enemy enemy = collider.GetComponent<Enemy>();

                    if (enemy && !enemy.IsDead())
                    {
                        bool isShieldable = true;

                        if (nonShieldableEnemies != null && nonShieldableEnemies.Length > 0)
                        {
                            foreach (string t in nonShieldableEnemies)
                            {
                                if (enemy.GetType() == Type.GetType(t))
                                {
                                    isShieldable = false;
                                    break;
                                }
                            }
                        }

                        if (isShieldable)
                        {
                            ShieldEnemy(enemy);
                            break;
                        }
                    }
                }

                timerTryGetEnemies = 1.5f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && isMoving)
        {
            direction = transform.position - player.transform.position;
            Flip();

            //animator.SetFloat("Speed", direction.normalized.magnitude);

            rb.velocity = direction.normalized * stats.speed * Time.fixedDeltaTime;
        }
        else if (!isDead && !isMoving)
        {
            //animator.SetFloat("Speed", 0f);
        }

        if (!isDead && isShielding && enemyShielded != null)
        {
            beamLine.SetPosition(1, (enemyShielded.transform.position - transform.position) / transform.localScale.x);
        }
    }

    private void ShieldEnemy(Enemy enemy)
    {
        enemy.onDeath += StopShieldingEnemy;
        isShielding = true;

        enemyShielded = enemy;
        
        fairyShield.SetActive(true);
        beamLine.gameObject.SetActive(true);
        
        // TODO
        // Reduce the damage the enemy is taken
        
        // TODO
        // Beam anim, then activate the enemy shield
        enemyShield = Instantiate(fairyShield, enemyShielded.transform.position, Quaternion.identity);
        enemyShield.transform.localScale = enemyShielded.transform.localScale;
    }

    private void StopShieldingEnemy()
    {
        enemyShielded.onDeath -= StopShieldingEnemy;
        isShielding = false;

        enemyShielded = null;
        
        fairyShield.gameObject.SetActive(false);
        beamLine.gameObject.SetActive(false);
        
        Destroy(enemyShield);
    }
    
    protected override IEnumerator Die()
    {
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Death");
        
        StopShieldingEnemy();

        return base.Die();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, enemiesDetectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shieldCastRange);
    }
}
