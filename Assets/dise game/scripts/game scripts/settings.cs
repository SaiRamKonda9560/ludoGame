using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public struct settingsData
{
    public bool mute;
}
public class settings : MonoBehaviour
{
    public List<Button> volumeButtons;
    public Sprite muteSprite;
    public Sprite volumeSprite;
    public settingsData settingsData;
    private void Start()
    {
        loadSettingsData();
        setSpriteToVolumeButtons();
    }
    void loadSettingsData()
    {
        var key = "settings";
        if (PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(json,settingsData);
                }
                catch
                {

                }
            }
            else
            {
                saveSettingsData();
            }
        }
        else
        {
            saveSettingsData();
        }
    }
    void saveSettingsData()
    {
        var key = "settings";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(settingsData));
        PlayerPrefs.Save();
    }

    public void muteToggle()
    {
        settingsData.mute =!settingsData.mute;
        saveSettingsData() ;
        setSpriteToVolumeButtons();
    }
    public void setSpriteToVolumeButtons()
    {
        var sprite = (settingsData.mute ? muteSprite : volumeSprite);
        foreach (var button in volumeButtons)
        {
            if (button)
            {
                button.image.sprite = sprite;
            }
        }
    }
}
