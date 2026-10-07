using System.Text.Json;

namespace JRPG_Inventory_System_Prototype;

public static class SaveSystem
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Save(string path, GameState s)
    {
        var data = new SaveData
        {
            Essence = s.Wallet.Essence,
            Items = s.Inventory.GetAll().ToDictionary(p => p.Key, p => p.Value),
            WorldFlags = s.Flags.All.ToList(),
            Erased = s.Party.Erased.ToList(),
            Party = s.Party.Members.Select(c => new MemberSave
            {
                Name = c.Name,
                Hp = c.CurrentHp,
                Solidity = c.CurrentSolidity,
                Equipped = c.Equipped.ToDictionary(p => p.Key, p => p.Value.Id),
                Statuses = new(c.Statuses)
            }).ToList()
        };
        File.WriteAllText(path, JsonSerializer.Serialize(data, Options));
    }

    public static void Load(string path, GameState s)
    {
        var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path))!;

        s.Wallet.Set(data.Essence);

        s.Inventory.Clear();
        foreach (var (id, qty) in data.Items) s.Inventory.Set(id, qty);

        s.Flags.Clear();
        foreach (var flag in data.WorldFlags) s.Flags.Set(flag);

        foreach (var c in s.Roster) c.ResetToDefault();
        s.Party.Clear();
        foreach (var name in data.Erased) s.Party.Erased.Add(name);

        foreach (var m in data.Party)
        {
            var c = s.Roster.First(r => r.Name == m.Name);
            foreach (var id in m.Equipped.Values) c.Equip((EquipmentItem)s.Database.Get(id));
            foreach (var (status, turns) in m.Statuses) c.AddStatus(status, turns);
            c.SetHp(m.Hp);               // al final, cuando el equipo ya define los máximos
            c.SetSolidity(m.Solidity);
            s.Party.TryAdd(c);
        }
    }
}
