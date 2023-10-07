using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public float explosionRadius = 5;
    public float timeToExplode = 3;
    public float initialForce = 20;
    public ParticleSystem explosionEffect;

    void Awake()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * initialForce, ForceMode.Impulse);
        Invoke("ExplodeEffect", timeToExplode - 0.1f);
        Invoke("Explode", timeToExplode);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            DamageableObject damageableObject = collider.GetComponent<DamageableObject>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(100);
            }
        }
        Destroy(gameObject);
    }

    void ExplodeEffect()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
    }
}
