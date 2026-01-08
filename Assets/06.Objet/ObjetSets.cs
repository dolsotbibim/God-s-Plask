using UnityEngine;
using System.Linq;
public class Meteor : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Spawn";
        Name = "Meteor";
        Description = "+1 pulse generation";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Syringe : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.DefaultRoundTripTime *= 0.9f;
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Spawn";
        Name = "Syringe";
        Description = "+10% hand speed\n+1 pulse generation";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Magnifier : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.SuccessRange *= 1.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 1;
        Type = "Spawn";
        Name = "Magnifier";
        Description = "+10% pulse range";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Dipole : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.EventMultiplyChance += 0.1f;
        Spawner.Instance.SpawnPanel.EventMultiplier = 2;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Spawn";
        Name = "Dipole";
        Description = "+10% double pulse chance";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Supernova : Objet
{
    public override void OnEquip() { }

    public override void Init()
    {
        level = 0;
        maxLevel = 1;
        Cost = 10;
        Type = "Spawn";
        Name = "Supernova";
        Description = "Most brightest pulse range becomes more powerful";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Arrow : Objet
{
    public override void OnEquip() { }

    public override void Init()
    {
        level = 0;
        maxLevel = 1;
        Cost = 5;
        Type = "Spawn";
        Name = "Arrow";
        Description = "Most longest pulse range becomes more powerful";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Spear : Objet
{
    public override void OnEquip() { }

    public override void Init()
    {
        level = 0;
        maxLevel = 1;
        Cost = 7;
        Type = "Spawn";
        Name = "Spear";
        Description = "Most Shortest pulse range becomes more powerful";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class StarCandy : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.BonusSpawnChance += 0.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Spawn";
        Name = "Star Candy";
        Description = "+10% chance of a bonus spawn";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Anvil : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.EnhancedSpawnChance += 0.1f;
        Cost += level + 2;
                Spawner.Instance.SetSpawnLevel();
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 1;
        Type = "Spawn";
        Name = "Anvil";
        Description = "+10% chance of an enhanced spawn";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class WaveAmplifier : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.MaxNesting += 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 3;
        Cost = 3;
        Type = "Spawn";
        Name = "Wave Amplifier";
        Description = "+1 max pulse nesting";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class MagneticField : Objet
{
    public override void OnEquip()
    {
        Object.StarPointMultiplier *= 1.1f;
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Cost";
        Name = "Magnetic Field";
        Description = "+10% star point gain\n+1 pulse generation";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Feathers : Objet
{
    public override void OnEquip()
    {
        Object.StarPointMultiplier *= 1.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 1;
        Type = "Cost";
        Name = "Feathers";
        Description = "+10% star point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class HolyGrail : Objet
{
    public override void OnEquip()
    {
        Object.CosmicPointMultiplier *= 1.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 1;
        Type = "Cost";
        Name = "Holy Grail";
        Description = "+10% cosmic point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class HyperCube : Objet
{
    public override void OnEquip()
    {
        Object.CosmicPointMultiplier *= 1.1f;
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Cost";
        Name = "Hyper Cube";
        Description = "+10% cosmic point gain\n+1 pulse generation";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class DarkEye : Objet
{
    public override void OnEquip()
    {
        Object.CosmicPointMultiplier *= 1.25f;
        Object.StarPointMultiplier *= 0.9f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Cost";
        Name = "Dark Eye";
        Description = "+25% cosmic point gain\n-10% star point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Crown : Objet
{
    public override void OnEquip()
    {
        Object.CosmicPointMultiplier *= 0.9f;
        Object.StarPointMultiplier *= 1.25f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Cost";
        Name = "Crown";
        Description = "+25% star point gain\n-10% cosmic point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Clover : Objet
{
    public override void OnEquip()
    {
        ObjetManager.Instance.CosmicGage.DoubleGetChance += 0.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 1;
        Type = "Cost";
        Name = "Clover";
        Description = "10% chance for bonus cosmic ore";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class CampFire : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.FeverBonus += 0.5f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Fever";
        Name = "Camp Fire";
        Description = "+0.5 fever spawn multiplier";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Thermometer : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.MaxFeverStack -= 1;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 2;
        Type = "Fever";
        Name = "Thermometer";
        Description = "-10% fever gauge cost";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Plasma : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.FeverRambda += 1f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Fever";
        Name = "Plasma";
        Description = "+1 fever pulse multiplier";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class HeatEngine : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.FiverRetriggerChance += 0.1f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 5;
        Cost = 3;
        Type = "Fever";
        Name = "Heat Engine";
        Description = "+10% fever retrigger chance";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class SteamEngine : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.FeverRambda += 1.5f;
        FeverGage.Instance.FeverSpeed += 1;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0;
        maxLevel = 1;
        Cost = 7;
        Type = "Fever";
        Name = "Steam Engine";
        Description = "+1.5 fever pulse multiplier\nDouble fever hand move speed";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Overclock : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.DefaultRoundTripTime *= 0.8f;
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0; maxLevel = 1; Cost = 2;
        Type = "Spawn"; Name = "Overclock";
        Description = "+20% hand speed\n+1 pulse generation";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Charger : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.DefaultRoundTripTime *= 0.85f;
        FeverGage.Instance.feverBonus += 0.5f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Fever"; Name = "Charger";
        Description = "+15% hand speed\n+0.5 fever spawn multiplier";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class MidasTouch : Objet
{
    public override void OnEquip()
    {
        Object.StarPointMultiplier *= 1.15f;
        Spawner.Instance.EnhancedSpawnChance += 0.15f;
                Spawner.Instance.SetSpawnLevel();
    }

    public override void Init()
    {
        level = 0; maxLevel = 1; Cost = 5;
        Type = "Cost"; Name = "Midas Touch";
        Description = "+15% star point gain\n+15% enhanced spawn chance";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class SlotMachine : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.BonusSpawnChance += 0.2f;
        Spawner.Instance.SpawnPanel.SuccessRange *= 0.9f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 2;
        Type = "Spawn"; Name = "Slot Machine";
        Description = "+20% bonus spawn chance\n-10% pulse range";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Pickaxe : Objet
{
    public override void OnEquip()
    {
        Object.StarPointMultiplier *= 1.15f;
        Object.CosmicPointMultiplier *= 1.15f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Cost"; Name = "Pickaxe";
        Description = "+15% star, cosmic point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Reactor : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.MaxNesting += 1;
        FeverGage.Instance.MaxFeverStack += 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 2; Cost = 3;
        Type = "Spawn"; Name = "Reactor";
        Description = "+1 max pulse nesting\n+10% fever gauge cost";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class CosmicCollector : Objet
{
    public override void OnEquip()
    {
        Object.CosmicPointMultiplier *= 1.10f;
        Spawner.Instance.SpawnPanel.SuccessRange *= 1.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Cost"; Name = "Cosmic Collector";
        Description = "+10% pulse range\n+10% cosmic point gain";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class HeatSink : Objet
{
    public override void OnEquip()
    {
        FeverGage.Instance.FiverRetriggerChance += 0.07f;
        Spawner.Instance.DefaultRoundTripTime *= 0.9f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Fever"; Name = "Heat Sink";
        Description = "+7% fever retrigger chance\n+10% hand speed";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Prism : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.SuccessRange *= 1.2f;
        Spawner.Instance.SpawnPanel.EventMultiplyChance += 0.05f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 4;
        Type = "Spawn"; Name = "Prism";
        Description = "+20% pulse range\n+5% double pulse chance";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Cash : Objet
{
    public override void OnEquip()
    {
        Object.StarPointMultiplier *= 1.25f;
        Object.CosmicPointMultiplier *= 1.25f;
        Spawner.Instance.DefaultRoundTripTime *= 0.75f;
        Cost += level + 8;
    }

    public override void Init()
    {
        level = 0; maxLevel = 1; Cost = 7;
        Type = "Cost"; Name = "Cash";
        Description = "+25% all points gain\n+25% hand speed";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Battery : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.Rambda += 2;
        Spawner.Instance.SpawnPanel.SuccessRange *= 0.85f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Spawn"; Name = "Battery";
        Description = "+2 pulse generation\n-15% pulse range";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class ChainReaction : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Spawner.Instance.SpawnPanel.EventMultiplyChance += 0.05f;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Spawn"; Name = "Chain Reaction";
        Description = "+1 pulse generation\n+5% double pulse chance";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Timer : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.Rambda += 1;
        Spawner.Instance.DefaultRoundTripTime *= 1.1f;
        Cost += level + 1;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Spawn"; Name = "Timer";
        Description = "+1 pulse generation\n-10% hand speed";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}

public class Nuclear : Objet
{
    public override void OnEquip()
    {
        Spawner.Instance.SpawnPanel.Rambda += 1;
        FeverGage.Instance.MaxFeverStack -= 1;
        Cost += level + 2;
    }

    public override void Init()
    {
        level = 0; maxLevel = 5; Cost = 3;
        Type = "Spawn"; Name = "Nuclear";
        Description = "+1 pulse generation\n-10% fever gauge cost";
        icon = Resources.LoadAll<Sprite>("Objet/" + Name.Replace(" ", "")).FirstOrDefault(s => s.name == "0");
    }
}