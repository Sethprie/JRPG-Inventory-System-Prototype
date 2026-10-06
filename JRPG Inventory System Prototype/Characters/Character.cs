using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype;

public class Character
{
    public string Name { get; }
    public Dictionary<Stat, int> BaseStats { get; }
    public Dictionary<EquipSlot, EquipmentItem> Equipped { get; } = new();
    public int CurrentHp { get; private set; }

    public Character(string name, int maxHp, int maxMp, int attack, int defense, int speed)
    {
        Name = name;
        BaseStats = new()
        {
            [Stat.MaxHp] = maxHp,
            [Stat.MaxMp] = maxMp,
            [Stat.Attack] = attack,
            [Stat.Defense] = defense,
            [Stat.Speed] = speed
        };
        CurrentHp = maxHp;
    }

    public int GetStat(Stat stat)
    {
        var mods = Equipped.Values.SelectMany(e => e.Modifiers).Where(m => m.Stat == stat).ToList();
        float flat = BaseStats[stat] + mods.Where(m => m.Type == ModifierType.Flat).Sum(m => m.Value);
        float pct = mods.Where(m => m.Type == ModifierType.Percent).Sum(m => m.Value);
        return (int)(flat * (1 + pct));
    }

    // Devuelve el item que estaba equipado antes en ese slot (o null)
    public EquipmentItem? Equip(EquipmentItem item)
    {
        Equipped.TryGetValue(item.Slot, out var previous);
        Equipped[item.Slot] = item;
        ClampHp();
        return previous;
    }

    public EquipmentItem? Unequip(EquipSlot slot)
    {
        if (!Equipped.Remove(slot, out var item)) return null;
        ClampHp();
        return item;
    }

    public int Heal(int amount)
    {
        int before = CurrentHp;
        CurrentHp = Math.Min(CurrentHp + amount, GetStat(Stat.MaxHp));
        return CurrentHp - before;
    }

    public void SetHp(int hp) => CurrentHp = Math.Clamp(hp, 0, GetStat(Stat.MaxHp));

    private void ClampHp() => CurrentHp = Math.Min(CurrentHp, GetStat(Stat.MaxHp));

    public Dictionary<StatusType, int> Statuses { get; } = new(); // estado -> turnos

    public void AddStatus(StatusType status, int turns) => Statuses[status] = turns;
    public bool RemoveStatus(StatusType status) => Statuses.Remove(status);

    public void TakeDamage(int amount) => CurrentHp = Math.Max(0, CurrentHp - amount);
}