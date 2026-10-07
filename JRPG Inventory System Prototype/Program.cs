using JRPG_Inventory_System_Prototype;

// ---------- Textos (idioma base: inglés) ----------
Loc.LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "Localization"));
Loc.SetLanguage("es");

// ---------- Consumibles ----------
var potion = new ConsumableItem
{
    Id = "potion", Category = ItemCategory.Consumable, BasePrice = 50,
    Effects = { new HealHpEffect(50) }
};

var antidote = new ConsumableItem
{
    Id = "antidote", Category = ItemCategory.Consumable, BasePrice = 30,
    Effects = { new RemoveStatusEffect(StatusType.Poison) }
};

var ether = new ConsumableItem
{
    Id = "ether", Category = ItemCategory.Consumable, BasePrice = 80,
    Effects = { new RestoreMagicEffect(30) }
};

var shard = new ConsumableItem
{
    Id = "cohesion_shard", Category = ItemCategory.Consumable, BasePrice = 150,
    UsableIn = UseContext.Map,
    Effects = { new RestoreSolidityEffect(1) }
};

var tonic = new ConsumableItem
{
    Id = "group_tonic", Category = ItemCategory.Consumable, BasePrice = 120,
    Target = TargetType.AllAllies,
    Effects = { new HealHpEffect(30) }
};

var key = new ConsumableItem
{
    Id = "key_rusty", Category = ItemCategory.KeyItem, MaxStack = 1
};

// ---------- Equipo ----------
var ringFlat = new EquipmentItem
{
    Id = "ring_vitality", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 300,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHealth, 30) }
};

var ringPercent = new EquipmentItem
{
    Id = "ring_vigor", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 800,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxHealth, 0.5f, ModifierType.Percent) }
};

var amulet = new EquipmentItem
{
    Id = "amulet_cohesion", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 600,
    Slot = EquipSlot.Accessory,
    Modifiers = { new StatModifier(Stat.MaxSolidity, 1) }
};

var emberRing = new EquipmentItem
{
    Id = "ember_ring", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 700,
    Slot = EquipSlot.Accessory,
    Passives = { new ImbuePhysicalElementPassive(Element.Fire), new StatusOnHitPassive(StatusType.Burning, 30, 3) }
};

// Ejemplo de ítem que modifica varios stats a la vez (primarios, derivados, fijo y porcentual)
var pendant = new EquipmentItem
{
    Id = "wayfarer_pendant", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 500,
    Slot = EquipSlot.Accessory,
    Modifiers =
    {
        new StatModifier(Stat.Strength, 3),
        new StatModifier(Stat.Luck, 2),
        new StatModifier(Stat.MaxHealth, 20),
        new StatModifier(Stat.Defense, 0.1f, ModifierType.Percent)
    }
};

// +20 a todos los stats (menos Solidez máx, que no es un stat de combate), fuego + luz y veneno al golpear
var excalibur = new EquipmentItem
{
    Id = "excalibur", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 5000,
    Slot = EquipSlot.Weapon,
    AllowedClasses = { CharacterClass.Warrior },
    Modifiers = Enum.GetValues<Stat>().Where(s => s != Stat.MaxSolidity).Select(s => new StatModifier(s, 20)).ToList(),
    DamageComponents =
    {
        new DamageComponent(DamageCategory.Physical, 15),
        new DamageComponent(DamageCategory.Magical, 10, Element.Fire),
        new DamageComponent(DamageCategory.Magical, 10, Element.Holy)
    },
    Passives = { new StatusOnHitPassive(StatusType.Poison, 100, 3) }
};

var ironSword = new EquipmentItem
{
    Id = "iron_sword", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 250,
    Slot = EquipSlot.Weapon,
    AllowedClasses = { CharacterClass.Warrior },
    DamageComponents = { new DamageComponent(DamageCategory.Physical, 8) }
};

var flameBrand = new EquipmentItem
{
    Id = "flame_brand", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 900,
    Slot = EquipSlot.Weapon,
    AllowedClasses = { CharacterClass.Warrior },
    DamageComponents = { new DamageComponent(DamageCategory.Physical, 6), new DamageComponent(DamageCategory.Magical, 6, Element.Fire) }
};

var staff = new EquipmentItem
{
    Id = "apprentice_staff", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 250,
    Slot = EquipSlot.Weapon,
    AllowedClasses = { CharacterClass.WhiteMage, CharacterClass.BlackMage },
    DamageComponents = { new DamageComponent(DamageCategory.Magical, 7) }
};

var ironMail = new EquipmentItem
{
    Id = "iron_mail", Category = ItemCategory.Equipment, MaxStack = 1, BasePrice = 400,
    Slot = EquipSlot.Armor,
    AllowedClasses = { CharacterClass.Warrior, CharacterClass.Hunter },
    Modifiers = { new StatModifier(Stat.Defense, 8) }
};

// ---------- Base de datos ----------
var db = new ItemDatabase();
foreach (var item in new ItemDefinition[]
{
    potion, antidote, ether, shard, tonic, key,
    ringFlat, ringPercent, amulet, emberRing, pendant, excalibur, ironSword, flameBrand, staff, ironMail
})
    db.Register(item);

// ---------- Personajes jugables ----------
var roster = new List<Character>
{
    new("Guerrero",    CharacterClass.Warrior),
    new("Cazador",     CharacterClass.Hunter),
    new("Maga Blanca", CharacterClass.WhiteMage),
    new("Maga Negra",  CharacterClass.BlackMage)
};

// ---------- Tiendas ----------
var shops = new List<Shop>
{
    new()
    {
        Name = "Vendedor del Asentamiento",
        Stock = { "potion", "antidote", "ether", "cohesion_shard", "group_tonic" },
        SellMultiplier = 0.5f
    },
    new()
    {
        Name = "Herrería",
        UnlockFlag = "smith_open",
        Stock = { "ring_vitality", "ring_vigor", "amulet_cohesion", "ember_ring", "wayfarer_pendant", "iron_sword", "flame_brand", "apprentice_staff", "iron_mail" },
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
state.Enemies.Add(new EnemyDefinition("training_dummy", 20, 20,
    new() { [Element.Fire] = 1.5f, [Element.Ice] = 0.5f, [Element.Lightning] = 0f }));

state.Inventory.Add(potion, 3);
state.Inventory.Add(antidote, 2);
state.Inventory.Add(ether, 2);
state.Inventory.Add(tonic, 2);
state.Inventory.Add(key, 1);
foreach (var gear in new[] { ringFlat, amulet, emberRing, pendant, excalibur, ironSword, flameBrand, staff, ironMail })
    state.Inventory.Add(gear, 1);

new ConsoleMenu(state, "save.json").Run();
