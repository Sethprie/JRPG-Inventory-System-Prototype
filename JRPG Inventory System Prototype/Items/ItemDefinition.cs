namespace JRPG_Inventory_System_Prototype
{
    public abstract class ItemDefinition
    {
        // Identificador estable: nunca se traduce ni se muestra. Los textos se buscan por él.
        public required string Id { get; init; }
        public ItemCategory Category { get; init; }
        public int MaxStack { get; init; } = 99;
        public int BasePrice { get; init; }

        public bool CanSell { get; init; } = true;
        public bool CanDiscard { get; init; } = true;

        public bool IsSellable => Category != ItemCategory.KeyItem && CanSell;
        public bool IsDiscardable => Category != ItemCategory.KeyItem && CanDiscard;

        // ---------- Textos (localizados) ----------
        public string DisplayName => Loc.T($"item.{Id}.name");
        public string Lore => Loc.T($"item.{Id}.lore");

        // Texto de efecto generado a partir de los datos del ítem (nunca escrito a mano)
        public virtual string EffectText => "";
    }
}
