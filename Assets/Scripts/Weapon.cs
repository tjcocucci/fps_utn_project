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
    public int ammo;
    public int magazineSize;

    // Start is called before the first frame update
    public void Start()
    {
        bulletContarinerTransform = GameObject.Find("BulletContainer").transform;
        timeForNextShot = Time.time;
    }

    public void Shoot()
    {
        if (Time.time > timeForNextShot && ammo > 0)
        {
            Bullet bullet = Instantiate(
                bulletPrefab,
                gunMuzzleTransform.position,
                gunMuzzleTransform.rotation,
                bulletContarinerTransform
            );
            ammo--;
            bullet.damage = damage;
            timeForNextShot = Time.time + timeBetweenShots;
        }
    }
    
    public int Reload(int ammoCount)
    {
        int ammoToReload = magazineSize - ammo;
        if (ammoCount < ammoToReload)
        {
            ammo += ammoCount;
            return 0;
        }
        else
        {
            ammo = magazineSize;
            return ammoCount - ammoToReload;
        }
    }
}
