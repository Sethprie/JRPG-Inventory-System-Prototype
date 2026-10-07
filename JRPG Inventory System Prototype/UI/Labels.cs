namespace JRPG_Inventory_System_Prototype;

// Textos en español para los enums (la lógica sigue usando los nombres en inglés)
public static class Labels
{
    public static string StatName(Stat s) => s switch
    {
        Stat.MaxHp => "PV máx",
        Stat.MaxMp => "PM máx",
        Stat.Attack => "Ataque",
        Stat.Defense => "Defensa",
        Stat.Speed => "Velocidad",
        Stat.MaxSolidity => "Solidez máx",
        _ => s.ToString()
    };

    public static string SlotName(EquipSlot s) => s switch
    {
        EquipSlot.Weapon => "Arma",
        EquipSlot.Armor => "Armadura",
        EquipSlot.Accessory => "Accesorio",
        _ => s.ToString()
    };

    public static string CategoryName(ItemCategory c) => c switch
    {
        ItemCategory.Consumable => "Consumibles",
        ItemCategory.Equipment => "Equipo",
        ItemCategory.Material => "Materiales",
        ItemCategory.KeyItem => "Objetos clave",
        _ => c.ToString()
    };

    public static string StatusName(StatusType s) => s switch
    {
        StatusType.Poison => "Veneno",
        StatusType.Sleep => "Sueño",
        StatusType.Silence => "Silencio",
        _ => s.ToString()
    };

    public static string ClassName(CharacterClass c) => c switch
    {
        CharacterClass.Warrior => "Guerrero",
        CharacterClass.Hunter => "Cazador",
        CharacterClass.WhiteMage => "Maga Blanca",
        CharacterClass.BlackMage => "Maga Negra",
        _ => c.ToString()
    };

    public static string TargetName(TargetType t) => t switch
    {
        TargetType.None => "sin objetivo",
        TargetType.SingleAlly => "un aliado",
        TargetType.AllAllies => "todo el grupo",
        TargetType.SingleEnemy => "un enemigo",
        TargetType.AllEnemies => "todos los enemigos",
        _ => t.ToString()
    };

    public static string Modifier(StatModifier m) => m.Type == ModifierType.Percent
        ? $"{m.Value * 100:+0.##;-0.##}% {StatName(m.Stat)}"
        : $"{m.Value:+0.##;-0.##} {StatName(m.Stat)}";

    public static string Describe(EquipmentItem item) => string.Join(", ", item.Modifiers.Select(Modifier));
}
