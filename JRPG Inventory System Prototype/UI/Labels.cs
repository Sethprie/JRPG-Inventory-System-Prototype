namespace JRPG_Inventory_System_Prototype;

// Atajos para buscar el texto localizado de cada enum (la lógica sigue usando los nombres en inglés)
public static class Labels
{
    public static string StatName(Stat s) => Loc.T($"stat.{s}");
    public static string SlotName(EquipSlot s) => Loc.T($"slot.{s}");
    public static string CategoryName(ItemCategory c) => Loc.T($"category.{c}");
    public static string StatusName(StatusType s) => Loc.T($"status.{s}");
    public static string ClassName(CharacterClass c) => Loc.T($"class.{c}");
    public static string TargetName(TargetType t) => Loc.T($"target.{t}");

    public static string ElementName(Element e) => Loc.T($"element.{e}");
    public static string DamageCategoryName(DamageCategory c) => Loc.T($"damagecat.{c}");

    public static string Describe(EquipmentItem item) => item.EffectText;
}
