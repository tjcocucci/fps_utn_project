using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
    public float timeToDestroy = 3.0f;
    private Rigidbody rb;
    public float offset = 0.1f;
    public LayerMask enemyCollisionMask;
    public LayerMask playerCollisionMask;
    public LayerMask obstacleCollisionMask;
    public float damage { get; set; }
    public AudioSource bulletObstacleHitSound;
    public AudioSource bulletPlayerOrEnemyHitSound;
    public AudioSource bulletShootSound;

    // Start is called before the first frame update
    void Awake()
    {
        bulletShootSound.Play();
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = speed * Time.deltaTime;
        CheckCollisions(distance);
        transform.Translate(Vector3.forward * distance);
    }

    void CheckCollisions(float distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if ( // Enemy or player collision
            Physics.Raycast(
                ray,
                out hit,
                distance + offset,
                enemyCollisionMask | playerCollisionMask,
                QueryTriggerInteraction.Collide
            )
        )
        {
            DamageableObject damageableObject = hit.collider.GetComponent<DamageableObject>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if ( // Obstacle collision

            Physics.Raycast(
                ray,
                out hit,
                distance,
                obstacleCollisionMask,
                QueryTriggerInteraction.Collide
            )
        )
        {
            Destroy(gameObject);
        }
    }
}
