namespace JRPG_Inventory_System_Prototype;

// La única moneda del juego es la Esencia
public class Wallet
{
    public int Essence { get; private set; }

    public Wallet(int essence = 0) => Essence = essence;

    public void Add(int amount) => Essence += amount;

    public bool TrySpend(int amount)
    {
        if (amount > Essence) return false;
        Essence -= amount;
        return true;
    }

    public void Set(int amount) => Essence = amount;
}
