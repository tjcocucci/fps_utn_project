using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialSlider : MonoBehaviour
{

    public GameObject mainMenuUI;
    public int activeSlideIndex;
    public Slide[] slides;
    public AudioSource buttonClickSound;
    public TextMeshProUGUI textMesh;
    public Image image;

    void OnEnable()
    {
        activeSlideIndex = 0;
        ShowSlide();
    }
    private void ShowSlide() {
        image.sprite = slides[activeSlideIndex].image;
        textMesh.text = slides[activeSlideIndex].text;
    }

    public void OnBackToMenuButtonClick()
    {
        buttonClickSound.Play();
        Invoke("BackToMenu", 0.5f);
    }

    public void BackToMenu()
    {
        mainMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnNextButtonClicked()
    {
        buttonClickSound.Play();
        if (activeSlideIndex < slides.Length - 1)
        {
            activeSlideIndex++;
        }
        ShowSlide();
    }

    public void OnPreviousButtonClicked()
    {
        buttonClickSound.Play();
        if (activeSlideIndex > 0)
        {
            activeSlideIndex--;
        }
        ShowSlide();
    }

}

[System.Serializable]
public class Slide {
    public string text;
    public Sprite image;
}