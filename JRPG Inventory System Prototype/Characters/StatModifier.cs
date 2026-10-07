namespace JRPG_Inventory_System_Prototype
{
    public record StatModifier(Stat Stat, float Value, ModifierType Type = ModifierType.Flat)
    {
        public string Describe() => Type == ModifierType.Percent
            ? Loc.T("modifier.percent", Value * 100, Labels.StatName(Stat))
            : Loc.T("modifier.flat", Value, Labels.StatName(Stat));
    }
}
