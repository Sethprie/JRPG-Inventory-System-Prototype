namespace JRPG_Inventory_System_Prototype;

// Flags globales del mundo (tiendas que se abren, quests completadas, etc.)
public class WorldFlags
{
    private readonly HashSet<string> _flags = new();

    public bool Has(string flag) => _flags.Contains(flag);
    public void Set(string flag) => _flags.Add(flag);
    public void Unset(string flag) => _flags.Remove(flag);
    public void Clear() => _flags.Clear();
    public IEnumerable<string> All => _flags;
}
