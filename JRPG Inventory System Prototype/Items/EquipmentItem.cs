using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype
{
    public class EquipmentItem : ItemDefinition
    {
        public EquipSlot Slot { get; init; }
        public List<StatModifier> Modifiers { get; init; } = new();
    }
}
