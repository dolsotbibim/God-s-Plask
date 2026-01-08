using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class MainUI : MonoBehaviour
{
    public Slider BGMSound;
    public Slider SFXSound;
    public Image MovePanel;
    public TMP_Dropdown Resolution;
    public Toggle FullScreen;
    public void OnResetData()
    {
        int resolution = DataManager.GetIntData("Resolution", 2);
        float BGM = DataManager.GetFloatData("BGMVolume", 0.5f);
        float SFX = DataManager.GetFloatData("SFXVolume", 0.5f);
        bool fullScreen = DataManager.GetIntData("FullScreen", 1) == 0 ? false : true;
        PlayerPrefs.DeleteAll();
        BGMSound.value = BGM;
        SFXSound.value = SFX;
        Resolution.value = resolution;
        FullScreen.isOn = fullScreen;
    }
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    public void OnStart()
    {
        MovePanel.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() => 
        {
            SceneManager.LoadScene("Game");
        });
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void Start()
    {
        FullScreen.onValueChanged.AddListener(delegate { FullScreenChange(); });

        FullScreen.isOn = PlayerPrefs.GetInt("FullScreen", 1) == 0 ? false : true;
        SetResolution(PlayerPrefs.GetInt("Resolution", 2));

        MovePanel.DOColor(new Color(0, 0, 0, 0), 1f).OnComplete(() =>
        {

        });
        
        BGMSound.onValueChanged.AddListener(delegate { BGMChangeCheck(); });
        SFXSound.onValueChanged.AddListener(delegate { SFXhangeCheck(); });
        BGMSound.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        SFXSound.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

    }

    void FullScreenChange()
    {
        SetResolution(PlayerPrefs.GetInt("Resolution", 2));
        PlayerPrefs.SetInt("FullScreen", FullScreen.isOn ? 1 : 0);
    }

    public void SetResolution(int index)
    {
        if (index == 0)
        {
            Screen.SetResolution(1024, 768, FullScreen.isOn);
        }
        else if (index == 1)
        {
            Screen.SetResolution(1280, 720, FullScreen.isOn);
        }
        else if (index == 2)
        {
            Screen.SetResolution(1920, 1080, FullScreen.isOn);
        }
        Resolution.value = index;
        PlayerPrefs.SetInt("Resolution", index);
    }

    public void BGMChangeCheck()
    {
        SoundManager.Instance.BGMAudioSource.volume = BGMSound.value;
        BGMSound.GetComponentInChildren<TextMeshProUGUI>().text = (BGMSound.value * 100).ToString("F0");
        PlayerPrefs.SetFloat("BGMVolume", BGMSound.value);
    }

    public void SFXhangeCheck()
    {
        SoundManager.Instance.SFXSoundVolume = SFXSound.value * 100;
        SFXSound.GetComponentInChildren<TextMeshProUGUI>().text = (SFXSound.value * 100).ToString("F0");
        PlayerPrefs.SetFloat("SFXVolume", SFXSound.value);
        SoundManager.Instance.SyncVolume();
    }
}