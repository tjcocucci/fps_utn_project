using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuUI : MonoBehaviour
{
    public GameObject gameContainer;
    public GameObject mainMenuItems;
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnEnable()
    {
        gameContainer.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void OnPlayButtonClicked()
    {
        audioSource.PlayOneShot(buttonClickSound);
        Invoke("SwitchToGame", 0.5f);
    }

    void SwitchToGame()
    {
        gameContainer.SetActive(true);
        gameObject.SetActive(false);
        LevelManager.Instance.LoadLevel(0);
    }

    public void OnQuitButtonClicked()
    {
        audioSource.PlayOneShot(buttonClickSound);
        Invoke("QuitGame", 0.5f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
