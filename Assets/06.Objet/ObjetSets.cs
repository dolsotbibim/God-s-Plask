using UnityEditor.Rendering;
using UnityEngine;
public class TestObjet : Objet
{
    public override void OnEquip()
    {
    }

    public override void Init()
    {
        Tier = 4;
        Name = "Test";
        Description = "Test";
        icon = Resources.Load<Sprite>("Objet/" + Name);
    }
}

public class TestObjet2 : Objet
{
    public override void OnEquip()
    {
    }

    public override void Init()
    {
        Tier = 4;
        Name = "Test2";
        Description = "Test";
        icon = Resources.Load<Sprite>("Objet/" + Name);
    }
}

public class TestObjet3 : Objet
{
    public override void OnEquip()
    {
    }

    public override void Init()
    {
        Tier = 3;
        Name = "Test3";
        Description = "Test";
        icon = Resources.Load<Sprite>("Objet/" + Name);
    }
}

public class TestObjet4 : Objet
{
    public override void OnEquip()
    {
    }

    public override void Init()
    {
        Tier = 5;
        Name = "Test4";
        Description = "Test";
        icon = Resources.Load<Sprite>("Objet/" + Name);
    }
}

public class TestObjet5 : Objet
{
    public override void OnEquip()
    {
    }

    public override void Init()
    {
        Tier = 4;
        Name = "Test5";
        Description = "Test";
        icon = Resources.Load<Sprite>("Objet/" + Name);
    }
}
