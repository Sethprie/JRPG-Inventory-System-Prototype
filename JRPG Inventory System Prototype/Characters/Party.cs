namespace JRPG_Inventory_System_Prototype;

public class Party
{
    public const int MaxMembers = 4;

    public List<Character> Members { get; } = new();
    public HashSet<string> Erased { get; } = new(); // borrados para siempre
    public Character? Leader => Members.Count > 0 ? Members[0] : null;

    public bool TryAdd(Character c)
    {
        if (Members.Count >= MaxMembers || Members.Contains(c) || Erased.Contains(c.Name)) return false;
        Members.Add(c);
        return true;
    }

    public void Clear()
    {
        Members.Clear();
        Erased.Clear();
    }

    // Un personaje es derrotado en combate. REGLAS PROVISIONALES (el SDD se contradice entre 3.5 y 4.3):
    //  - Pierde 1 de Solidez y reemerge con los PV completos.
    //  - A 0: si es el líder, GAME OVER; si no, se borra y su equipo vuelve a la mochila.
    public DefeatOutcome Defeat(Character c, Inventory inv)
    {
        c.LoseSolidity(1);

        if (c.CurrentSolidity > 0)
        {
            c.SetHealth(c.GetStat(Stat.MaxHealth));
            return DefeatOutcome.SolidityLost;
        }

        if (c == Leader) return DefeatOutcome.GameOver;

        foreach (var slot in c.Equipped.Keys.ToList())
        {
            var item = c.Unequip(slot);
            if (item != null) inv.Add(item, 1);
        }
        Members.Remove(c);
        Erased.Add(c.Name);
        return DefeatOutcome.MemberErased;
    }
}
