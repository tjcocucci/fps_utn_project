using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player playerPrefab;
    public EnemySpawner enemySpawnerPrefab;
    public Level[] levels;
    public int currentLevelIndex;
    public Level currentLevel;
    public AudioSource startGameSound;
    public Transform gameContainer;

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
        foreach (Level level in levels)
        {
            level.gameObject.SetActive(false);
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

    // Start is called before the first frame update
    void Start() { }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex > levels.Length)
        {
            Debug.LogError("Level index out of range!");
            return;
        }
        if (levelIndex == levels.Length)
        {
            if (OnWin != null)
            {
                OnWin();
            }
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

        currentLevel?.gameObject.SetActive(false);
        currentLevelIndex = levelIndex;
        currentLevel = levels[currentLevelIndex];
        currentLevel.gameObject.SetActive(true);
        Debug.Log("Level: " + currentLevelIndex);

        // Create Enemy Spawner
        enemySpawner.CleanUp();
        enemySpawner.SetUp(currentLevel, player);

        if (levelIndex == 0)
        {
            startGameSound.Play();
            UIOverlay.Instance.ShowInstructions();
            StartCoroutine(enableSpawnerAfterDelay(3));
        }
        else
        {
            enemySpawner.Enable();
        }

        UIOverlay.Instance.StartUI();
    }

    IEnumerator enableSpawnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemySpawner.Enable();
    }

    public void OnEnemyDeath()
    {
        enemyKills++;
        if (enemyKills == currentLevel.totalNumberOfEnemies)
        {
            Invoke("NextLevel", 1);
        }
    }

    void NextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }
}
