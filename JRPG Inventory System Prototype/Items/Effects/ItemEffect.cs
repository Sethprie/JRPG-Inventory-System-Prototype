using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype;

public interface IItemEffect
{
    string Apply(Character target); // devuelve un mensaje para la UI
}

public class HealHpEffect(int amount) : IItemEffect
{
    public string Apply(Character t) => $"{t.Name} recupera {t.Heal(amount)} PV.";
}

public class RemoveStatusEffect(StatusType status) : IItemEffect
{
    public string Apply(Character t) =>
        t.RemoveStatus(status) ? $"{t.Name} ya no tiene {status}." : "No tuvo efecto.";
}

public class ApplyStatusEffect(StatusType status, int turns) : IItemEffect
{
    public string Apply(Character t)
    {
        t.AddStatus(status, turns);
        return $"{t.Name} sufre {status}.";
    }
}