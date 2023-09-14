using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
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
    public GameObject spawnPlane;

    [HideInInspector]
    public Bounds spawnBounds;

    public Map map;

}
