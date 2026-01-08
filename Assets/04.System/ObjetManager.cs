using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;
public class ObjetManager : MonoBehaviour
{
    public CosmicGage CosmicGage;

    public static ObjetManager Instance;

    public float RequiredCosmicPoint = 10;
    public ObservableCollection<Component> EquipedObjetList = new ObservableCollection<Component>();
    public List<Component> TempEquipedObjetList = new List<Component>();
    public List<Objet> ObjetList = new List<Objet>();

    private Objet LastSelectedObjet;

    public Image SelectButtonImage;

    public Image OldObjetSlot;
    public Image NewObjet1Slot;
    public Image NewObjet2Slot;

    public Image OldObjet;
    public Image NewObjet1;
    public Image NewObjet2;

    public Image Line1;
    public Image Line2;

    public Material[] Mats;

    public List<(string, Color, Color)> ColorSet = new List<(string, Color, Color)>
    {
        ("Fever", Color.red, new Color(255 / 255f, 146 / 255f, 146 / 255f)),
        ("Cost", Color.green, new Color(147 / 255f, 255 / 255f, 147 / 255f)),
        ("Spawn", new Color(0f, 0.3f, 1f), new Color(75 / 255f, 75 / 255f, 255 / 255f)),
        ("3", Color.yellow, new Color(255 / 255f, 255 / 255f, 255 / 255f)),
    };

    private (Objet, Objet) GetRandomObjet()
    {
        bool isLastSelectedNull = LastSelectedObjet == null;

        List<Objet> availableSameType = ObjetList
            .Where(a =>
                (a.level < a.maxLevel) &&
                (isLastSelectedNull || a.Type == LastSelectedObjet.Type)
            )
            .ToList();

        List<Objet> availableDifferentType = new List<Objet>();
        if (!isLastSelectedNull)
        {
            availableDifferentType = ObjetList
                .Where(a =>
                    (a.level < a.maxLevel) &&
                    (a.Type != LastSelectedObjet.Type)
                )
                .ToList();
        }

        if (availableSameType.Count == 0)
        {
            if (availableDifferentType.Count >= 2)
            {
                int index1 = Random.Range(0, availableDifferentType.Count);
                Objet objet1 = availableDifferentType[index1];

                availableDifferentType.RemoveAt(index1);

                int index2 = Random.Range(0, availableDifferentType.Count);
                Objet objet2 = availableDifferentType[index2];

                return (objet1, objet2);
            }
            else if (availableDifferentType.Count == 1)
            {
                return (availableDifferentType[0], availableDifferentType[0]);
            }
            else
            {
                return (null, null);
            }
        }

        if (availableDifferentType.Count == 0)
        {
            if (availableSameType.Count >= 2)
            {
                int index1 = Random.Range(0, availableSameType.Count);
                Objet objet1 = availableSameType[index1];

                availableSameType.RemoveAt(index1);

                int index2 = Random.Range(0, availableSameType.Count);
                Objet objet2 = availableSameType[index2];

                if (UIManager.Instance.IsTutorial) objet1 = GetComponent<Magnifier>();
                return (objet1, objet2);
            }
            else if (availableSameType.Count == 1)
            {
                return (availableSameType[0], availableSameType[0]);
            }
        }

        int sameIndex = Random.Range(0, availableSameType.Count);
        int diffIndex = Random.Range(0, availableDifferentType.Count);

        return (availableSameType[sameIndex], availableDifferentType[diffIndex]);
    }
    private void Awake()
    {
        Instance = this;
        EquipedObjetList.CollectionChanged += OnListChanged;

        SetObjetList();
        
    }

    public void InitialSetting()
    {
        if (DataManager.GetStringData("Objet1", "") != "")
            LoadNextObjet(DataManager.GetStringData("Objet1", ""), DataManager.GetStringData("Objet2", ""));
        else
            GetObjet();
    }
    public void LoadNextObjet(string componentName, string componentName2)
    {
        Type typeToAdd;
        Component newComponent = null;
        Component newComponent2 = null;
        typeToAdd = Type.GetType(componentName);
        if (typeToAdd != null)
            newComponent = ObjetManager.Instance.GetComponent(typeToAdd);

        typeToAdd = Type.GetType(componentName2);
        if (typeToAdd != null)
            newComponent2 = ObjetManager.Instance.GetComponent(typeToAdd);
        Objet[] allObjects = ObjetManager.Instance.GetComponentsInChildren<Objet>(true);
        Objet objet1 = null;
        Objet objet2 = null;
        foreach (Objet obj in allObjects)
        {
            if (obj == newComponent)
            {
                objet1 = obj;

            }
            if (obj == newComponent2)
            {
                objet2 = obj;
            }
        }
        SetObjet((objet1, objet2));
    }

    private void Update()
    {
        if (int.Parse(Cost.text) > CosmicGage.Point)
            SelectButtonImage.material.SetColor("_Color", new Color(138 / 255f, 39 / 255f, 191 / 255f) * 1);
        else
            SelectButtonImage.material.SetColor("_Color", new Color(138 / 255f, 39 / 255f, 191 / 255f) * 3);

    }

    void OnListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        List<string> componentNames = new List<string>();
        foreach (Component objet in EquipedObjetList)
        {
            componentNames.Add(objet.GetType().Name);
        }
        string Levels = "";
        foreach (Objet objet in EquipedObjetList)
        {
            Levels += objet.level.ToString();
        }
        DataManager.SetStringData("Levels", Levels);
        string data = string.Join(",", componentNames);
        DataManager.SetStringData("Objets", data);
        UIManager.Instance.SetInfoUI();
    }

    public void InitializeObjet()
    {
        foreach (Objet objet in EquipedObjetList)
        {
            for(int i = 0; i < objet.level; i++)
            {
                objet.OnEquip();
            }
        }
        
        UIManager.Instance.SetInfoUI();
        InitialSetting();
    }
    public (Objet Item1, Objet Item2) objets;
    public void GetObjet()
    {
        (string, Color, Color) GetColorsByType(string type)
        {
            var defaultTuple = ("NotFound", Color.white, Color.white);
            return ColorSet.FirstOrDefault(tuple => tuple.Item1 == type) != default
                   ? ColorSet.FirstOrDefault(tuple => tuple.Item1 == type)
                   : defaultTuple;
        }

        Material GetImageMaterialInstance(Image imageComponent)
        {
            if (imageComponent == null) return null;
            return imageComponent.materialForRendering;
        }

        void SetImageColors(Image targetImage, Sprite icon, (string, Color, Color) colors)
        {
            if (targetImage == null) return;

            targetImage.sprite = icon;

            Material mat = GetImageMaterialInstance(targetImage);
            if (mat != null)
            {
                mat.SetColor("_Color", colors.Item3);
                mat.SetColor("_Color2", colors.Item2);
            }
        }

        void SetMaterialColors(Material mat, Color color1, Color color2)
        {
            if (mat == null) return;
            mat.SetColor("_Color", color1);
            mat.SetColor("_Color2", color2);
        }

        objets = GetRandomObjet();
        NewObjet1Slot.material = new Material(Mats[objets.Item1.level]);
        NewObjet2Slot.material = new Material(Mats[objets.Item2.level]);
        OldObjetSlot.material = new Material(Mats[LastSelectedObjet != null ? LastSelectedObjet.level - 1 : 0]);
        var colors1 = GetColorsByType(objets.Item1.Type);
        var colors2 = GetColorsByType(objets.Item2.Type);

        SetImageColors(NewObjet1, objets.Item1.icon, colors1);
        SetImageColors(NewObjet2, objets.Item2.icon, colors2);
        SetImageColors(NewObjet1Slot, null, colors1);
        SetImageColors(NewObjet2Slot, null, colors2);

        Material matLine1 = GetImageMaterialInstance(Line1);
        Material matLine2 = GetImageMaterialInstance(Line2);

        if (LastSelectedObjet != null)
        {
            var colorsOld = GetColorsByType(LastSelectedObjet.Type);

            SetImageColors(OldObjet, LastSelectedObjet.icon, colorsOld);
            SetImageColors(OldObjetSlot, null, colorsOld);

            SetMaterialColors(matLine1, colorsOld.Item2, colors1.Item2);
            SetMaterialColors(matLine2, colorsOld.Item2, colors2.Item2);
        }
        else
        {
            Color white = Color.white;

            SetMaterialColors(GetImageMaterialInstance(OldObjetSlot), white, white);

            SetMaterialColors(matLine1, white, colors1.Item2);
            SetMaterialColors(matLine2, white, colors2.Item2);
        }
        DataManager.SetStringData("Objet1", objets.Item1.GetType().Name);
        DataManager.SetStringData("Objet2", objets.Item2.GetType().Name);

        OnSelcetSlot(1);
    }

    public void SetObjet((Objet, Objet) objets)
    {

        if (objets.Item1 == null || objets.Item2 == null)
        {
            return;
        }
        (string, Color, Color) GetColorsByType(string type)
        {
            var defaultTuple = ("NotFound", Color.white, Color.white);
            return ColorSet.FirstOrDefault(tuple => tuple.Item1 == type) != default
                   ? ColorSet.FirstOrDefault(tuple => tuple.Item1 == type)
                   : defaultTuple;
        }

        Material GetImageMaterialInstance(Image imageComponent)
        {
            if (imageComponent == null) return null;
            return imageComponent.materialForRendering;
        }

        void SetImageColors(Image targetImage, Sprite icon, (string, Color, Color) colors)
        {
            if (targetImage == null) return;

            targetImage.sprite = icon;

            Material mat = GetImageMaterialInstance(targetImage);
            if (mat != null)
            {
                mat.SetColor("_Color", colors.Item3);
                mat.SetColor("_Color2", colors.Item2);
            }
        }

        void SetMaterialColors(Material mat, Color color1, Color color2)
        {
            if (mat == null) return;
            mat.SetColor("_Color", color1);
            mat.SetColor("_Color2", color2);
        }
        this.objets = objets;
        NewObjet1Slot.material = new Material(Mats[objets.Item1.level]);
        NewObjet2Slot.material = new Material(Mats[objets.Item2.level]);
        OldObjetSlot.material = new Material(Mats[LastSelectedObjet != null ? LastSelectedObjet.level - 1 : 0]);
        var colors1 = GetColorsByType(objets.Item1.Type);
        var colors2 = GetColorsByType(objets.Item2.Type);

        SetImageColors(NewObjet1, objets.Item1.icon, colors1);
        SetImageColors(NewObjet2, objets.Item2.icon, colors2);
        SetImageColors(NewObjet1Slot, null, colors1);
        SetImageColors(NewObjet2Slot, null, colors2);

        Material matLine1 = GetImageMaterialInstance(Line1);
        Material matLine2 = GetImageMaterialInstance(Line2);

        if (LastSelectedObjet != null)
        {
            var colorsOld = GetColorsByType(LastSelectedObjet.Type);

            SetImageColors(OldObjet, LastSelectedObjet.icon, colorsOld);
            SetImageColors(OldObjetSlot, null, colorsOld);

            SetMaterialColors(matLine1, colorsOld.Item2, colors1.Item2);
            SetMaterialColors(matLine2, colorsOld.Item2, colors2.Item2);
        }
        else
        {
            Color white = Color.white;

            SetMaterialColors(GetImageMaterialInstance(OldObjetSlot), white, white);

            SetMaterialColors(matLine1, white, colors1.Item2);
            SetMaterialColors(matLine2, white, colors2.Item2);
        }
        OnSelcetSlot(1);
    }
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Desc;
    public TextMeshProUGUI Cost;
    public Button SelectButton;
    int selectedIndex = 1;
    public void OnSelcetSlot(int i)
    {
        if (i == 1)
        {
            Line2.material.SetFloat("_Float", 0.5f);
            Line1.material.SetFloat("_Float", 0.5f);
            Name.text = objets.Item1.Name;
            Desc.text = objets.Item1.Description;
            Cost.text = objets.Item1.Cost.ToString();
            NewObjet1Slot.transform.localScale = Vector3.one * 1.1f;
            NewObjet1Slot.material.SetFloat("_Brightness", 1.5f);
            NewObjet2Slot.transform.localScale = Vector3.one;
            NewObjet2Slot.material.SetFloat("_Brightness", 1.25f);

            selectedIndex = 1;
        }
        else if (i == 2)
        {
            Line2.material.SetFloat("_Float", 0.5f);
            Line1.material.SetFloat("_Float", 0.5f);
            Name.text = objets.Item2.Name;
            Desc.text = objets.Item2.Description;
            Cost.text = objets.Item2.Cost.ToString();
            NewObjet2Slot.transform.localScale = Vector3.one * 1.1f;
            NewObjet2Slot.material.SetFloat("_Brightness", 1.5f);
            NewObjet1Slot.transform.localScale = Vector3.one;
            NewObjet1Slot.material.SetFloat("_Brightness", 1.25f);


            selectedIndex = 2;
        }
    }
    public RectTransform CosmicOre;
    public void SelectEnd()
    {
        if(int.Parse(Cost.text) > CosmicGage.Point)
        {
            return;
        }
        SelectButton.interactable = false;
        if (selectedIndex == 1)
        {
            objets.Item1.OnEquip();
            objets.Item1.level += 1;
            LastSelectedObjet = objets.Item1;
            if(!EquipedObjetList.Contains(objets.Item1))
            {
                EquipedObjetList.Add(objets.Item1);
            }
        }
        else if(selectedIndex == 2)
        {
            objets.Item2.OnEquip();
            objets.Item2.level += 1;
            LastSelectedObjet = objets.Item2;
            if (!EquipedObjetList.Contains(objets.Item2))
            {
                EquipedObjetList.Add(objets.Item2);
            }
        }
        List<string> componentNames = new List<string>();
        foreach (Component objet in EquipedObjetList)
        {
            componentNames.Add(objet.GetType().Name);
        }
        string Levels = "";
        foreach (Objet objet in EquipedObjetList)
        {
            Levels += objet.level.ToString();
        }
        DataManager.SetStringData("Levels", Levels);
        string data = string.Join(",", componentNames);
        DataManager.SetStringData("Objets", data);
        UIManager.Instance.SetInfoUI();

        CosmicGage.Point -= int.Parse(Cost.text);
        StartCoroutine(SelcetEffect(selectedIndex));
    }

    IEnumerator SelcetEffect(int i)
    {
        Image image = i == 1 ? NewObjet1 : NewObjet2;
        Image Slot = i == 1 ? NewObjet1Slot : NewObjet2Slot;
        Image Line = i == 1 ? Line1 : Line2;
        UpgradeEffectPS ps = Spawner.Instance.PoolManager.GetFromPool<UpgradeEffectPS>();
        ps.SetTarget(selectedIndex + 1, CosmicOre.GetComponent<RectTransform>().position, 0);
        for(int j = 0 ; j < 30; j++)
        {
            ps.Emit(1);
            yield return new WaitForSeconds(0.01f);
        }
        //yield return Line.materialForRendering.DOFloat(0, "_Cut", 1);
        SoundManager.Instance.PlaySFX(7, 0.2f, false);
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.PlaySFX(8, 3f, false);

        Slot.materialForRendering.DOFloat(5, "_Brightness", 0.5f);
        image.materialForRendering.DOFloat(30, "_Brightness", 0.5f);
        yield return new WaitForSeconds(0.2f);

        NewObjet1.DOFade(0, 1);
        NewObjet2.DOFade(0, 1);
        NewObjet1Slot.DOFade(0, 1);
        NewObjet2Slot.DOFade(0, 1);
        /*Line1.DOFade(0, 1);
        Line2.DOFade(0, 1);
        OldObjet.DOFade(0, 1);
        OldObjetSlot.DOFade(0, 1);*/
        yield return new WaitForSeconds(1f);
        /*Line1.materialForRendering.SetFloat("_Cut", -1);
        Line2.materialForRendering.SetFloat("_Cut", -1);*/
        Slot.materialForRendering.SetFloat("_Brightness", 1.25f);
        image.materialForRendering.SetFloat("_Brightness", 1.25f);
        GetObjet();
        SelectButton.interactable = true;

        NewObjet1.DOFade(1, 1);
        NewObjet2.DOFade(1, 1);
        NewObjet1Slot.DOFade(1, 1);
        NewObjet2Slot.DOFade(1, 1);
        /*Line1.DOFade(1, 1);
        Line2.DOFade(1, 1);
        OldObjet.DOFade(1, 1);
        OldObjetSlot.DOFade(1, 1);*/
    }

    private void SetObjetList()
    {
        ObjetList.Add(gameObject.AddComponent<Anvil>());
        ObjetList.Add(gameObject.AddComponent<Arrow>());
        ObjetList.Add(gameObject.AddComponent<Battery>());
        ObjetList.Add(gameObject.AddComponent<CampFire>());
        ObjetList.Add(gameObject.AddComponent<Cash>());
        ObjetList.Add(gameObject.AddComponent<ChainReaction>());
        ObjetList.Add(gameObject.AddComponent<Charger>());
        ObjetList.Add(gameObject.AddComponent<Clover>());
        ObjetList.Add(gameObject.AddComponent<CosmicCollector>());
        ObjetList.Add(gameObject.AddComponent<Crown>());
        ObjetList.Add(gameObject.AddComponent<DarkEye>());
        ObjetList.Add(gameObject.AddComponent<Dipole>());
        ObjetList.Add(gameObject.AddComponent<Feathers>());
        ObjetList.Add(gameObject.AddComponent<HeatEngine>());
        ObjetList.Add(gameObject.AddComponent<HeatSink>());
        ObjetList.Add(gameObject.AddComponent<HolyGrail>());
        ObjetList.Add(gameObject.AddComponent<HyperCube>());
        ObjetList.Add(gameObject.AddComponent<MagneticField>());
        ObjetList.Add(gameObject.AddComponent<Magnifier>());
        ObjetList.Add(gameObject.AddComponent<Meteor>());
        ObjetList.Add(gameObject.AddComponent<MidasTouch>());
        ObjetList.Add(gameObject.AddComponent<Nuclear>());
        ObjetList.Add(gameObject.AddComponent<Overclock>());
        ObjetList.Add(gameObject.AddComponent<Pickaxe>());
        ObjetList.Add(gameObject.AddComponent<Plasma>());
        ObjetList.Add(gameObject.AddComponent<Prism>());
        ObjetList.Add(gameObject.AddComponent<Reactor>());
        ObjetList.Add(gameObject.AddComponent<SlotMachine>());
        ObjetList.Add(gameObject.AddComponent<Spear>());
        ObjetList.Add(gameObject.AddComponent<StarCandy>());
        ObjetList.Add(gameObject.AddComponent<SteamEngine>());
        ObjetList.Add(gameObject.AddComponent<Supernova>());
        ObjetList.Add(gameObject.AddComponent<Syringe>());
        ObjetList.Add(gameObject.AddComponent<Thermometer>());
        ObjetList.Add(gameObject.AddComponent<Timer>());
        ObjetList.Add(gameObject.AddComponent<WaveAmplifier>());
    }

    public bool CheckObjetEquiped<T>() where T : Objet
    {
        foreach (Objet objet in EquipedObjetList)
        {
            if (objet is T)
            {
                return true;
            }
        }
        return false;
    }
}
