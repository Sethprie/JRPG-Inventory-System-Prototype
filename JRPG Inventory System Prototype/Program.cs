using JRPG_Inventory_System_Prototype;

// ---------- Ítems ----------
var potion = new ConsumableItem
{
    Id = "potion", Name = "Poción", Description = "Recupera 50 PV.",
    Category = ItemCategory.Consumable, BasePrice = 50,
    Effects = { new HealHpEffect(50) }
};

var antidote = new ConsumableItem
{
    Id = "antidote", Name = "Antídoto", Description = "Cura el veneno.",
    Category = ItemCategory.Consumable, BasePrice = 30,
    Effects = { new RemoveStatusEffect(StatusType.Poison) }
};

var shard = new ConsumableItem
{
    Id = "cohesion_shard", Name = "Fragmento de Cohesión", Description = "Recupera 1 de Solidez. (Ítem de prueba)",
    Category = ItemCategory.Consumable, BasePrice = 150,
    Effects = { new RestoreSolidityEffect(1) }
};

var ringFlat = new EquipmentItem
{
    Id = "ring_vitality", Name = "Anillo de Vitalidad", Description = "+30 PV máximos.",
    Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 300,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHp, 30) }
};

var ringPercent = new EquipmentItem
{
    Id = "ring_vigor", Name = "Anillo de Vigor", Description = "+50% PV máximos.",
    Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 800,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHp, 0.5f, ModifierType.Percent) }
};

var amulet = new EquipmentItem
{
    Id = "amulet_cohesion", Name = "Amuleto de Cohesión", Description = "+1 Solidez máxima.",
    Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 600,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxSolidity, 1) }
};

var key = new ConsumableItem
{
    Id = "key_rusty", Name = "Llave oxidada", Description = "Abre la bodega.",
    Category = ItemCategory.KeyItem, MaxStack = 1
};

// ---------- Base de datos ----------
var db = new ItemDatabase();
foreach (var item in new ItemDefinition[] { potion, antidote, shard, ringFlat, ringPercent, amulet, key })
    db.Register(item);

// ---------- Personajes jugables (stats provisionales) ----------
var roster = new List<Character>
{
    new("Guerrero",    CharacterClass.Warrior,   140, 20, 12, 10, 5),
    new("Cazador",     CharacterClass.Hunter,    100, 25, 13,  5, 12),
    new("Maga Blanca", CharacterClass.WhiteMage,  80, 50,  5,  4, 8),
    new("Maga Negra",  CharacterClass.BlackMage,  85, 60, 16,  3, 7)
};

// ---------- Tiendas ----------
var shops = new List<Shop>
{
    new()
    {
        Name = "Vendedor del Asentamiento",
        Stock = { "potion", "antidote", "cohesion_shard" },
        SellMultiplier = 0.5f
    },
    new()
    {
        Name = "Herrería",
        UnlockFlag = "smith_open",
        Stock = { "ring_vitality", "ring_vigor", "amulet_cohesion" },
        SellMultiplier = 0.3f,
        SellMultiplierByCategory = { [ItemCategory.Equipment] = 0.7f }
    }
};

// ---------- Estado de la partida ----------
var state = new GameState
{
    Database = db,
    Wallet = new Wallet(500),
    Roster = roster,
    Shops = shops
};
state.Inventory.Add(potion, 3);
state.Inventory.Add(antidote, 2);
state.Inventory.Add(ringFlat, 1);
state.Inventory.Add(amulet, 1);
state.Inventory.Add(key, 1);

new ConsoleMenu(state, "save.json").Run();
