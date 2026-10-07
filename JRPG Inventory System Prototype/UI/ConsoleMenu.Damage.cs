namespace JRPG_Inventory_System_Prototype;

// Prueba del cálculo de daño contra un muñeco de entrenamiento (sin azar, para poder verificar los números)
public partial class ConsoleMenu
{
    private void DamageTest()
    {
        Ui.Header("Prueba de daño", g.Wallet.Essence);
        var attacker = PickMember("¿Quién ataca?");
        if (attacker == null) return;
        var enemy = g.Enemies[0];

        Ui.Header("Prueba de daño", g.Wallet.Essence);
        var weapon = attacker.Equipped.GetValueOrDefault(EquipSlot.Weapon);
        Ui.Write($"  {attacker.Name}", ConsoleColor.White);
        Ui.WriteLine($"  con  {(weapon?.DisplayName ?? "(sin arma)")}", ConsoleColor.DarkGray);
        Ui.Write($"  Objetivo: {enemy.DisplayName}", ConsoleColor.White);
        Ui.WriteLine($"  ·  Defensa {enemy.Defense}  ·  Def. mágica {enemy.MagicDefense}", ConsoleColor.DarkGray);
        Ui.WriteLine("  Afinidades: " + string.Join("  ", enemy.Affinities.Select(a => $"{Labels.ElementName(a.Key)} ×{a.Value}")), ConsoleColor.DarkGray);
        Ui.WriteLine();

        var r = DamageCalculator.Preview(attacker, enemy);
        Ui.Section("Daño por componente");
        foreach (var c in r.Components)
        {
            string element = c.Element == Element.None ? "" : $" ({Labels.ElementName(c.Element)})";
            Ui.Write($"  {Labels.DamageCategoryName(c.Category)}{element}".PadRight(26));
            Ui.Write($"({c.Attack:0.#}) × 100/(100+{c.Defense}) × {c.Affinity}".PadRight(34), ConsoleColor.DarkGray);
            Ui.WriteLine($"= {c.Damage}", c.Damage > 0 ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
        }
        Ui.WriteLine();
        Ui.Write("  Total por golpe: ");
        Ui.WriteLine($"{r.Total}", ConsoleColor.Yellow);
        Ui.Dim($"Crítico ({r.CritChance}% de probabilidad): {r.CritTotal}");
        foreach (var s in r.Statuses)
            Ui.Dim($"{Labels.StatusName(s.Status)}: {s.ChancePercent}% por golpe ({s.Turns} turnos)");
        Ui.Pause();
    }
}
