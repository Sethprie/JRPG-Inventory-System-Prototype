namespace JRPG_Inventory_System_Prototype;

// Un componente de daño del arma. Un arma híbrida tiene dos (uno físico y uno mágico).
public record DamageComponent(DamageCategory Category, int Power, Element Element = Element.None)
{
    public string Describe() => Element == Element.None
        ? Loc.T($"damage.{Category}", Power)
        : Loc.T($"damage.{Category}.elem", Power, Labels.ElementName(Element));
}
