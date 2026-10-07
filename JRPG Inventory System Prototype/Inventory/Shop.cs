namespace JRPG_Inventory_System_Prototype;

public class Shop
{
    public required string Name { get; init; }
    public List<string> Stock { get; init; } = new();   // IDs de lo que vende
    public string? UnlockFlag { get; init; }            // null = abierta desde el inicio
    public float BuyMultiplier { get; init; } = 1.0f;   // lo que cobra al jugador
    public float SellMultiplier { get; init; } = 0.5f;  // lo que paga al jugador
    public Dictionary<ItemCategory, float> SellMultiplierByCategory { get; init; } = new();

    public bool IsUnlocked(WorldFlags flags) => UnlockFlag == null || flags.Has(UnlockFlag);

    public int GetBuyPrice(ItemDefinition item) =>
        (int)Math.Ceiling(item.BasePrice * BuyMultiplier);

    public int GetSellPrice(ItemDefinition item)
    {
        float mult = SellMultiplierByCategory.TryGetValue(item.Category, out var m) ? m : SellMultiplier;
        return (int)Math.Floor(item.BasePrice * mult);
    }

    public TradeResult TryBuy(Inventory inv, Wallet wallet, ItemDefinition item, int amount)
    {
        if (!Stock.Contains(item.Id)) return TradeResult.NotInStock;
        // Comprobamos el espacio antes de cobrar
        if (inv.GetCount(item.Id) + amount > item.MaxStack) return TradeResult.InventoryFull;
        if (!wallet.TrySpend(GetBuyPrice(item) * amount)) return TradeResult.NotEnoughMoney;
        inv.Add(item, amount);
        return TradeResult.Success;
    }

    public TradeResult TrySell(Inventory inv, Wallet wallet, ItemDefinition item, int amount)
    {
        if (!item.IsSellable) return TradeResult.NotSellable;
        if (!inv.Remove(item.Id, amount)) return TradeResult.NotEnoughItems;
        wallet.Add(GetSellPrice(item) * amount);
        return TradeResult.Success;
    }
}
