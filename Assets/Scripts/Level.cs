using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int totalNumberOfEnemies;
    public float timeBetweenSpawns;
    public EnemyType enemyType;
    public float enemySpeed;
    public float enemyDamage;
    public float enemyHealth;
    public int enemyWeaponIndex;
    public float minSpawnDistanceToPlayer;
    public Vector3 playerSpawnPosition;
    public GameObject map;
    public GameObject spawnPlane;

    [HideInInspector]
    public Bounds spawnBounds;

    private Vector3 mapCenter;

    void Start()
    {
        mapCenter = map.transform.position;
        spawnBounds = spawnPlane.GetComponent<MeshRenderer>().bounds;
    }

    public Bounds GetSpawnBounds()
    {
        return spawnPlane.GetComponent<MeshRenderer>().bounds;
    }

}
