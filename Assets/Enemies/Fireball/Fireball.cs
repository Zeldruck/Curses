using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private bool isDestroyed;
    private float timer;
    private int damage;

    private void Update()
    {
        if (timer >= 0f)
        {
            timer -= Time.deltaTime;
        }
        else if (!isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void LaunchFireball(Vector2 _direction, int _damage, float _despawnTime)
    {
        damage = _damage;
        GetComponent<Rigidbody2D>().velocity = _direction;
        timer = _despawnTime;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDestroyed)
            return;
        
        other.gameObject.GetComponent<Player>().TakeDamage(0);
        Destroy(gameObject);
    }
}
