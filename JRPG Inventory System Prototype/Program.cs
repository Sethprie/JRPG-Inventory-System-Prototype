using JRPG_Inventory_System_Prototype;

// ---------- Ítems ----------
var potion = new ConsumableItem
{
    Id = "potion",
    Name = "Poción",
    Description = "Recupera 50 PV.",
    Category = ItemCategory.Consumable,
    BasePrice = 50,
    Effects = { new HealHpEffect(50) }
};

var antidote = new ConsumableItem
{
    Id = "antidote",
    Name = "Antídoto",
    Description = "Cura el veneno.",
    Category = ItemCategory.Consumable,
    BasePrice = 30,
    Effects = { new RemoveStatusEffect(StatusType.Poison) }
};

var ringFlat = new EquipmentItem
{
    Id = "ring_vitality",
    Name = "Anillo de Vitalidad",
    Description = "+30 PV máximos.",
    Category = ItemCategory.Equipment,
    MaxStack = 1,
    BasePrice = 300,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHp, 30) }
};

var ringPercent = new EquipmentItem
{
    Id = "ring_vigor",
    Name = "Anillo de Vigor",
    Description = "+50% PV máximos.",
    Category = ItemCategory.Equipment,
    MaxStack = 1,
    BasePrice = 800,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHp, 0.5f, ModifierType.Percent) }
};

var key = new ConsumableItem
{
    Id = "key_rusty",
    Name = "Llave oxidada",
    Description = "Abre la bodega.",
    Category = ItemCategory.KeyItem,
    MaxStack = 1
};

// ---------- Base de datos ----------
var db = new ItemDatabase();
db.Register(potion);
db.Register(antidote);
db.Register(ringFlat);
db.Register(ringPercent);
db.Register(key);

// ---------- Inventario ----------
var inv = new Inventory();
Console.WriteLine($"Añadidas: {inv.Add(potion, 95)}");      // 95
Console.WriteLine($"Añadidas: {inv.Add(potion, 10)}");      // 4 (tope 99)
Console.WriteLine($"Añadidas: {inv.Add(ringFlat, 2)}");     // 1 (MaxStack = 1)
Console.WriteLine($"Quitar 5: {inv.Remove("potion", 5)}");  // True

foreach (var (id, qty) in inv.GetAll())
    Console.WriteLine($"{db.Get(id).Name} x{qty}");

// ---------- Personaje y equipo ----------
var hero = new Character("Héroe", 100, 30, 10, 5, 8);
Console.WriteLine($"PV máx base: {hero.GetStat(Stat.MaxHp)}");   // 100

hero.Equip(ringFlat);
Console.WriteLine($"Con +30: {hero.GetStat(Stat.MaxHp)}");       // 130

hero.Equip(ringPercent);
Console.WriteLine($"Con +50%: {hero.GetStat(Stat.MaxHp)}");      // 150

hero.Unequip(EquipSlot.Accessory);
Console.WriteLine($"Sin anillo: {hero.GetStat(Stat.MaxHp)}");    // 100

// ---------- Consumibles ----------
inv.Add(antidote, 3);
hero.TakeDamage(70);
hero.AddStatus(StatusType.Poison, 3);
Console.WriteLine($"PV: {hero.CurrentHp}/{hero.GetStat(Stat.MaxHp)}");   // 30/100

foreach (var msg in inv.Use(potion, hero)) Console.WriteLine(msg);
foreach (var msg in inv.Use(antidote, hero)) Console.WriteLine(msg);
foreach (var msg in inv.Use(antidote, hero)) Console.WriteLine(msg);     // "No tuvo efecto."
Console.WriteLine($"PV: {hero.CurrentHp}/{hero.GetStat(Stat.MaxHp)}");   // 80/100

// ---------- Tienda ----------
inv.Add(key, 1);

var wallet = new Wallet(200);
var general = new Shop { Name = "Tienda general", SellMultiplier = 0.5f };
var smith = new Shop
{
    Name = "Herrería",
    SellMultiplier = 0.3f,
    SellMultiplierByCategory = { [ItemCategory.Equipment] = 0.7f }
};

Console.WriteLine($"Comprar 2 pociones: {general.TryBuy(inv, wallet, potion, 2)} | Oro: {wallet.Gold}"); // Success | 100
Console.WriteLine($"Comprar 5 anillos: {general.TryBuy(inv, wallet, ringFlat, 5)}");                    // InventoryFull
Console.WriteLine($"Vender llave: {general.TrySell(inv, wallet, key, 1)}");                             // NotSellable
Console.WriteLine($"Tirar llave: {inv.TryDiscard(key, 1)}");                                            // NotDiscardable
Console.WriteLine($"Vender anillo (general): {general.GetSellPrice(ringFlat)}");                        // 150
Console.WriteLine($"Vender anillo (herrería): {smith.GetSellPrice(ringFlat)}");                         // 210
Console.WriteLine($"Vender 1 anillo: {smith.TrySell(inv, wallet, ringFlat, 1)} | Oro: {wallet.Gold}"); // Success | 310

// ---------- Guardar / Cargar ----------
hero.Equip(ringPercent);
hero.TakeDamage(40);
hero.AddStatus(StatusType.Poison, 2);

SaveSystem.Save("save.json", inv, wallet, hero);
Console.WriteLine($"Guardado -> PV: {hero.CurrentHp}/{hero.GetStat(Stat.MaxHp)} | Oro: {wallet.Gold}");

// Estropeamos el estado para comprobar que la carga lo restaura
wallet.Set(0);
inv.Clear();
hero.Unequip(EquipSlot.Accessory);
hero.Statuses.Clear();
Console.WriteLine($"Estropeado -> PV: {hero.CurrentHp}/{hero.GetStat(Stat.MaxHp)} | Oro: {wallet.Gold}");

SaveSystem.Load("save.json", db, inv, wallet, hero);
Console.WriteLine($"Cargado -> PV: {hero.CurrentHp}/{hero.GetStat(Stat.MaxHp)} | Oro: {wallet.Gold}");
Console.WriteLine($"Poción x{inv.GetCount("potion")} | Veneno: {hero.Statuses.ContainsKey(StatusType.Poison)}");