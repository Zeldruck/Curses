using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovements movements;
    
    private Animator animator;
    private SpriteRenderer sr;
    private bool isAttacking = false;
    private bool isStunned = false;
    

    [Header("Sprite")]
    [SerializeField] private float spriteTurnLerpSpeed;

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
        movements = GetComponent<PlayerMovements>();
        
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (weapons[0] != null)
            weapons[0].animator.speed = (weapons[0] as MeleeWeaponClass != null ? mAttackSpeed : rAttackSpeed);

        for (int i = 1; i < weapons.Count; i++)
        {
            if (weapons[i] == null)
                continue;

            weapons[i].child.SetActive(false);
            weapons[i].animator.speed = (weapons[i] as MeleeWeaponClass != null ? mAttackSpeed : rAttackSpeed);
        }
    }

    void Update()
    {
        movements.InputHandler();
        
        if (!isStunned)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;

            if (weapons[0].GetType() != typeof(MeleeWeaponClass) || (weapons[0].GetType() == typeof(MeleeWeaponClass) && weapons[0].canAttack))
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

            if (Input.GetMouseButton(0) && weapons[0].canAttack)
                weapons[0].Attack();

            /*if (Input.GetKeyDown(KeyCode.Tab) && weapons.Count > 1)
            {
                SwapWeapon(true);
            }*/
        }

        animator.SetFloat("speed", movements.Direction.magnitude);
    }

    private void FixedUpdate()
    {
        movements.MovementsFU();
    }

    public WeaponClass GetWeapon(int index)
    {
        return (index >= weapons.Count || index < 0) ? null : weapons[index];
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

    /*public void SwapWeapon(bool swapLeft)
    {
        if (weapons.Count <= 1)
        {
            return;
        }

        weapons[indexWeapon].child.SetActive(false);
        int direction = swapLeft ? -1 : 1;
        indexWeapon += (indexWeapon + direction == weapons.Count ? -indexWeapon : direction);

        if (indexWeapon < 0)
            indexWeapon = weapons.Count - 1;

        weapons[indexWeapon].child.SetActive(true);
        //weapons[indexWeapon].animator.speed = (weapons[indexWeapon] as MeleeWeaponClass ? mAttackSpeed : rAttackSpeed);
    }*/

    public void SwapWeapon(int index)
    {
        if (index > weapons.Count || index < 0)
            return;

        weapons[0].child.SetActive(false);

        WeaponClass[] temp = new WeaponClass[weapons.Count];

        for (int i = 0; i < weapons.Count; i++)
        {
            temp[i] = weapons[i];
        }
        
        for (int i = 0; i < weapons.Count; i++)
        {
            int place = i + weapons.Count - index;

            if (place >= weapons.Count)
                place = place - weapons.Count;

            weapons[place] = temp[i];
        }

        weapons[0].child.SetActive(true); 
    }

    public void Stunt(float duration)
    {
        StartCoroutine(IEStunt(duration));
    }

    private IEnumerator IEStunt(float duration)
    {
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
    }

    public void TakeDamage(int _damage)
    {
        
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
