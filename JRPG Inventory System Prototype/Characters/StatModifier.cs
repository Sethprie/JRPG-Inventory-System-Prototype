using System;
using System.Collections.Generic;
using System.Text;

namespace JRPG_Inventory_System_Prototype
{
    public record StatModifier(Stat Stat, float Value, ModifierType Type = ModifierType.Flat);
}
