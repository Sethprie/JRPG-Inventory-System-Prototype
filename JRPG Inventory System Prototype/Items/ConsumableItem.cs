
namespace JRPG_Inventory_System_Prototype;

public class ConsumableItem : ItemDefinition
{
    public List<IItemEffect> Effects { get; init; } = new();
}