using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject[] Tutorials;
    public int TutorialStep
    {
        get { return tutorialStep; }
        set
        {
            tutorialStep = value;
            DataManager.SetIntData("TutorialStep", tutorialStep);

            if (tutorialStep > 0)
                Tutorials[tutorialStep - 1].SetActive(false);
        }
    }
    public int tutorialStep;
    public bool IsTutorial;
    public GameObject InfoPanel;
    public Button CloseButton;
    public TextMeshProUGUI PulseText;
    public TextMeshProUGUI FeverText;
    public TextMeshProUGUI GainText;

    public InfoObject InfoObject1;
    public InfoObject InfoObject2;
    public TextMeshProUGUI InfoObjectText1;
    public TextMeshProUGUI InfoObjectText2;

    public Slider BGMSound;
    public Slider SFXSound;

    public Material SkyboxMaterial;
    public Image MovePanel;

    public Texture2D cursorTexture;

    public TMP_Dropdown Resolution;
    public Toggle FullScreen;
    public Vector2 hotSpot = Vector2.zero;
    private void Start()
    {
        SetInfoUI();
        BGMSound.onValueChanged.AddListener(delegate { BGMChangeCheck(); });
        SFXSound.onValueChanged.AddListener(delegate { SFXhangeCheck(); });
        FullScreen.onValueChanged.AddListener(delegate { FullScreenChange(); });
        CloseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Open Info";
        MovePanel.DOColor(new Color(0, 0, 0, 0), 1f).OnComplete(() =>
        {
            
        });

        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
}
    void FullScreenChange()
    {
        SetResolution(Resolution.value);
        PlayerPrefs.SetInt("FullScreen", FullScreen.isOn ? 1 : 0);
    }
    public void BackToLobby()
    {
        MovePanel.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Main");
        });
    }
    public void EndingBackToLobby()
    {
        MovePanel.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() =>
        {
            DataManager.ResetData();
            SceneManager.LoadScene("Main");
        });
    }

    public void SetResolution(int index)
    {
        if(index == 0)
        {
            Screen.SetResolution(1024, 768, FullScreen.isOn);
        }
        else if(index == 1)
        {
            Screen.SetResolution(1280, 720, FullScreen.isOn);
        }
        else if(index == 2)
        {
            Screen.SetResolution(1920, 1080, FullScreen.isOn);
        }
        Resolution.value = index;
        DataManager.SetIntData("Resolution", index);
    }
    public float timer = 0;
    public TextMeshProUGUI timerText;
    private void Update()
    {
        if (!BlackHole.instance.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int hours = Mathf.FloorToInt(timer / 3600f);
        int minutes = Mathf.FloorToInt((timer % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        DataManager.SetFloatData("PlayTime", timer);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public GameObject SettingPanel;
    public void OnSettingButton()
    {
        SettingPanel.SetActive(!SettingPanel.activeSelf);
    }

    public void BGMChangeCheck()
    {
        SoundManager.Instance.BGMAudioSource.volume = BGMSound.value;
        BGMSound.GetComponentInChildren<TextMeshProUGUI>().text = (BGMSound.value * 100).ToString("F0");
        DataManager.SetFloatData("BGMVolume", BGMSound.value);
    }

    public void SFXhangeCheck()
    {
        SoundManager.Instance.SFXSoundVolume = SFXSound.value * 100;
        SFXSound.GetComponentInChildren<TextMeshProUGUI>().text = (SFXSound.value * 100).ToString("F0");
        DataManager.SetFloatData("SFXVolume", SFXSound.value);
        SoundManager.Instance.SyncVolume();
    }


    public void SetInfoUI()
    {
        PulseText.text = Spawner.Instance.SpawnPanel.Rambda + "\n" + Spawner.Instance.SpawnPanel.SuccessRange.ToString("F2");
        FeverText.text = "x" + FeverGage.Instance.FeverRambda + "\n" + "x" + FeverGage.Instance.feverBonus + "\n" + Mathf.Round((FeverGage.Instance.FiverRetriggerChance * 100)) + "%";
        GainText.text = (Object.StarPointMultiplier * 100).ToString("F2") + "%\n" + (Object.CosmicPointMultiplier * 100).ToString("F2") + "%";
    }

    public void SetInfoObject((int, float) table1, (int ,float) table2)
    {
        InfoObject1.Level = table1.Item1;
        InfoObjectText1.text = (int)(table1.Item2) + "%";
        InfoObject2.Level = table2.Item1;
        InfoObjectText2.text = (int)(table2.Item2) + "%";
    }

    public void OnInfoUICloseButton()
    {
        
        bool IsClosed = string.Equals(CloseButton.GetComponentInChildren<TextMeshProUGUI>().text, "Open Info");
        if (IsClosed)
        {
            InfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(InfoPanel.GetComponent<RectTransform>().anchoredPosition.x, -250);
            CloseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close Info";
        }
        else
        {
            InfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(InfoPanel.GetComponent<RectTransform>().anchoredPosition.x, 250);
            CloseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Open Info";
        }
    }
}
