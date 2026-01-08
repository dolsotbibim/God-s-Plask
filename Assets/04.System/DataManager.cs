using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI.Extensions.ColorPicker;

public class DataManager : MonoBehaviour
{
    List<string> Objets = new List<string>();
    private void Start()
    {
        Object.StarPointMultiplier = 1;
        Object.CosmicPointMultiplier = 1;
        Application.targetFrameRate = 60;
        UIManager.Instance.Resolution.value = GetIntData("Resolution", 2);
        UIManager.Instance.FullScreen.isOn = GetIntData("FullScreen", 1) == 0 ? false : true;
        UIManager.Instance.SetResolution(GetIntData("Resolution", 2));

        int TutorialCleared = GetIntData("TutorialCleared", 0);
        if (TutorialCleared != 0)
        {
            UIManager.Instance.tutorialStep = 99;
            UIManager.Instance.IsTutorial = false;
        }
        else
        {
            UIManager.Instance.tutorialStep = GetIntData("TutorialStep", 0);
            UIManager.Instance.IsTutorial = true;
        }
        Flask.Instance.Level = GetIntData("FlaskLevel", 1);
        Flask.Instance.colorValue = GetFloatData("ColorValue", 0f);
        UpgradeManager.Instance.FlaskGage.FlaskPoint = GetFloatData("FlaskPoint", 0);

        ObjetManager.Instance.CosmicGage.Point = GetIntData("CosmicOre", 0);
        ObjetManager.Instance.CosmicGage.CollectedCosmicOre = GetIntData("CosmicOreCount", 0);
        ObjetManager.Instance.CosmicGage.CosmicPoint = GetFloatData("CosmicPoint", 0);

        

        Color baseColor = Color.HSVToRGB(Mathf.Repeat(Flask.Instance.colorValue, 1f), 150 / 255f, 150 / 255f);
        UIManager.Instance.BGMSound.value = GetFloatData("BGMVolume", 0.5f);
        UIManager.Instance.SFXSound.value = GetFloatData("SFXVolume", 0.5f);
        UIManager.Instance.timer = GetFloatData("PlayTime", 0f);
        UIManager.Instance.SkyboxMaterial.SetColor("_Tint", baseColor);

        Spawner.Instance.SpawnedObjectCount = GetIntData("SpawnedObjectCount", 0);
        Spawner.Instance.MergedObjectCount = GetIntData("MergedObjectCount", 0);
        Spawner.Instance.FailCount = GetIntData("FailCount", 0);
        Spawner.Instance.SuccessCount = GetIntData("SuccessCount", 0);

        FeverGage.Instance.FeverStack = GetFloatData("FeverStack", 0f);

        string ObjetList = GetStringData("Objets", "");
        string LevelList = GetStringData("Levels", "");
        string[] array = ObjetList.Split(',');
        Objets = new List<string>(array);
        AddComponentByName(Objets, LevelList);
        StartCoroutine(SaveRountine());
    }

    IEnumerator SaveRountine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            PlayerPrefs.Save();
        }
    }

    public int SpawnedObjectCount = 0;
    public int MergedCount = 0;
    public int DecomposedCount = 0;

    public int FailCount = 0;
    public int SuccessCount = 0;
    public int CosmicOreCount = 0;
    public void AddComponentByName(List<string> componentName, string Levels)
    {
        if (string.IsNullOrEmpty(Levels))
        {
            ObjetManager.Instance.InitializeObjet();
            return;
        }
        ;

        for (int i = 0; i < componentName.Count; i++)
        {
            string name = componentName[i];

            // 인덱스 안전 확인
            if (i >= Levels.Length) break;
            int level = int.Parse(Levels[i].ToString());

            Type typeToAdd = Type.GetType(name);

            if (typeToAdd != null && typeof(Component).IsAssignableFrom(typeToAdd))
            {
                Component comp = ObjetManager.Instance.gameObject.GetComponent(typeToAdd);

                if (comp == null)
                    comp = ObjetManager.Instance.gameObject.AddComponent(typeToAdd);

                Objet targetObjet = comp as Objet;
                if (targetObjet != null)
                {
                    targetObjet.level = level;

                    if (!ObjetManager.Instance.EquipedObjetList.Contains(comp))
                    {
                        ObjetManager.Instance.EquipedObjetList.Add(comp);
                    }
                }
            }
        }
        ObjetManager.Instance.InitializeObjet();
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    public static void ResetData()
    {
        int Resolution = DataManager.GetIntData("Resolution", 2);
        float BGM = DataManager.GetFloatData("BGMVolume", 0.5f);
        float SFX = DataManager.GetFloatData("SFXVolume", 0.5f);
        bool FullScreen = DataManager.GetIntData("FullScreen", 1) == 0 ? false : true;
        PlayerPrefs.DeleteAll();
        DataManager.SetIntData("Resolution", Resolution);
        DataManager.GetFloatData("BGMVolume", BGM);
        DataManager.GetFloatData("SFXVolume", SFX);
        DataManager.SetIntData("FullScreen", FullScreen == true ? 1 : 0);
    }

    public static void SetIntData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetFloatData(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetStringData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static int GetIntData(string key, int defaultValue)
    {
        int loadedValue = PlayerPrefs.GetInt(key, defaultValue);
        return loadedValue;
    }

    public static float GetFloatData(string key, float defaultValue)
    {
        float loadedValue = PlayerPrefs.GetFloat(key, defaultValue);
        return loadedValue;
    }

    public static string GetStringData(string key, string defaultValue)
    {
        string loadedValue = PlayerPrefs.GetString(key, defaultValue);
        return loadedValue;
    }
}
