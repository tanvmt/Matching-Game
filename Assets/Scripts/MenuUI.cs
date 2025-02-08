using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject buttonPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject ratingPanel;

    [Header("Settings")]
    [SerializeField] Button musicButton;
    [SerializeField] Sprite[] musicSprites;
    [SerializeField] Slider musicSlider;
    [SerializeField] float musicVolume;
    [SerializeField] Button soundButton;
    [SerializeField] Sprite[] soundSprites;
    [SerializeField] Slider soundSlider;
    [SerializeField] float soundVolume;

    [Header("Rating")]
    [SerializeField] Button[] ratingButtons;
    [SerializeField] Sprite[] ratingSprites;

    void Start()
    {
        buttonPanel.SetActive(true);
        settingsPanel.SetActive(false);
        helpPanel.SetActive(false);
        ratingPanel.SetActive(false);
        musicVolume = musicSlider.value;
        soundVolume = soundSlider.value;
    }

    public void OpenSettingsPanel()
    {
        buttonPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        buttonPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenHelpPanel()
    {
        buttonPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        buttonPanel.SetActive(true);
        helpPanel.SetActive(false);
    }

    public void OpenRatingPanel()
    {
        buttonPanel.SetActive(false);
        ratingPanel.SetActive(true);
    }

    public void CloseRatingPanel()
    {
        buttonPanel.SetActive(true);
        ratingPanel.SetActive(false);
    }

    public void ToggleMusic()
    {
        if (musicButton.image.sprite == musicSprites[0])
        {
            musicButton.image.sprite = musicSprites[1];
            musicVolume = musicSlider.value;
            musicSlider.value = 0f;
        }
        else
        {
            musicButton.image.sprite = musicSprites[0];
            musicSlider.value = musicVolume;
        }
    }

    public void ToggleSound()
    {
        if (soundButton.image.sprite == soundSprites[0])
        {
            soundButton.image.sprite = soundSprites[1];
            soundVolume = soundSlider.value;
            soundSlider.value = 0f;
        }
        else
        {
            soundButton.image.sprite = soundSprites[0];
            soundSlider.value = soundVolume;
        }
    }

    public void RateGame(int rating)
    {
        for (int i = 0; i < ratingButtons.Length; i++)
        {
            if (i < rating)
            {
                ratingButtons[i].image.sprite = ratingSprites[1];
            }
            else
            {
                ratingButtons[i].image.sprite = ratingSprites[0];
            }
        }
    }

    public void OpenFacebook()
    {
        Application.OpenURL("https://www.facebook.com/TanVMT");
    }

    public void OpenInstagram()
    {
        Application.OpenURL("https://www.instagram.com/tanvmt");
    }
    
}
