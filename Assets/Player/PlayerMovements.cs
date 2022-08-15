
using System;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 exteriorVelocity;
    
    private Vector2 direction;
    public Vector2 Direction
    {
        get => direction;
        private set => direction = value;
    }
    private bool canMove = true;

    [SerializeField] private float frictionEV;
    [Space]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelerationAmount;
    [SerializeField] private float decelerationAmount;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InputHandler()
    {
        direction = Vector2.zero;

        if (canMove)
        {
            // REWIRED
            direction.x = Input.GetAxisRaw("Horizontal");
            direction.y = Input.GetAxisRaw("Vertical");
            direction.Normalize();
        }
    }

    public void MovementsFU()
    {
        rb.velocity = direction * maxSpeed * Time.fixedDeltaTime + exteriorVelocity;
        
        // Exterior velocity handled
        exteriorVelocity.x += (Mathf.Sign(exteriorVelocity.x) > 0f ? -1f : 1f) * frictionEV;
        exteriorVelocity.y += (Mathf.Sign(exteriorVelocity.y) > 0f ? -1f : 1f) * frictionEV;
    }

    public void AddExteriorVelocity(Vector2 _velocity)
    {
        exteriorVelocity += _velocity;
    }
}
