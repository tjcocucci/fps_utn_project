using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public float health;
    public float totalHealth;
    public event System.Action OnObjectDied;

    public bool IsAlive()
    {
        return health > 0;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (OnObjectDied != null)
            {
                OnObjectDied();
            }
        }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        health = totalHealth;
    }
}
