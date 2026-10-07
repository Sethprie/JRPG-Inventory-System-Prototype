namespace JRPG_Inventory_System_Prototype;

public class Character
{
    public string Name { get; }
    public CharacterClass Class { get; }
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }
    public Dictionary<EquipSlot, EquipmentItem> Equipped { get; } = new();
    public Dictionary<StatusType, int> Statuses { get; } = new(); // estado -> turnos

    // Estado (cambia en juego; no son stats)
    public int CurrentHealth { get; private set; }
    public int CurrentMagic { get; private set; }
    public int CurrentSolidity { get; private set; }

    public Character(string name, CharacterClass characterClass)
    {
        Name = name;
        Class = characterClass;
        FillVitals();
    }

    // ---------- Stats ----------

    public int GetStat(Stat stat) => GetBreakdown(stat).Final;

    public StatBreakdown GetBreakdown(Stat stat)
    {
        var mods = Equipped.Values.SelectMany(e => e.Modifiers).Where(m => m.Stat == stat).ToList();
        float equipFlat = mods.Where(m => m.Type == ModifierType.Flat).Sum(m => m.Value);
        float percent = mods.Where(m => m.Type == ModifierType.Percent).Sum(m => m.Value);

        var parts = new List<(string, float)>();
        if (StatFormulas.IsPrimary(stat))
        {
            var def = Classes.Get(Class);
            parts.Add((Loc.T("breakdown.base"), def.BasePrimaries[stat]));
            parts.Add((Loc.T("breakdown.level"), def.GrowthPerLevel[stat] * (Level - 1)));
        }
        else
        {
            parts.Add((Loc.T("breakdown.base"), StatFormulas.BaseDerived.GetValueOrDefault(stat)));
            if (StatFormulas.Sources.TryGetValue(stat, out var sources))
                foreach (var (primary, coef) in sources)
                {
                    int value = GetStat(primary);
                    parts.Add(($"{Labels.StatName(primary)} ({value}×{coef})", value * coef));
                }
        }
        parts.Add((Loc.T("breakdown.equipment"), equipFlat));

        int final = (int)Math.Max(0, parts.Sum(p => p.Item2) * (1 + percent));
        if (StatFormulas.Caps.TryGetValue(stat, out int cap)) final = Math.Min(final, cap);
        return new StatBreakdown(stat, parts, percent, final);
    }

    // ---------- Experiencia ----------

    public int ExperienceToNext => Level >= Progression.MaxLevel ? 0 : Progression.ExperienceForNext(Level);

    // Devuelve cuántos niveles subió. Los atributos suben solos según la clase.
    public int AddExperience(int amount)
    {
        if (Level >= Progression.MaxLevel) return 0;
        Experience += amount;
        int gained = 0;
        while (Level < Progression.MaxLevel && Experience >= Progression.ExperienceForNext(Level))
        {
            Experience -= Progression.ExperienceForNext(Level);
            Level++;
            gained++;
        }
        if (Level >= Progression.MaxLevel) Experience = 0;
        return gained;
    }

    public void SetProgress(int level, int experience)
    {
        Level = Math.Clamp(level, 1, Progression.MaxLevel);
        Experience = Math.Max(0, experience);
    }

    // ---------- Equipo ----------

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
        int before = CurrentHealth;
        CurrentHealth = Math.Min(CurrentHealth + amount, GetStat(Stat.MaxHealth));
        return CurrentHealth - before;
    }

    public void SetHealth(int value) => CurrentHealth = Math.Clamp(value, 0, GetStat(Stat.MaxHealth));
    public void TakeDamage(int amount) => CurrentHealth = Math.Max(0, CurrentHealth - amount);

    // ---------- Magia ----------

    public int RestoreMagic(int amount)
    {
        int before = CurrentMagic;
        CurrentMagic = Math.Min(CurrentMagic + amount, GetStat(Stat.MaxMagic));
        return CurrentMagic - before;
    }

    public void SetMagic(int value) => CurrentMagic = Math.Clamp(value, 0, GetStat(Stat.MaxMagic));

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
        Level = 1;
        Experience = 0;
        FillVitals();
    }

    private void FillVitals()
    {
        CurrentHealth = GetStat(Stat.MaxHealth);
        CurrentMagic = GetStat(Stat.MaxMagic);
        CurrentSolidity = GetStat(Stat.MaxSolidity);
    }

    private void ClampVitals()
    {
        CurrentHealth = Math.Min(CurrentHealth, GetStat(Stat.MaxHealth));
        CurrentMagic = Math.Min(CurrentMagic, GetStat(Stat.MaxMagic));
        CurrentSolidity = Math.Min(CurrentSolidity, GetStat(Stat.MaxSolidity));
    }
}
