using System;
using System.Collections.Generic;
using System.Text;

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

    public List<string> Use(ConsumableItem item, Character target)
    {
        if (!Remove(item.Id, 1))
            return new() { $"No tienes {item.Name}." };

        return item.Effects.Select(e => e.Apply(target)).ToList();
    }

    public TradeResult TryDiscard(ItemDefinition item, int amount)
    {
        if (!item.IsDiscardable) return TradeResult.NotDiscardable;
        return Remove(item.Id, amount) ? TradeResult.Success : TradeResult.NotEnoughItems;
    }

    public void Clear() => _items.Clear();
    public void Set(string id, int qty) => _items[id] = qty;
}