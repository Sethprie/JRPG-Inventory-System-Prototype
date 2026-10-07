namespace JRPG_Inventory_System_Prototype;

public class SaveData
{
    public int Essence { get; set; }
    public Dictionary<string, int> Items { get; set; } = new();
    public List<string> WorldFlags { get; set; } = new();
    public List<string> Erased { get; set; } = new();
    public List<MemberSave> Party { get; set; } = new(); // el primero es el líder
}

public class MemberSave
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int Hp { get; set; }
    public int Magic { get; set; }
    public int Solidity { get; set; }
    public Dictionary<EquipSlot, string> Equipped { get; set; } = new();
    public Dictionary<StatusType, int> Statuses { get; set; } = new();
}
