namespace JRPG_Inventory_System_Prototype;

// Resultado estructurado de un efecto (la UI o el combate deciden cómo mostrarlo)
public record EffectResult(string Message, bool Applied);

public interface IItemEffect
{
    bool CanAffect(Character target);      // ¿tendría algún efecto sobre este objetivo?
    EffectResult Apply(Character target);
}

public class HealHpEffect(int amount) : IItemEffect
{
    public bool CanAffect(Character t) => t.CurrentHp < t.GetStat(Stat.MaxHp);

    public EffectResult Apply(Character t)
    {
        int healed = t.Heal(amount);
        return healed > 0
            ? new($"{t.Name} recupera {healed} PV.", true)
            : new($"{t.Name} ya tiene los PV al máximo.", false);
    }
}

public class RemoveStatusEffect(StatusType status) : IItemEffect
{
    public bool CanAffect(Character t) => t.Statuses.ContainsKey(status);

    public EffectResult Apply(Character t) => t.RemoveStatus(status)
        ? new($"{t.Name} ya no tiene {Labels.StatusName(status)}.", true)
        : new($"{t.Name} no tenía {Labels.StatusName(status)}.", false);
}

public class ApplyStatusEffect(StatusType status, int turns) : IItemEffect
{
    public bool CanAffect(Character t) => true;

    public EffectResult Apply(Character t)
    {
        t.AddStatus(status, turns);
        return new($"{t.Name} sufre {Labels.StatusName(status)}.", true);
    }
}

public class RestoreSolidityEffect(int amount) : IItemEffect
{
    public bool CanAffect(Character t) => t.CurrentSolidity > 0 && t.CurrentSolidity < t.GetStat(Stat.MaxSolidity);

    public EffectResult Apply(Character t)
    {
        int restored = t.RestoreSolidity(amount);
        return restored > 0
            ? new($"{t.Name} recupera {restored} de Solidez.", true)
            : new($"{t.Name} ya tiene la Solidez al máximo.", false);
    }
}
