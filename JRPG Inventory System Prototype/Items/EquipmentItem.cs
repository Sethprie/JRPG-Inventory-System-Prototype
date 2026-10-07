namespace JRPG_Inventory_System_Prototype
{
    public class EquipmentItem : ItemDefinition
    {
        public EquipSlot Slot { get; init; }
        public List<StatModifier> Modifiers { get; init; } = new();
        public List<CharacterClass> AllowedClasses { get; init; } = new();   // vacío = cualquier clase
        public List<DamageComponent> DamageComponents { get; init; } = new(); // solo armas
        public List<IEquipPassive> Passives { get; init; } = new();

        public bool CanBeUsedBy(Character c) => AllowedClasses.Count == 0 || AllowedClasses.Contains(c.Class);

        public string RestrictionText => AllowedClasses.Count == 0
            ? ""
            : Loc.T("restriction.classes", string.Join(", ", AllowedClasses.Select(Labels.ClassName)));

        public override string EffectText
        {
            get
            {
                var parts = new List<string>();
                if (Modifiers.Count > 0) parts.Add(string.Join(", ", Modifiers.Select(m => m.Describe())));
                parts.AddRange(DamageComponents.Select(d => d.Describe()));
                parts.AddRange(Passives.Select(p => p.Describe()));
                return string.Join(" · ", parts);
            }
        }
    }
}
