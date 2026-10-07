namespace JRPG_Inventory_System_Prototype;

// Recuperar Solidez en el Asentamiento a cambio de Esencia
public static class SolidityService
{
    public const int CostPerPoint = 100; // provisional

    public static TradeResult TryRestore(Character c, Wallet wallet)
    {
        if (c.CurrentSolidity >= c.GetStat(Stat.MaxSolidity)) return TradeResult.NothingToRestore;
        if (!wallet.TrySpend(CostPerPoint)) return TradeResult.NotEnoughMoney;
        c.RestoreSolidity(1);
        return TradeResult.Success;
    }
}
