namespace JRPG_Inventory_System_Prototype;

// Efectos pasivos de equipo que reaccionan al atacar (no son modificadores de stat)
public interface IEquipPassive
{
    string Describe();
}

// Los componentes físicos del arma pasan a ser de este elemento
public class ImbuePhysicalElementPassive(Element element) : IEquipPassive
{
    public Element Element => element;
    public string Describe() => Loc.T("passive.imbue", Labels.ElementName(element));
}

// Probabilidad de aplicar un estado al golpear
public class StatusOnHitPassive(StatusType status, int chancePercent, int turns) : IEquipPassive
{
    public StatusType Status => status;
    public int ChancePercent => chancePercent;
    public int Turns => turns;
    public string Describe() => Loc.T("passive.status_on_hit", chancePercent, Labels.StatusName(status));
}
