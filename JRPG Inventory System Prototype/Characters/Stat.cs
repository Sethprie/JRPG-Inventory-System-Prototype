namespace JRPG_Inventory_System_Prototype;

public enum Stat
{
    // Primarios: suben solos con el nivel, según la clase
    Strength, Agility, Endurance, Intelligence, Wisdom, Luck,
    // Derivados: se calculan a partir de los primarios
    MaxHealth, MaxMagic, AttackPower, MagicPower, Defense, MagicDefense,
    Speed, CritChance, CritDamage, Evasion, MaxSolidity
}
