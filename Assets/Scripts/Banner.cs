using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Banner : MonoBehaviour
{
    public TextMeshProUGUI bannerText;
    public GameObject gameContainer;
    public GameObject menuGameObject;
    public GameObject playButtonGameObject;
    public GameObject goToMenuButtonGameObject;
    private AudioSource audioSource;
    public AudioClip buttonClickClip;



    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void ShowBanner()
    {
        LeanTween.moveLocalY(gameObject, 50, 0.5f).setEaseOutBounce();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetText(string text)
    {
        bannerText.text = text;
    }

    void DeactivateButtons ()
    {
        playButtonGameObject.SetActive(false);
        goToMenuButtonGameObject.SetActive(false);
    }

    void ActivateButtons ()
    {
        playButtonGameObject.SetActive(true);
        goToMenuButtonGameObject.SetActive(true);
    }

    public void ShowInstructionBannerForSeconds(float seconds)
    {
        DeactivateButtons();
        ShowBanner();
        Invoke("HideBanner", seconds);
        Invoke("ActivateButtons", seconds+0.5f);
    }

    public void HideBanner()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LeanTween.moveLocalY(gameObject, 350, 0.5f).setEaseInBounce();
    }

    public void OnPlayButtonClick()
    {
        audioSource.PlayOneShot(buttonClickClip);
        Invoke("SwitchToGame", 0.5f);
        HideBanner();
    }

    public void OnBackToMenuButtonClick()
    {
        audioSource.PlayOneShot(buttonClickClip);
        Invoke("SwitchToMenu", 0.5f);
        HideBanner();
    }

    void SwitchToMenu()
    {
        gameContainer.SetActive(false);
        menuGameObject.SetActive(true);
    }

    void SwitchToGame () {
        LevelManager.Instance.LoadLevel(0);
        HideBanner();
    }

}
