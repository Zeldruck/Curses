using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeaponClass : WeaponClass
{
    protected List<GameObject> bullets;

    [Header("Weapon")]
    [SerializeField] protected Transform shootTransform;

    [Header("Weapon Attack1")]
    public GameObject bulletPrefab;
    public float firerate = 0.5f;
    public float bulletSpeed = 15f;

    protected virtual void ShootBullet()
    {

    }
}
