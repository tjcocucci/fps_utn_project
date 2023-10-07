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
    public float enemyHealth;
    public int enemyWeaponIndex;
    public float minSpawnDistanceToPlayer;
    public Vector3 playerSpawnPosition;

    [HideInInspector]
    public Bounds spawnBounds;

    public Map map;
    public Transform[,] GetSpawnPositions()
    {
        return map.tileMap;
    }

}
