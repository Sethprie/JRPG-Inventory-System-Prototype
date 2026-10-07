namespace JRPG_Inventory_System_Prototype;

// Primarios al nivel 1 y cuánto sube cada uno por nivel (valores provisionales)
public record ClassDefinition(Dictionary<Stat, int> BasePrimaries, Dictionary<Stat, int> GrowthPerLevel);

public static class Classes
{
    private static Dictionary<Stat, int> P(int str, int agi, int end, int intl, int wis, int luck) => new()
    {
        [Stat.Strength] = str, [Stat.Agility] = agi, [Stat.Endurance] = end,
        [Stat.Intelligence] = intl, [Stat.Wisdom] = wis, [Stat.Luck] = luck
    };

    private static readonly Dictionary<CharacterClass, ClassDefinition> Defs = new()
    {
        [CharacterClass.Warrior]   = new(P(10, 5, 10, 3, 4, 4),  P(3, 1, 3, 0, 1, 1)),
        [CharacterClass.Hunter]    = new(P(8, 10, 6, 4, 4, 7),   P(2, 3, 1, 1, 1, 2)),
        [CharacterClass.WhiteMage] = new(P(3, 5, 5, 9, 10, 5),   P(0, 1, 1, 2, 3, 1)),
        [CharacterClass.BlackMage] = new(P(3, 6, 4, 12, 6, 5),   P(0, 1, 1, 3, 2, 1))
    };

    public static ClassDefinition Get(CharacterClass c) => Defs[c];
}
