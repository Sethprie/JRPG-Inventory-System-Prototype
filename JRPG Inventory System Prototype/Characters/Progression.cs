namespace JRPG_Inventory_System_Prototype;

public static class Progression
{
    public const int MaxLevel = 20;                                            // provisional
    public static int ExperienceForNext(int level) => 25 * level * (level + 1); // provisional
}
