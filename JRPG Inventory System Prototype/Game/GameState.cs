namespace JRPG_Inventory_System_Prototype;

// Todo el estado de la partida en un solo sitio (en Unreal sería tu GameInstance)
public class GameState
{
    public required ItemDatabase Database { get; init; }
    public required Wallet Wallet { get; init; }
    public required List<Character> Roster { get; init; }   // los 4 personajes jugables
    public required List<Shop> Shops { get; init; }
    public Inventory Inventory { get; } = new();
    public Party Party { get; } = new();
    public WorldFlags Flags { get; } = new();
}
