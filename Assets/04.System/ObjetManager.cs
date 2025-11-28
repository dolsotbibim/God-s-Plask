using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
public class ObjetManager : MonoBehaviour
{
    public CosmicGage CosmicGage;

    public static ObjetManager Instance;

    public int RequiredCosmicPoint = 100;
    public bool IsObjetWindowEnabled = false;
    public ObservableCollection<Component> EquipedObjetList = new ObservableCollection<Component>();
    public List<Component> TempEquipedObjetList = new List<Component>();
    public List<Objet> ObjetList = new List<Objet>();
    public ObjetSelectPanel[] ObjetSelectPanels;
    public GameObject ObjetSelectWindow;

    public Dictionary<int, float> rarityWeights = new Dictionary<int, float>
    {
        { 0, 1f },
        { 1, 3f },
        { 2, 10f },
        { 3, 49f },
        { 4, 37f },
    };

    private Objet GetRandomObjet()
    {
        TempEquipedObjetList = EquipedObjetList.ToList<Component>();
        if (ObjetList == null || ObjetList.Count == 0) return null;
        var availableAugments = ObjetList
            .Where(a => !TempEquipedObjetList.Contains(a) && a.GetComponent<Objet>().Tier == GetTargetRarity())
            .ToList();
        if (availableAugments.Count == 0) return null;

        int index = Random.Range(0, availableAugments.Count);
        Objet chosenAugment = (Objet)availableAugments[index];
        TempEquipedObjetList.Add(chosenAugment);
        return chosenAugment;

        int GetTargetRarity()
        {
            float chance = Random.value * 100f;
            if (chance < rarityWeights[0])
                return 0;
            chance -= rarityWeights[0];
            if (chance < rarityWeights[1])
                return 1;
            chance -= rarityWeights[1];
            if (chance < rarityWeights[2])
                return 2;
            chance -= rarityWeights[2];
            if (chance < rarityWeights[3])
                return 3;
            chance -= rarityWeights[3];
            if (chance < rarityWeights[4])
                return 4;
            return 5;
        }
}
    private void Awake()
    {
        Instance = this;
        EquipedObjetList.CollectionChanged += OnListChanged;

        SetObjetList();
    }

    void OnListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        List<string> componentNames = new List<string>();
        foreach (Component objet in EquipedObjetList)
        {
            componentNames.Add(objet.GetType().Name);
        }

        string data = string.Join(",", componentNames);
        DataManager.SetStringData("Objets", data);
    }

    public void InitializeObjet()
    {
        foreach (Objet objet in EquipedObjetList)
        {
            objet.OnEquip();
        }
    }

    public void GetObjet()
    {
        IsObjetWindowEnabled = true;
        ObjetSelectWindow.SetActive(true);
        foreach (ObjetSelectPanel panel in ObjetSelectPanels) 
        {
            panel.SetPanel(GetRandomObjet());
        }
    }

    public void SelectEnd(Objet objet)
    {
        EquipedObjetList.Add(objet);
        ObjetSelectWindow.SetActive(false);
        IsObjetWindowEnabled = false;
    }

private void SetObjetList()
    {
        ObjetList.Add(gameObject.AddComponent<TestObjet>());
        ObjetList.Add(gameObject.AddComponent<TestObjet2>());
        ObjetList.Add(gameObject.AddComponent<TestObjet3>());
        ObjetList.Add(gameObject.AddComponent<TestObjet4>());
        ObjetList.Add(gameObject.AddComponent<TestObjet5>());
    }
}
