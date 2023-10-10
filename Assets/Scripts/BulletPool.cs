using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet bulletPrefab;
    public int poolSize = 10;
    public List<Bullet> bullets;

    // Start is called before the first frame update
    void Start()
    {
        bullets = new List<Bullet>();
        for (int i = 0; i < poolSize; i++)
        {
            Bullet bulletInstance = Instantiate(bulletPrefab);
            bulletInstance.gameObject.SetActive(false);
            bulletInstance.transform.SetParent(transform);
            bullets.Add(bulletInstance);
        }
    }

    public Bullet GetBulletInstance()
    {
        Bullet bullet = GetFromInactivePool();
        if (bullet == null)
        {
            bullet = Instantiate(bulletPrefab);
            bullet.transform.SetParent(transform);
            bullets.Add(bullet);
        }
        return bullet;
    }

    public void ReturnBulletToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public Bullet GetFromInactivePool()
    {
        foreach (Bullet bullet in bullets)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                bullet.gameObject.SetActive(true);
                return bullet;
            }
        }
        return null;
    }
}
