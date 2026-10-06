using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype;

public class SaveData
{
    public int Gold { get; set; }
    public Dictionary<string, int> Items { get; set; } = new();
    public int HeroHp { get; set; }
    public Dictionary<EquipSlot, string> Equipped { get; set; } = new();
    public Dictionary<StatusType, int> Statuses { get; set; } = new();
}