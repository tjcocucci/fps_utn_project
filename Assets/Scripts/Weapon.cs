using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public Transform gunMuzzleTransform;
    public Bullet bulletPrefab;
    public float damage = 10;
    public float timeBetweenShots = 0.5f;
    private float timeForNextShot;
    public Transform bulletContarinerTransform;

    // Start is called before the first frame update
    public void Start()
    {
        bulletContarinerTransform = GameObject.Find("BulletContainer").transform;
        timeForNextShot = Time.time;
    }

    public void Shoot()
    {
        if (Time.time > timeForNextShot)
        {
            Bullet bullet = Instantiate(
                bulletPrefab,
                gunMuzzleTransform.position,
                gunMuzzleTransform.rotation,
                bulletContarinerTransform
            );
            bullet.damage = damage;
            timeForNextShot = Time.time + timeBetweenShots;
        }
    }
}
