
namespace JRPG_Inventory_System_Prototype
{
    public abstract class ItemDefinition
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public ItemCategory Category { get; init; }
        public int MaxStack { get; init; } = 99;
        public int BasePrice { get; init; }

        public bool CanSell { get; init; } = true;
        public bool CanDiscard { get; init; } = true;

        public bool IsSellable => Category != ItemCategory.KeyItem && CanSell;
        public bool IsDiscardable => Category != ItemCategory.KeyItem && CanDiscard;
    }
}
