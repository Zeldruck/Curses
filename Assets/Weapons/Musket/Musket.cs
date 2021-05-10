using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musket : RangeWeaponClass
{
    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attack()
    {
        animator.SetTrigger("shoot");
        canAttack = false;
    }

    protected override void ShootBullet()
    {
        GameObject nBullet = Instantiate(bulletPrefab, shootTransform.position, transform.parent.transform.rotation);
        bullets.Add(nBullet);
        StartCoroutine(CWeaponAttack(bullets[bullets.Count - 1]));
    }

    private IEnumerator CWeaponAttack(GameObject bullet)
    {
        Rigidbody2D rbW = bullet.GetComponent<Rigidbody2D>();

        rbW.velocity = bullet.transform.right * bulletSpeed;

        yield return new WaitForSeconds(firerate);

        canAttack = true;

        yield return new WaitForSeconds(3f);

        bullets.Remove(bullet);
        Destroy(bullet);
    }
}
