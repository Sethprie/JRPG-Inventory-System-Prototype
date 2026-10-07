namespace JRPG_Inventory_System_Prototype;

public class ConsumableItem : ItemDefinition
{
    public List<IItemEffect> Effects { get; init; } = new();
    public UseContext UsableIn { get; init; } = UseContext.Combat | UseContext.Map;
    public TargetType Target { get; init; } = TargetType.SingleAlly;

    public override string EffectText => string.Join(" ", Effects.Select(e => e.Describe()));
}
