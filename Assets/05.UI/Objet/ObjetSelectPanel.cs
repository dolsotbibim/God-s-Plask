using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjetSelectPanel : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Desc;
    public Image Image;
    public int index;
    public static int FocusedPanel = -1;
    bool isFocused = false;
    public Objet Objet;
    public bool IsFocused
    {
        get { return isFocused; }
        set
        {
            if (FocusedPanel == index)
            {
                Objet.OnEquip();
                ObjetManager.Instance.SelectEnd(Objet);
            }
            FocusedPanel = index;
        }
    }

    private void Update()
    {
        if (FocusedPanel == index)
        {
            transform.localScale = Vector3.one * 1.1f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
    private void OnDisable()
    {
        FocusedPanel = -1;
        transform.localScale = Vector3.one;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }
    public void SetPanel(Objet objet)
    {
        if (objet == null) return;
        Objet = objet;
        Name.text = objet.Name;
        Desc.text = objet.Description;
        Image.sprite = objet.icon;
    }
}
