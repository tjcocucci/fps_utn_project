using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class UIOverlay : MonoBehaviour
{
    private Player player;

    public Banner banner;
    private float healthPercent;
    public TMPro.TextMeshProUGUI UIText;
    private LevelManager levelManager;
    private bool pauseUpdates;
    public AudioClip winSound;
    public AudioClip loseSound;
    private AudioSource audioSource;

    private static UIOverlay _instance;
    public static UIOverlay Instance
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
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        levelManager = LevelManager.Instance;
        levelManager.OnWin += OnWin;
    }

    public void StartUI()
    {
        Debug.Log("Starting UI");
        pauseUpdates = false;
        player = FindObjectOfType<Player>();
        player.OnObjectDied += OnPlayerDeath;
    }

    void Update()
    {
        if (pauseUpdates)
        {
            return;
        }
        healthPercent = 0;
        if (player != null)
        {
            healthPercent = 100 * player.health / player.totalHealth;
            UpdateText();
        }
    }

    void UpdateText()
    {
        UIText.text =
            "Health: "
            + healthPercent.ToString("0")
            + "%"
            + "\n"
            + "Weapon: "
            + player.weaponController.weaponList[player.weaponIndex].weaponName
            + "\n"
            + "Level: "
            + (levelManager.currentLevelIndex + 1)
            + "\n"
            + "Kills: "
            + levelManager.enemyKills
            + "\n"
            + "Time: "
            + Time.time.ToString("0.00")
            + "\n";
    }

    public void ShowInstructions()
    {
        banner.SetText("Use WASD to move, mouse to aim, and left click to shoot");
        banner.ShowInstructionBannerForSeconds(2.5f);
    }

    public void OnPlayerDeath()
    {
        audioSource.PlayOneShot(loseSound);
        pauseUpdates = true;
        banner.SetText("You died!");
        banner.ShowBanner();
    }

    public void OnWin()
    {
        audioSource.PlayOneShot(winSound);
        pauseUpdates = true;
        banner.SetText("You win!");
        banner.ShowBanner();
    }
}
