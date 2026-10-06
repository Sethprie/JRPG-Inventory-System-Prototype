using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype
{
    public class ItemDatabase
    {
        private readonly Dictionary<string, ItemDefinition> _defs = new();

        public void Register(ItemDefinition item) => _defs[item.Id] = item;
        public ItemDefinition Get(string id) => _defs[id];
    }
}
