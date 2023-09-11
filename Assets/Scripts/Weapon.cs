using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public Transform gunMuzzleTransform;
    public float damage = 10;
    public float timeBetweenShots = 0.5f;
    public LayerMask layers;
    private float timeForNextShot;

    // Start is called before the first frame update
    public void Start()
    {
        timeForNextShot = Time.time;
    }

    public void Shoot()
    {
        if (Time.time > timeForNextShot)
        {
            // Raycast shoot
            if (
                Physics.Raycast(
                    gunMuzzleTransform.position,
                    gunMuzzleTransform.forward,
                    out RaycastHit hit,
                    layers
                )
            )
            {
                Debug.Log(hit.transform.name);
                DamageableObject damageableObject = hit.transform.GetComponent<DamageableObject>();
                damageableObject?.TakeDamage(damage);
            }
            timeForNextShot = Time.time + timeBetweenShots;
        }
    }
}
