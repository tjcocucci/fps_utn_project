using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(AudioSource))]
public class UIOverlay : MonoBehaviour
{
    private Player player;

    public Banner banner;
    private float healthPercent;
    public TMPro.TextMeshProUGUI UIText;
    public TMPro.TextMeshProUGUI InventoryText;

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
            UpdateInventoryText();
        }
    }

    void UpdateText()
    {
        Weapon weapon = player.weaponController.weapon;
        int ammo = weapon.ammo;
        Debug.Log("ammo: " + ammo);
        int magazineSize = weapon.magazineSize;
        UIText.text =
            "Health: "
            + healthPercent.ToString("0")
            + "%"
            + "\n"
            + "Weapon: "
            + weapon.weaponName
            + "     "
            + weapon.ammo
            + " / "
            + magazineSize
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

    public void UpdateInventoryText()
    {
        InventoryText.text =
            "Primary Weapon Ammo: "
            + player.inventory.primaryWeaponAmmo
            + "\n"
            + "Secondary Weapon Ammo: "
            + player.inventory.secondaryWeaponAmmo
            + "\n"
            + "Grenades: "
            + player.inventory.granadeCount
            + "\n"
            + "Health Packs: "
            + player.inventory.healthPacks
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
