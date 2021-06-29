using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    private SpriteRenderer sr;
    private bool isAttacking = false;
    private bool canMove = true;


    [SerializeField] private float speed;

    [Header("Sprite")]
    [SerializeField] private float spriteTuenLerpSpeed;

    [Header("Weapon")]
    [SerializeField] private List<WeaponClass> weapons;
    private int indexWeapon = 0;
    [SerializeField] private GameObject weapon;
    [SerializeField] private float offset;
    [SerializeField] private float weaponLerpSpeed;

    public float mAttackSpeed = 1f;
    public float rAttackSpeed = 1f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        weapons[0].animator.speed = (weapons[0] as MeleeWeaponClass ? mAttackSpeed : rAttackSpeed);

        for (int i = 1; i < weapons.Count; i++)
        {
            weapons[i].child.SetActive(false);
            weapons[i].animator.speed = (weapons[i] as MeleeWeaponClass ? mAttackSpeed : rAttackSpeed);
        }
    }

    void Update()
    {
        direction = Vector2.zero;

        if (canMove)
        {
            direction.x = Input.GetAxisRaw("Horizontal");
            direction.y = Input.GetAxisRaw("Vertical");
            direction.Normalize();


            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;

            if (weapons[indexWeapon].GetType() != typeof(MeleeWeaponClass) || (weapons[indexWeapon].GetType() == typeof(MeleeWeaponClass) && weapons[indexWeapon].canAttack))
            {
                /* Weapon Movement */
                Vector2 posMouseVec = (mousePosition - transform.position).normalized;
                weapon.transform.localPosition = Vector2.Lerp(weapon.transform.localPosition, posMouseVec * offset, Time.deltaTime * weaponLerpSpeed);

                RotateSprite(mousePosition);

                /* Weapon LookAt */
                Vector3 dirAngle = (weapon.transform.position - transform.position).normalized * offset;
                float angle = Mathf.Atan2(dirAngle.y, dirAngle.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                if (Mathf.Sign(weapon.transform.localScale.y) >= 0f && weapon.transform.rotation.eulerAngles.z >= 90f && weapon.transform.rotation.eulerAngles.z <= 270f)
                {
                    weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, -weapon.transform.localScale.y, weapon.transform.localScale.z);
                }
                else if (Mathf.Sign(weapon.transform.localScale.y) < 0f && (weapon.transform.rotation.eulerAngles.z < 90f || weapon.transform.rotation.eulerAngles.z > 270f))
                {
                    weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, -weapon.transform.localScale.y, weapon.transform.localScale.z);
                }
            }

            if (Input.GetMouseButton(0) && weapons[indexWeapon].canAttack)
                weapons[indexWeapon].Attack();

            if (Input.GetKeyDown(KeyCode.Tab) && weapons.Count > 1)
            {
                weapons[indexWeapon].child.SetActive(false);
                indexWeapon += (indexWeapon + 1 == weapons.Count ? -indexWeapon : 1);
                weapons[indexWeapon].child.SetActive(true);

                weapons[indexWeapon].animator.speed = (weapons[indexWeapon] as MeleeWeaponClass ? mAttackSpeed : rAttackSpeed);
            }
        }

        animator.SetFloat("speed", direction.magnitude);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    private void RotateSprite(Vector2 mousePosition)
    {
        float angle = Vector2.SignedAngle(transform.right, -((Vector2)transform.position - mousePosition).normalized);

        if (sr.flipX && (angle >= 90f || angle <= -90f))
        {
            sr.flipX = false;
        }
        else if (!sr.flipX && angle < 90f && angle > -90f)
        {
            sr.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
        }
    }

    /*private void WeaponAttackSpecial()
    {
        GameObject nBullet = Instantiate(bulletPrefab, weapon.transform.position, weapon.transform.rotation);
        nBullet.transform.localScale = new Vector3(5f, nBullet.transform.localScale.y, nBullet.transform.localScale.z);
        nBullet.transform.position = nBullet.transform.position + nBullet.transform.right * nBullet.transform.localScale.x / 2f;
        bullets.Add(nBullet);

        StartCoroutine(CWeaponAttackSpecial(bullets[bullets.Count - 1]));
    }

    private IEnumerator CWeaponAttackSpecial(GameObject bullet)
    {
        canShoot = false;
        canMove = false;

        Rigidbody2D rbW = bullet.GetComponent<Rigidbody2D>();

        for (int i = 0; i < 20; i++)
        {
            rb.MovePosition(rb.position + -(Vector2)bullet.transform.right * speed * Time.fixedDeltaTime);
            rbW.MovePosition(rbW.position + -(Vector2)bullet.transform.right * speed * Time.fixedDeltaTime);
            yield return new WaitForSeconds(0.05f);
        }

        canMove = true;

        bullets.Remove(bullet);
        Destroy(bullet);

        yield return new WaitForSeconds(firerate);

        canShoot = true;
    }*/
}
