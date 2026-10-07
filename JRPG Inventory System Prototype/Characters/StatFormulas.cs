namespace JRPG_Inventory_System_Prototype;

// Cómo se calculan los derivados. Todo lineal: derivado = base + Σ(primario × coeficiente) + equipo
public static class StatFormulas
{
    public static bool IsPrimary(Stat s) => s <= Stat.Luck;

    public static readonly Dictionary<Stat, int> BaseDerived = new()
    {
        [Stat.MaxHealth] = 40,
        [Stat.MaxMagic] = 10,
        [Stat.CritDamage] = 150,   // % del daño normal
        [Stat.MaxSolidity] = 3
    };

    public static readonly Dictionary<Stat, (Stat Primary, float Coef)[]> Sources = new()
    {
        [Stat.MaxHealth]    = [(Stat.Endurance, 10f)],
        [Stat.MaxMagic]     = [(Stat.Intelligence, 3f), (Stat.Wisdom, 3f)],
        [Stat.AttackPower]  = [(Stat.Strength, 2f)],
        [Stat.MagicPower]   = [(Stat.Intelligence, 2f)],
        [Stat.Defense]      = [(Stat.Endurance, 1f), (Stat.Strength, 0.5f)],
        [Stat.MagicDefense] = [(Stat.Wisdom, 2f)],
        [Stat.Speed]        = [(Stat.Agility, 1f)],
        [Stat.CritChance]   = [(Stat.Agility, 0.5f), (Stat.Luck, 0.5f)],   // en puntos porcentuales
        [Stat.Evasion]      = [(Stat.Luck, 1f)]                            // prob. de que el enemigo falle
    };

    public static readonly Dictionary<Stat, int> Caps = new()
    {
        [Stat.CritChance] = 100,
        [Stat.Evasion] = 40
    };
}
