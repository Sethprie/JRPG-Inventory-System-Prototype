using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace JRPG_Inventory_System_Prototype;

public static class SaveSystem
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Save(string path, Inventory inv, Wallet wallet, Character hero)
    {
        var data = new SaveData
        {
            Gold = wallet.Gold,
            Items = inv.GetAll().ToDictionary(p => p.Key, p => p.Value),
            HeroHp = hero.CurrentHp,
            Equipped = hero.Equipped.ToDictionary(p => p.Key, p => p.Value.Id),
            Statuses = new(hero.Statuses)
        };
        File.WriteAllText(path, JsonSerializer.Serialize(data, Options));
    }

    public static void Load(string path, ItemDatabase db, Inventory inv, Wallet wallet, Character hero)
    {
        var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path))!;

        wallet.Set(data.Gold);

        inv.Clear();
        foreach (var (id, qty) in data.Items) inv.Set(id, qty);

        foreach (var slot in hero.Equipped.Keys.ToList()) hero.Unequip(slot);
        foreach (var id in data.Equipped.Values) hero.Equip((EquipmentItem)db.Get(id));

        hero.Statuses.Clear();
        foreach (var (status, turns) in data.Statuses) hero.AddStatus(status, turns);

        hero.SetHp(data.HeroHp); // al final, cuando el equipo ya define el PV máximo
    }
}