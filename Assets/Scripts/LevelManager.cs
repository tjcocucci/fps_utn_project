using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player playerPrefab;
    public EnemySpawner enemySpawnerPrefab;
    public Level[] levels;
    public int currentLevelIndex;
    public AudioSource startGameSound;
    public Transform gameContainer;
    public MapGenerator mapGenerator;

    [HideInInspector]
    public Player player;

    [HideInInspector]
    private EnemySpawner enemySpawner;

    public int enemyKills;

    public event System.Action OnWin;

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        enemySpawner = Instantiate(
            enemySpawnerPrefab,
            Vector3.zero,
            Quaternion.identity,
            gameContainer
        );
    }

    void OnPlayerDeath()
    {
        enemySpawner.Disable();
        enemySpawner.CleanUp();
        player.OnObjectDied -= OnPlayerDeath;
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex > levels.Length)
        {
            Debug.LogError("Level index out of range!");
            return;
        }
        if (levelIndex == levels.Length)
        {
            OnWin?.Invoke();
            return;
        }
        if (levelIndex == 0)
        {
            if (player != null)
            {
                Debug.Log("Destroying player");
                Destroy(player.gameObject);
            }
            player = Instantiate(
                playerPrefab,
                levels[levelIndex].playerSpawnPosition,
                Quaternion.identity,
                gameContainer
            );
            player.OnObjectDied += OnPlayerDeath;
        }

        enemyKills = 0;

        currentLevelIndex = levelIndex;
        SetUpMap();
        Debug.Log("Level: " + currentLevelIndex);

        // Create Enemy Spawner
        enemySpawner.CleanUp();
        enemySpawner.SetUp(levels[currentLevelIndex], player);

        if (levelIndex == 0)
        {
            startGameSound.Play();
            UIOverlay.Instance.ShowInstructions();
            StartCoroutine(EnableSpawnerAfterDelay(3));
        }
        else
        {
            enemySpawner.Enable();
        }

        UIOverlay.Instance.StartUI();
    }

    IEnumerator EnableSpawnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemySpawner.Enable();
    }

    void SetUpMap()
    {
        mapGenerator.GenerateMap(levels[currentLevelIndex].map);
    }

    public void OnEnemyDeath()
    {
        enemyKills++;
        if (enemyKills == levels[currentLevelIndex].totalNumberOfEnemies)
        {
            Invoke("NextLevel", 1);
        }
    }

    void NextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }
}
