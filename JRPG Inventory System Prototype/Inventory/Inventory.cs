
namespace JRPG_Inventory_System_Prototype;

public class Inventory
{
    private readonly Dictionary<string, int> _items = new();

    public int GetCount(string id) => _items.GetValueOrDefault(id);

    public int Add(ItemDefinition item, int amount)
    {
        int current = GetCount(item.Id);
        int added = Math.Min(amount, item.MaxStack - current);
        if (added <= 0) return 0;
        _items[item.Id] = current + added;
        return added;
    }

    public bool Remove(string id, int amount)
    {
        int current = GetCount(id);
        if (amount <= 0 || current < amount) return false;
        if (current == amount) _items.Remove(id);
        else _items[id] = current - amount;
        return true;
    }

    public IEnumerable<KeyValuePair<string, int>> GetAll() => _items;

    // Valida el uso SIN gastar nada (ítem, turno). Si no hay ningún efecto posible, lo rechaza.
    public UseCheck CanUse(ConsumableItem item, IReadOnlyCollection<Character> targets, UseContext context)
    {
        if (item.Category != ItemCategory.Consumable) return UseCheck.NotUsable;
        if (!item.UsableIn.HasFlag(context)) return UseCheck.WrongContext;
        if (GetCount(item.Id) < 1) return UseCheck.NotInStock;
        bool anyEffect = targets.Any(t => item.Effects.Any(e => e.CanAffect(t)));
        return anyEffect ? UseCheck.Ok : UseCheck.NoEffect;
    }

    // Valida, consume 1 y aplica los efectos a cada objetivo (un solo consumo aunque haya varios)
    public UseResult Use(ConsumableItem item, IReadOnlyCollection<Character> targets, UseContext context)
    {
        var check = CanUse(item, targets, context);
        if (check != UseCheck.Ok) return new(check, new());

        Remove(item.Id, 1);
        var results = targets.SelectMany(t => item.Effects.Select(e => e.Apply(t))).ToList();
        return new(UseCheck.Ok, results);
    }

    public TradeResult TryDiscard(ItemDefinition item, int amount)
    {
        if (!item.IsDiscardable) return TradeResult.NotDiscardable;
        return Remove(item.Id, amount) ? TradeResult.Success : TradeResult.NotEnoughItems;
    }

    public void Clear() => _items.Clear();
    public void Set(string id, int qty) => _items[id] = qty;
}