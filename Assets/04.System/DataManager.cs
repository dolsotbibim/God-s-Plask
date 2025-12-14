using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    List<string> Objets = new List<string>();
    private void Start()
    {
        ResetData();

        Plask.Instance.Level = GetIntData("PlaskLevel", 1);
        UpgradeManager.Instance.PlaskGage.PlaskPoint = GetFloatData("PlaskPoint", 0);

        ObjetManager.Instance.CosmicGage.CosmicPoint = GetFloatData("CosmicPoint", 0);
        string ObjetList = GetStringData("Objets", "");
        string[] array = ObjetList.Split(',');

        Objets = new List<string>(array);
        AddComponentByName(Objets);

    }

    public void AddComponentByName(List<string> componentName)
    {
        Type typeToAdd;
        Component newComponent;
        foreach (string name in componentName)
        {
            typeToAdd = Type.GetType(name);

            if (typeToAdd != null && typeToAdd.IsSubclassOf(typeof(Component)))
            {
                newComponent = ObjetManager.Instance.gameObject.AddComponent(typeToAdd);

                ObjetManager.Instance.EquipedObjetList.Add(newComponent);
            }
        }
        ObjetManager.Instance.InitializeObjet();
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void SetIntData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static void SetFloatData(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static void SetStringData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
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
