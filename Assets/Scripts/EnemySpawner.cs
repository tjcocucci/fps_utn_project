using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Transform enemyContainerTransform;
    public Enemy enemyPrefab;
    private float enemyHeight;
    public bool isEnabled;

    private int numberOfSpawnedEnemies;
    private int totalNumberOfEnemies;
    private float timeBetweenSpawns;
    private Player player;
    private Transform playerTransform;
    private float minSpawnDistanceToPlayer;
    private List<Enemy> enemies = new List<Enemy>();

    private float enemySpeed;
    private float enemyDamage;
    private float enemyHealth;
    private int enemyWeaponIndex;
    public EnemyType type;

    private Bounds planeBounds;
    private float timeForNextSpawn;

    public void  Start ()
    {
        enemyContainerTransform = new GameObject("EnemyContainer").transform;
        enemyContainerTransform.parent = transform;
        enemyHeight = enemyPrefab.GetComponent<CharacterController>().height;
    }

    public void SetUp(Level level, Player player)
    {
        this.player = player;
        playerTransform = this.player.transform;
        
        numberOfSpawnedEnemies = 0;
        totalNumberOfEnemies = level.totalNumberOfEnemies;
        timeBetweenSpawns = level.timeBetweenSpawns;
        timeForNextSpawn = Time.time + timeBetweenSpawns;
        minSpawnDistanceToPlayer = level.minSpawnDistanceToPlayer;
        // planeBounds = level.GetSpawnBounds();
        
        enemySpeed = level.enemySpeed;
        type = level.enemyType;
        enemyDamage = level.enemyDamage;
        enemyHealth = level.enemyHealth;
        enemyWeaponIndex = level.enemyWeaponIndex;


    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    void Update()
    {
        if (isEnabled)
        {
            if (
                Time.time > timeForNextSpawn
                && numberOfSpawnedEnemies < totalNumberOfEnemies
                && player.IsAlive()
            )
            {
                Spawn();
                numberOfSpawnedEnemies++;
                timeForNextSpawn = Time.time + timeBetweenSpawns;
            }
        }
    }

    void Spawn()
    {
        Vector3 spawnPosition = Vector3.zero;

        bool foundUsableSpawnPosition = false;
        for (int i = 0; i < 500; i++)
        {
            spawnPosition = new Vector3(
                Random.Range(planeBounds.min.x, planeBounds.max.x),
                enemyHeight * 4,
                Random.Range(planeBounds.min.z, planeBounds.max.z)
            );
            if (
                playerTransform != null
                && Vector3.Distance(spawnPosition, playerTransform.position)
                    > minSpawnDistanceToPlayer
            )
            {
                foundUsableSpawnPosition = true;
                break;
            }
        }
        if (!foundUsableSpawnPosition)
        {
            Debug.Log("Could not find a spawn position");
        }
        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemies.Add(enemy);
        SetUpEnemyProperties(enemy);
    }

    public void CleanUp()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemies.Clear();
    }

    void SetUpEnemyProperties(Enemy enemy)
    {
        enemy.SetType(type);
        enemy.speed = enemySpeed;
        enemy.damage = enemyDamage;
        enemy.totalHealth = enemyHealth;
        enemy.weaponIndex = enemyWeaponIndex;
        enemy.playerTransform = playerTransform;
        enemy.OnObjectDied += LevelManager.Instance.OnEnemyDeath;
    }

}
