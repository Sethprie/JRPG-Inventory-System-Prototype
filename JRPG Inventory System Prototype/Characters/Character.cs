namespace JRPG_Inventory_System_Prototype;

public class Character
{
    public const int DefaultSolidity = 3;

    public string Name { get; }
    public CharacterClass Class { get; }
    public Dictionary<Stat, int> BaseStats { get; }
    public Dictionary<EquipSlot, EquipmentItem> Equipped { get; } = new();
    public Dictionary<StatusType, int> Statuses { get; } = new(); // estado -> turnos
    public int CurrentHp { get; private set; }
    public int CurrentSolidity { get; private set; }

    public Character(string name, CharacterClass characterClass,
                     int maxHp, int maxMp, int attack, int defense, int speed,
                     int maxSolidity = DefaultSolidity)
    {
        Name = name;
        Class = characterClass;
        BaseStats = new()
        {
            [Stat.MaxHp] = maxHp,
            [Stat.MaxMp] = maxMp,
            [Stat.Attack] = attack,
            [Stat.Defense] = defense,
            [Stat.Speed] = speed,
            [Stat.MaxSolidity] = maxSolidity
        };
        CurrentHp = maxHp;
        CurrentSolidity = maxSolidity;
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
        ClampVitals();
        return previous;
    }

    public EquipmentItem? Unequip(EquipSlot slot)
    {
        if (!Equipped.Remove(slot, out var item)) return null;
        ClampVitals();
        return item;
    }

    // ---------- PV ----------

    public int Heal(int amount)
    {
        int before = CurrentHp;
        CurrentHp = Math.Min(CurrentHp + amount, GetStat(Stat.MaxHp));
        return CurrentHp - before;
    }

    public void SetHp(int hp) => CurrentHp = Math.Clamp(hp, 0, GetStat(Stat.MaxHp));
    public void TakeDamage(int amount) => CurrentHp = Math.Max(0, CurrentHp - amount);

    // ---------- Solidez ----------

    public void LoseSolidity(int amount) => CurrentSolidity = Math.Max(0, CurrentSolidity - amount);

    // Devuelve cuántos puntos se recuperaron. Un personaje con 0 ya está borrado.
    public int RestoreSolidity(int amount)
    {
        if (CurrentSolidity <= 0) return 0;
        int before = CurrentSolidity;
        CurrentSolidity = Math.Min(CurrentSolidity + amount, GetStat(Stat.MaxSolidity));
        return CurrentSolidity - before;
    }

    public void SetSolidity(int value) => CurrentSolidity = Math.Clamp(value, 0, GetStat(Stat.MaxSolidity));

    // ---------- Estados ----------

    public void AddStatus(StatusType status, int turns) => Statuses[status] = turns;
    public bool RemoveStatus(StatusType status) => Statuses.Remove(status);

    // Deja al personaje como recién creado (se usa al cargar partida)
    public void ResetToDefault()
    {
        Equipped.Clear();
        Statuses.Clear();
        CurrentHp = GetStat(Stat.MaxHp);
        CurrentSolidity = GetStat(Stat.MaxSolidity);
    }

    private void ClampVitals()
    {
        CurrentHp = Math.Min(CurrentHp, GetStat(Stat.MaxHp));
        CurrentSolidity = Math.Min(CurrentSolidity, GetStat(Stat.MaxSolidity));
    }
}
