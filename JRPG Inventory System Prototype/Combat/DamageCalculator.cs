namespace JRPG_Inventory_System_Prototype;

public record ComponentResult(DamageCategory Category, Element Element, float Attack, int Defense, float Affinity, int Damage);
public record StatusChance(StatusType Status, int ChancePercent, int Turns);
public record AttackResult(List<ComponentResult> Components, int Total, int CritChance, int CritTotal, List<StatusChance> Statuses);

// Fórmula provisional y deterministica (sin azar): daño = (ataque + poder del arma) × 100 / (100 + defensa) × afinidad
public static class DamageCalculator
{
    public static AttackResult Preview(Character attacker, EnemyDefinition target)
    {
        var weapon = attacker.Equipped.GetValueOrDefault(EquipSlot.Weapon);
        var components = weapon is { DamageComponents.Count: > 0 }
            ? weapon.DamageComponents
            : new List<DamageComponent> { new(DamageCategory.Physical, 0) }; // sin arma: golpe desarmado

        var imbue = attacker.Equipped.Values.SelectMany(e => e.Passives).OfType<ImbuePhysicalElementPassive>().FirstOrDefault();

        var results = new List<ComponentResult>();
        foreach (var comp in components)
        {
            var element = comp.Category == DamageCategory.Physical && imbue != null ? imbue.Element : comp.Element;
            float attack = attacker.GetStat(comp.Category == DamageCategory.Physical ? Stat.AttackPower : Stat.MagicPower) + comp.Power;
            int defense = target.GetDefense(comp.Category);
            float affinity = target.GetAffinity(element);

            float raw = attack * 100f / (100f + defense) * affinity;
            int damage = affinity > 0 ? Math.Max(1, (int)Math.Round(raw)) : (int)Math.Round(raw); // inmune = 0, absorbe < 0
            results.Add(new(comp.Category, element, attack, defense, affinity, damage));
        }

        int total = results.Sum(r => r.Damage);
        int critTotal = total * attacker.GetStat(Stat.CritDamage) / 100;
        var statuses = attacker.Equipped.Values.SelectMany(e => e.Passives).OfType<StatusOnHitPassive>()
            .Select(p => new StatusChance(p.Status, p.ChancePercent, p.Turns)).ToList();

        return new(results, total, attacker.GetStat(Stat.CritChance), critTotal, statuses);
    }
}
