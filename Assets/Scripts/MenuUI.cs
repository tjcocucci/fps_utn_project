using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject gameContainer;
    public GameObject mainMenuItems;
    public AudioSource buttonClickSound;

    public void OnPlayButtonClicked()
    {
        buttonClickSound.Play();
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
        buttonClickSound.Play();
        Invoke("QuitGame", 0.5f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
