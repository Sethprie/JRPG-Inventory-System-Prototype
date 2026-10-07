namespace JRPG_Inventory_System_Prototype;

public class ConsoleMenu(GameState g, string savePath)
{
    private bool _gameOver;
    private readonly List<(string Text, ConsoleColor Color)> _notices = new();

    // ---------- Avisos (se muestran en la siguiente pantalla, no se pierden al limpiar) ----------

    private void Ok(string text) => _notices.Add((text, ConsoleColor.Green));
    private void Warn(string text) => _notices.Add((text, ConsoleColor.Yellow));
    private void Fail(string text) => _notices.Add((text, ConsoleColor.Red));

    private void PrintNotices()
    {
        foreach (var (text, color) in _notices)
        {
            if (color == ConsoleColor.Green) Ui.Ok(text);
            else if (color == ConsoleColor.Yellow) Ui.Warn(text);
            else Ui.Error(text);
        }
        if (_notices.Count > 0) Ui.WriteLine();
        _notices.Clear();
    }

    // ---------- Bucle principal ----------

    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (g.Party.Members.Count == 0) ChooseStarter();

        while (true)
        {
            if (_gameOver) { if (!GameOverScreen()) return; continue; }

            Ui.Clear();
            Ui.Banner("EL DESPERTAR DEL VASTO · Inventario");
            PartyPanel();
            PrintNotices();

            Ui.Section("Menú");
            Ui.MenuRow("Grupo", (1, "Estado"), (3, "Usar ítem"), (4, "Equipo"));
            Ui.MenuRow("Mochila", (2, "Inventario"), (6, "Tirar ítem"));
            Ui.MenuRow("Asentamiento", (5, "Tienda"), (7, "Restaurar Solidez"));
            Ui.MenuRow("Partida", (8, "Guardar"), (9, "Cargar"), (10, "Debug"));
            Ui.MenuRow("", (0, "Salir"));
            Ui.WriteLine();

            switch (Ui.ReadInt(0, 10))
            {
                case 1: ShowStatus(); break;
                case 2: ShowInventory(); break;
                case 3: UseItem(); break;
                case 4: EquipMenu(); break;
                case 5: ShopMenu(); break;
                case 6: DiscardItem(); break;
                case 7: RestoreSolidity(); break;
                case 8: Save(); break;
                case 9: Load(); break;
                case 10: DebugMenu(); break;
                case 0: return;
            }
        }
    }

    private bool GameOverScreen()
    {
        Ui.Clear();
        Ui.WriteLine();
        Ui.WriteLine("  G A M E   O V E R", ConsoleColor.Red);
        Ui.WriteLine("  El líder se ha borrado del mundo.", ConsoleColor.DarkGray);
        Ui.WriteLine();
        PrintNotices();
        Ui.Line(1, "Cargar última partida");
        Ui.Back("Salir");
        Ui.WriteLine();

        if (Ui.ReadInt(0, 1) == 0) return false;
        Load();
        return true;
    }

    // Resumen del grupo, siempre visible arriba
    private void PartyPanel()
    {
        Ui.WriteLine();
        Ui.Write($"  Esencia: {g.Wallet.Essence}", ConsoleColor.Yellow);
        Ui.WriteLine($"   ·   Grupo {g.Party.Members.Count}/{Party.MaxMembers}", ConsoleColor.DarkGray);
        Ui.WriteLine();

        foreach (var c in g.Party.Members)
        {
            Ui.Write(c == g.Party.Leader ? "  ★ " : "    ", ConsoleColor.Yellow);
            Ui.Write(c.Name.PadRight(13));
            Ui.Write("PV ", ConsoleColor.DarkGray);
            Ui.HpBar(c.CurrentHp, c.GetStat(Stat.MaxHp));
            Ui.Write("  Solidez ", ConsoleColor.DarkGray);
            Ui.SolidityPips(c.CurrentSolidity, c.GetStat(Stat.MaxSolidity));
            Ui.WriteLine();
        }
        Ui.WriteLine();
    }

    // ---------- Inicio ----------

    private void ChooseStarter()
    {
        Ui.Clear();
        Ui.Banner("EL DESPERTAR DEL VASTO");
        Ui.WriteLine();
        Ui.Section("Elige a tu personaje inicial (será el líder)");

        var rows = g.Roster.Select(c => (
            c.Name,
            "",
            $"PV {c.BaseStats[Stat.MaxHp]} · PM {c.BaseStats[Stat.MaxMp]} · Ataque {c.BaseStats[Stat.Attack]} · Defensa {c.BaseStats[Stat.Defense]} · Vel {c.BaseStats[Stat.Speed]}"
        )).ToList();
        Ui.Rows(rows, numbered: true);
        Ui.WriteLine();

        int choice = Ui.ReadInt(1, g.Roster.Count);
        g.Party.TryAdd(g.Roster[choice - 1]);
    }

    // ---------- Pantallas de consulta ----------

    private void ShowStatus()
    {
        Ui.Header("Estado del grupo", g.Wallet.Essence);

        foreach (var c in g.Party.Members)
        {
            Ui.Write($"  {c.Name}", ConsoleColor.White);
            Ui.Write($"  ({Labels.ClassName(c.Class)})", ConsoleColor.DarkGray);
            if (c == g.Party.Leader) Ui.Write("  ★ Líder", ConsoleColor.Yellow);
            Ui.WriteLine();

            Ui.Write("    PV         ", ConsoleColor.DarkGray);
            Ui.HpBar(c.CurrentHp, c.GetStat(Stat.MaxHp), 14);
            Ui.WriteLine();
            Ui.Write("    Solidez    ", ConsoleColor.DarkGray);
            Ui.SolidityPips(c.CurrentSolidity, c.GetStat(Stat.MaxSolidity));
            Ui.WriteLine();

            var stats = new[] { Stat.MaxMp, Stat.Attack, Stat.Defense, Stat.Speed };
            Ui.Write("    Atributos  ", ConsoleColor.DarkGray);
            Ui.WriteLine(string.Join("  ·  ", stats.Select(s => $"{Labels.StatName(s)} {c.GetStat(s)}")));

            foreach (var slot in Enum.GetValues<EquipSlot>())
            {
                var eq = c.Equipped.GetValueOrDefault(slot);
                Ui.Write($"    {Labels.SlotName(slot),-11}", ConsoleColor.DarkGray);
                if (eq == null) Ui.WriteLine("—", ConsoleColor.DarkGray);
                else
                {
                    Ui.Write(eq.Name);
                    Ui.WriteLine($"  ({Labels.Describe(eq)})", ConsoleColor.DarkGray);
                }
            }

            Ui.Write("    Estados    ", ConsoleColor.DarkGray);
            if (c.Statuses.Count == 0) Ui.WriteLine("ninguno", ConsoleColor.DarkGray);
            else Ui.WriteLine(string.Join(", ", c.Statuses.Select(s => $"{Labels.StatusName(s.Key)} ({s.Value}t)")), ConsoleColor.Yellow);
            Ui.WriteLine();
        }

        if (g.Party.Erased.Count > 0)
            Ui.Dim($"Borrados para siempre: {string.Join(", ", g.Party.Erased)}");
        Ui.Pause();
    }

    private void ShowInventory()
    {
        Ui.Header("Inventario", g.Wallet.Essence);

        var entries = GetInventoryEntries(_ => true);
        if (entries.Count == 0) Ui.Dim("La mochila está vacía.");

        foreach (var group in entries.GroupBy(e => e.Def.Category))
        {
            Ui.Section(Labels.CategoryName(group.Key));
            Ui.Rows(group.Select(e => (e.Def.Name, $"x{e.Qty}", ItemNote(e.Def))).ToList(), numbered: false);
            Ui.WriteLine();
        }
        Ui.Pause();
    }

    // ---------- Acciones ----------

    private void UseItem()
    {
        Ui.Header("Usar ítem", g.Wallet.Essence);
        var item = PickFromInventory(i => i.Category == ItemCategory.Consumable);
        if (item is not ConsumableItem consumable) return;

        var target = PickMember("¿Sobre quién?");
        if (target == null) return;

        foreach (var msg in g.Inventory.Use(consumable, target))
        {
            if (msg == "No tuvo efecto.") Warn($"{consumable.Name} sobre {target.Name}: no tuvo efecto.");
            else Ok(msg);
        }
        Ui.Dim($"Te quedan {g.Inventory.GetCount(item.Id)} de {item.Name}.");
    }

    private void DiscardItem()
    {
        Ui.Header("Tirar ítem", g.Wallet.Essence);
        var item = PickFromInventory(_ => true, i => i.IsDiscardable ? "" : "no se puede tirar");
        if (item == null) return;

        int max = Math.Max(1, g.Inventory.GetCount(item.Id));
        int qty = Ui.ReadInt(1, max, "Cantidad a tirar");
        var result = g.Inventory.TryDiscard(item, qty);
        if (result == TradeResult.Success) Ok($"Tiraste {item.Name} x{qty}.");
        else Fail(TradeMessage(result));
    }

    private void RestoreSolidity()
    {
        Ui.Header("Restaurar Solidez", g.Wallet.Essence);
        Ui.Dim($"Cuesta {SolidityService.CostPerPoint} Esencia por punto.");
        Ui.WriteLine();

        var target = PickMember("¿A quién restaurar?");
        if (target == null) return;

        var result = SolidityService.TryRestore(target, g.Wallet);
        if (result == TradeResult.Success)
            Ok($"{target.Name} recupera 1 de Solidez (-{SolidityService.CostPerPoint} Esencia).");
        else if (result == TradeResult.NothingToRestore) Warn(TradeMessage(result));
        else Fail(TradeMessage(result));
    }

    // ---------- Equipo ----------

    private void EquipMenu()
    {
        Ui.Header("Equipo", g.Wallet.Essence);
        var member = PickMember("¿Quién?");
        if (member == null) return;

        Ui.Header($"Equipo de {member.Name}", g.Wallet.Essence);
        foreach (var slot in Enum.GetValues<EquipSlot>())
        {
            var eq = member.Equipped.GetValueOrDefault(slot);
            Ui.Write($"  {Labels.SlotName(slot),-11}", ConsoleColor.DarkGray);
            if (eq == null) Ui.WriteLine("—", ConsoleColor.DarkGray);
            else { Ui.Write(eq.Name); Ui.WriteLine($"  ({Labels.Describe(eq)})", ConsoleColor.DarkGray); }
        }
        Ui.WriteLine();

        Ui.Line(1, "Equipar desde la mochila");
        Ui.Line(2, "Desequipar un slot");
        Ui.Back();
        Ui.WriteLine();

        switch (Ui.ReadInt(0, 2))
        {
            case 1: EquipFromInventory(member); break;
            case 2: UnequipSlot(member); break;
        }
    }

    private void EquipFromInventory(Character member)
    {
        Ui.Header($"Equipar a {member.Name}", g.Wallet.Essence);
        var picked = PickFromInventory(i => i is EquipmentItem, i => Labels.Describe((EquipmentItem)i));
        if (picked is not EquipmentItem item) return;

        // Comprobamos antes que el ítem que se va a reemplazar tenga hueco en la mochila
        var current = member.Equipped.GetValueOrDefault(item.Slot);
        if (current != null && current.Id != item.Id && g.Inventory.GetCount(current.Id) >= current.MaxStack)
        {
            Fail($"No hay espacio en la mochila para {current.Name}.");
            return;
        }

        g.Inventory.Remove(item.Id, 1);
        var previous = member.Equip(item);
        if (previous != null) g.Inventory.Add(previous, 1);

        Ok($"{member.Name} equipa {item.Name}.");
        if (previous != null) Ui.Dim($"{previous.Name} vuelve a la mochila.");
    }

    private void UnequipSlot(Character member)
    {
        var slots = Enum.GetValues<EquipSlot>().Where(member.Equipped.ContainsKey).ToList();
        if (slots.Count == 0) { Warn($"{member.Name} no lleva nada equipado."); return; }

        Ui.Header($"Desequipar a {member.Name}", g.Wallet.Essence);
        Ui.Rows(slots.Select(s => (Labels.SlotName(s), member.Equipped[s].Name, "")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, slots.Count);
        if (choice == 0) return;

        var equipped = member.Equipped[slots[choice - 1]];
        if (g.Inventory.GetCount(equipped.Id) >= equipped.MaxStack)
        {
            Fail($"No hay espacio en la mochila para {equipped.Name}.");
            return;
        }

        var item = member.Unequip(slots[choice - 1])!;
        g.Inventory.Add(item, 1);
        Ok($"{item.Name} vuelve a la mochila.");
    }

    // ---------- Tienda ----------

    private void ShopMenu()
    {
        Ui.Header("Tiendas", g.Wallet.Essence);
        var open = g.Shops.Where(s => s.IsUnlocked(g.Flags)).ToList();
        Ui.Rows(open.Select(s => (s.Name, "", "")).ToList(), numbered: true);
        int closed = g.Shops.Count - open.Count;
        if (closed > 0) Ui.Dim($"({closed} tienda(s) aún cerrada(s))");
        Ui.Back();
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, open.Count);
        if (choice == 0) return;
        var shop = open[choice - 1];

        while (true)
        {
            Ui.Header(shop.Name, g.Wallet.Essence);
            PrintNotices();
            Ui.Line(1, "Comprar");
            Ui.Line(2, "Vender");
            Ui.Back("Salir de la tienda");
            Ui.WriteLine();

            switch (Ui.ReadInt(0, 2))
            {
                case 1: Buy(shop); break;
                case 2: Sell(shop); break;
                case 0: return;
            }
        }
    }

    private void Buy(Shop shop)
    {
        Ui.Header($"{shop.Name} · Comprar", g.Wallet.Essence);
        var stock = shop.Stock.Select(g.Database.Get).ToList();
        if (stock.Count == 0) { Warn("No tiene nada a la venta."); return; }

        Ui.Rows(stock.Select(i => (i.Name, $"{shop.GetBuyPrice(i)} Esencia", $"tienes {g.Inventory.GetCount(i.Id)}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, stock.Count);
        if (choice == 0) return;

        var item = stock[choice - 1];
        int qty = Ui.ReadInt(1, item.MaxStack, "Cantidad");
        int total = shop.GetBuyPrice(item) * qty;
        var result = shop.TryBuy(g.Inventory, g.Wallet, item, qty);
        if (result == TradeResult.Success) Ok($"Compraste {item.Name} x{qty} por {total} Esencia.");
        else Fail(TradeMessage(result));
    }

    private void Sell(Shop shop)
    {
        Ui.Header($"{shop.Name} · Vender", g.Wallet.Essence);
        // Mostramos todo; los no vendibles aparecen con "—" en lugar del precio
        var item = PickFromInventory(_ => true,
            i => i.IsSellable ? $"{shop.GetSellPrice(i)} Esencia c/u" : "—  no vendible");
        if (item == null) return;

        int qty = Ui.ReadInt(1, Math.Max(1, g.Inventory.GetCount(item.Id)), "Cantidad a vender");
        int total = shop.GetSellPrice(item) * qty;
        var result = shop.TrySell(g.Inventory, g.Wallet, item, qty);
        if (result == TradeResult.Success) Ok($"Vendiste {item.Name} x{qty} por {total} Esencia.");
        else Fail(TradeMessage(result));
    }

    // ---------- Guardado ----------

    private void Save()
    {
        SaveSystem.Save(savePath, g);
        Ok($"Partida guardada ({Path.GetFileName(savePath)}).");
    }

    private void Load()
    {
        if (!File.Exists(savePath)) { Fail("No hay partida guardada."); return; }
        try
        {
            SaveSystem.Load(savePath, g);
            _gameOver = false;
            Ok($"Partida cargada. Grupo: {string.Join(", ", g.Party.Members.Select(m => m.Name))}.");
        }
        catch (Exception ex)
        {
            Fail($"Error al cargar: {ex.Message}");
        }
    }

    // ---------- Debug ----------

    private void DebugMenu()
    {
        Ui.Header("Debug", g.Wallet.Essence);
        Ui.Line(1, "Recibir 30 de daño");
        Ui.Line(2, "Envenenar");
        Ui.Line(3, "Curar PV al máximo");
        Ui.Line(4, "+500 de Esencia");
        Ui.Line(5, "Añadir ítem al inventario");
        Ui.Line(6, "Derrotar a un personaje (pierde Solidez)");
        Ui.Line(7, "Reclutar personaje");
        Ui.Line(8, "Activar/desactivar flag del mundo");
        Ui.Back();
        Ui.WriteLine();

        switch (Ui.ReadInt(0, 8))
        {
            case 1: { var c = PickMember("¿A quién?"); if (c != null) { c.TakeDamage(30); Ok($"{c.Name} recibe 30 de daño."); } break; }
            case 2: { var c = PickMember("¿A quién?"); if (c != null) { c.AddStatus(StatusType.Poison, 3); Ok($"{c.Name} está envenenado."); } break; }
            case 3: { var c = PickMember("¿A quién?"); if (c != null) { c.SetHp(c.GetStat(Stat.MaxHp)); Ok($"{c.Name} recupera todos sus PV."); } break; }
            case 4: g.Wallet.Add(500); Ok("+500 de Esencia."); break;
            case 5: DebugAddItem(); break;
            case 6: DebugDefeat(); break;
            case 7: DebugRecruit(); break;
            case 8: DebugToggleFlag(); break;
        }
    }

    private void DebugAddItem()
    {
        Ui.Header("Debug · Añadir ítem", g.Wallet.Essence);
        var all = g.Database.GetAll().OrderBy(i => i.Name).ToList();
        Ui.Rows(all.Select(i => (i.Name, Labels.CategoryName(i.Category), $"tope {i.MaxStack}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, all.Count);
        if (choice == 0) return;

        var item = all[choice - 1];
        int qty = Ui.ReadInt(1, 200, "Cantidad");
        int added = g.Inventory.Add(item, qty);
        if (added == qty) Ok($"Añadidos {item.Name} x{added}.");
        else Warn($"Solo se añadieron {added} de {qty} (tope {item.MaxStack}).");
    }

    private void DebugDefeat()
    {
        var c = PickMember("¿Quién es derrotado?");
        if (c == null) return;

        switch (g.Party.Defeat(c, g.Inventory))
        {
            case DefeatOutcome.SolidityLost:
                Warn($"{c.Name} pierde 1 de Solidez ({c.CurrentSolidity}/{c.GetStat(Stat.MaxSolidity)}) y reemerge con los PV completos.");
                break;
            case DefeatOutcome.MemberErased:
                Fail($"{c.Name} se ha borrado para siempre. Su equipo vuelve a la mochila.");
                break;
            case DefeatOutcome.GameOver:
                Fail($"{c.Name}, el líder, se ha borrado.");
                _gameOver = true;
                break;
        }
    }

    private void DebugRecruit()
    {
        var candidates = g.Roster.Where(r => !g.Party.Members.Contains(r) && !g.Party.Erased.Contains(r.Name)).ToList();
        if (candidates.Count == 0) { Warn("No hay nadie disponible para reclutar."); return; }

        Ui.Header("Debug · Reclutar", g.Wallet.Essence);
        Ui.Rows(candidates.Select(c => (c.Name, Labels.ClassName(c.Class), "")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, candidates.Count);
        if (choice == 0) return;

        var picked = candidates[choice - 1];
        if (g.Party.TryAdd(picked)) Ok($"{picked.Name} se une al grupo.");
        else Warn("El grupo está completo.");
    }

    private void DebugToggleFlag()
    {
        var flags = g.Shops.Where(s => s.UnlockFlag != null).Select(s => s.UnlockFlag!).Distinct().ToList();
        if (flags.Count == 0) { Warn("No hay flags conocidos."); return; }

        Ui.Header("Debug · Flags del mundo", g.Wallet.Essence);
        Ui.Rows(flags.Select(f => (f, g.Flags.Has(f) ? "activo" : "inactivo", "")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, flags.Count);
        if (choice == 0) return;

        var flag = flags[choice - 1];
        if (g.Flags.Has(flag)) g.Flags.Unset(flag); else g.Flags.Set(flag);
        Ok($"{flag}: {(g.Flags.Has(flag) ? "activo" : "inactivo")}.");
    }

    // ---------- Utilidades ----------

    private List<(ItemDefinition Def, int Qty)> GetInventoryEntries(Func<ItemDefinition, bool> filter) =>
        g.Inventory.GetAll()
           .Select(p => (Def: g.Database.Get(p.Key), Qty: p.Value))
           .Where(e => filter(e.Def))
           .OrderBy(e => e.Def.Category).ThenBy(e => e.Def.Name)
           .ToList();

    // Lista numerada del inventario; devuelve la definición elegida o null si cancela
    private ItemDefinition? PickFromInventory(Func<ItemDefinition, bool> filter, Func<ItemDefinition, string>? note = null)
    {
        var entries = GetInventoryEntries(filter);
        if (entries.Count == 0) { Warn("No hay ítems disponibles."); return null; }

        Ui.Rows(entries.Select(e => (e.Def.Name, $"x{e.Qty}", note?.Invoke(e.Def) ?? e.Def.Description)).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, entries.Count);
        return choice == 0 ? null : entries[choice - 1].Def;
    }

    // Si solo hay un miembro lo devuelve directamente
    private Character? PickMember(string prompt)
    {
        var members = g.Party.Members;
        if (members.Count == 0) return null;
        if (members.Count == 1) return members[0];

        Ui.WriteLine();
        Ui.Section(prompt);
        Ui.Rows(members.Select(m => (
            m.Name,
            $"PV {m.CurrentHp}/{m.GetStat(Stat.MaxHp)}",
            $"Solidez {m.CurrentSolidity}/{m.GetStat(Stat.MaxSolidity)}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, members.Count);
        return choice == 0 ? null : members[choice - 1];
    }

    private static string ItemNote(ItemDefinition item)
    {
        var tags = new List<string>();
        if (!item.IsSellable) tags.Add("no vendible");
        if (!item.IsDiscardable) tags.Add("no se puede tirar");
        string tagText = tags.Count == 0 ? "" : $"  [{string.Join(", ", tags)}]";
        return item.Description + tagText;
    }

    private static string TradeMessage(TradeResult result) => result switch
    {
        TradeResult.Success => "Hecho.",
        TradeResult.NotSellable => "Ese ítem no se puede vender.",
        TradeResult.NotDiscardable => "Ese ítem no se puede tirar.",
        TradeResult.NotEnoughItems => "No tienes suficientes.",
        TradeResult.NotEnoughMoney => "No tienes suficiente Esencia.",
        TradeResult.InventoryFull => "No cabe en el inventario (tope de stack).",
        TradeResult.NotInStock => "La tienda no vende eso.",
        TradeResult.NothingToRestore => "La Solidez ya está al máximo.",
        _ => result.ToString()
    };
}
