namespace JRPG_Inventory_System_Prototype;

// Resultado estructurado de un efecto (la UI o el combate deciden cómo mostrarlo)
public record EffectResult(string Message, bool Applied);

public interface IItemEffect
{
    bool CanAffect(Character target);      // ¿tendría algún efecto sobre este objetivo?
    EffectResult Apply(Character target);
    string Describe();                     // texto de efecto del ítem, generado desde los datos
}

public class HealHpEffect(int amount) : IItemEffect
{
    public bool CanAffect(Character t) => t.CurrentHealth < t.GetStat(Stat.MaxHealth);
    public string Describe() => Loc.T("effect.heal_hp", amount);

    public EffectResult Apply(Character t)
    {
        int healed = t.Heal(amount);
        return healed > 0
            ? new(Loc.T("msg.heal", t.Name, healed), true)
            : new(Loc.T("msg.heal_full", t.Name), false);
    }
}

public class RemoveStatusEffect(StatusType status) : IItemEffect
{
    public bool CanAffect(Character t) => t.Statuses.ContainsKey(status);
    public string Describe() => Loc.T("effect.remove_status", Labels.StatusName(status));

    public EffectResult Apply(Character t) => t.RemoveStatus(status)
        ? new(Loc.T("msg.status_removed", t.Name, Labels.StatusName(status)), true)
        : new(Loc.T("msg.status_none", t.Name, Labels.StatusName(status)), false);
}

public class ApplyStatusEffect(StatusType status, int turns) : IItemEffect
{
    public bool CanAffect(Character t) => true;
    public string Describe() => Loc.T("effect.apply_status", Labels.StatusName(status), turns);

    public EffectResult Apply(Character t)
    {
        t.AddStatus(status, turns);
        return new(Loc.T("msg.status_applied", t.Name, Labels.StatusName(status)), true);
    }
}

public class RestoreSolidityEffect(int amount) : IItemEffect
{
    public bool CanAffect(Character t) => t.CurrentSolidity > 0 && t.CurrentSolidity < t.GetStat(Stat.MaxSolidity);
    public string Describe() => Loc.T("effect.restore_solidity", amount);

    public EffectResult Apply(Character t)
    {
        int restored = t.RestoreSolidity(amount);
        return restored > 0
            ? new(Loc.T("msg.solidity_restored", t.Name, restored), true)
            : new(Loc.T("msg.solidity_full", t.Name), false);
    }
}

public class RestoreMagicEffect(int amount) : IItemEffect
{
    public bool CanAffect(Character t) => t.CurrentMagic < t.GetStat(Stat.MaxMagic);
    public string Describe() => Loc.T("effect.restore_magic", amount);

    public EffectResult Apply(Character t)
    {
        int restored = t.RestoreMagic(amount);
        return restored > 0
            ? new(Loc.T("msg.magic_restored", t.Name, restored), true)
            : new(Loc.T("msg.magic_full", t.Name), false);
    }
}
