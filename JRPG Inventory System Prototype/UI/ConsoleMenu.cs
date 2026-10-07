namespace JRPG_Inventory_System_Prototype;

public partial class ConsoleMenu(GameState g, string savePath)
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
            Ui.MenuRow("Partida", (8, "Guardar"), (9, "Cargar"));
            Ui.MenuRow("Pruebas", (10, "Debug"), (11, "Simular combate"), (12, "Idioma"), (13, "Probar daño"));
            Ui.MenuRow("", (0, "Salir"));
            Ui.WriteLine();

            switch (Ui.ReadInt(0, 13))
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
                case 11: SimulateCombat(); break;
                case 12: LanguageMenu(); break;
                case 13: DamageTest(); break;
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
        PartyRows();
    }

    private void PartyRows()
    {
        foreach (var c in g.Party.Members)
        {
            Ui.Write(c == g.Party.Leader ? "  ★ " : "    ", ConsoleColor.Yellow);
            Ui.Write(c.Name.PadRight(13));
            Ui.Write("PV ", ConsoleColor.DarkGray);
            Ui.HpBar(c.CurrentHealth, c.GetStat(Stat.MaxHealth));
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
            $"PV {c.GetStat(Stat.MaxHealth)} · Magia {c.GetStat(Stat.MaxMagic)} · Ataque {c.GetStat(Stat.AttackPower)} · Def {c.GetStat(Stat.Defense)} · Vel {c.GetStat(Stat.Speed)}"
        )).ToList();
        Ui.Rows(rows, numbered: true);
        Ui.WriteLine();

        int choice = Ui.ReadInt(1, g.Roster.Count);
        g.Party.TryAdd(g.Roster[choice - 1]);
    }

    // ---------- Pantallas de consulta ----------

    private void ShowStatus()
    {
        while (true)
        {
            Ui.Header("Estado del grupo", g.Wallet.Essence);

            foreach (var c in g.Party.Members)
            {
                Ui.Write($"  {c.Name}", ConsoleColor.White);
                Ui.Write($"  ({Labels.ClassName(c.Class)})", ConsoleColor.DarkGray);
                if (c == g.Party.Leader) Ui.Write("  ★ Líder", ConsoleColor.Yellow);
                Ui.WriteLine();

                Ui.Write($"    Nivel      {c.Level}", ConsoleColor.Gray);
                Ui.WriteLine(c.ExperienceToNext > 0 ? $"   XP {c.Experience}/{c.ExperienceToNext}" : "   (máximo)", ConsoleColor.DarkGray);
                Ui.Write("    Vida       ", ConsoleColor.DarkGray);
                Ui.HpBar(c.CurrentHealth, c.GetStat(Stat.MaxHealth), 14);
                Ui.WriteLine();
                Ui.Write("    Magia      ", ConsoleColor.DarkGray);
                Ui.HpBar(c.CurrentMagic, c.GetStat(Stat.MaxMagic), 14);
                Ui.WriteLine();
                Ui.Write("    Solidez    ", ConsoleColor.DarkGray);
                Ui.SolidityPips(c.CurrentSolidity, c.GetStat(Stat.MaxSolidity));
                Ui.WriteLine();

                foreach (var slot in Enum.GetValues<EquipSlot>())
                {
                    var eq = c.Equipped.GetValueOrDefault(slot);
                    Ui.Write($"    {Labels.SlotName(slot),-11}", ConsoleColor.DarkGray);
                    if (eq == null) Ui.WriteLine("—", ConsoleColor.DarkGray);
                    else Ui.WriteLine(eq.DisplayName);
                }

                Ui.Write("    Estados    ", ConsoleColor.DarkGray);
                if (c.Statuses.Count == 0) Ui.WriteLine("ninguno", ConsoleColor.DarkGray);
                else Ui.WriteLine(string.Join(", ", c.Statuses.Select(st => $"{Labels.StatusName(st.Key)} ({st.Value}t)")), ConsoleColor.Yellow);
                Ui.WriteLine();
            }

            if (g.Party.Erased.Count > 0)
                Ui.Dim($"Borrados para siempre: {string.Join(", ", g.Party.Erased)}");

            var members = g.Party.Members;
            Ui.Section("Ficha detallada");
            Ui.Rows(members.Select(m => (m.Name, Labels.ClassName(m.Class), $"Nivel {m.Level}")).ToList(), numbered: true);
            Ui.Back();
            Ui.WriteLine();

            int choice = Ui.ReadInt(0, members.Count);
            if (choice == 0) return;
            CharacterSheet(members[choice - 1]);
        }
    }

    // Ficha con el desglose de cada stat: de dónde sale cada número
    private void CharacterSheet(Character c)
    {
        Ui.Header($"Ficha de {c.Name}", g.Wallet.Essence);
        Ui.Dim($"{Labels.ClassName(c.Class)} · Nivel {c.Level}");
        Ui.WriteLine();

        Ui.Section("Atributos (suben solos con el nivel)");
        foreach (var stat in Enum.GetValues<Stat>().Where(StatFormulas.IsPrimary)) SheetLine(c, stat);
        Ui.WriteLine();

        Ui.Section("Derivados");
        foreach (var stat in Enum.GetValues<Stat>().Where(st => !StatFormulas.IsPrimary(st))) SheetLine(c, stat);
        Ui.Pause();
    }

    private static void SheetLine(Character c, Stat stat)
    {
        var b = c.GetBreakdown(stat);
        Ui.Write($"  {Labels.StatName(stat),-17}", ConsoleColor.White);
        Ui.Write($"{b.Final,4}", ConsoleColor.Yellow);
        Ui.WriteLine($"  = {b.Describe()}", ConsoleColor.DarkGray);
    }

    private void ShowInventory()
    {
        while (true)
        {
            Ui.Header("Inventario", g.Wallet.Essence);

            var entries = GetInventoryEntries(_ => true);
            if (entries.Count == 0) { Ui.Dim("La mochila está vacía."); Ui.Pause(); return; }

            int index = 1;
            foreach (var group in entries.GroupBy(e => e.Def.Category))
            {
                Ui.Section(Labels.CategoryName(group.Key));
                Ui.Rows(group.Select(e => (e.Def.DisplayName, $"x{e.Qty}", ItemNote(e.Def))).ToList(), numbered: true, startIndex: index);
                index += group.Count();
                Ui.WriteLine();
            }

            Ui.Dim("Elige un número para ver el detalle del ítem.");
            Ui.Back();
            Ui.WriteLine();

            int choice = Ui.ReadInt(0, entries.Count);
            if (choice == 0) return;
            ItemDetail(entries[choice - 1].Def);
        }
    }

    // Pantalla de detalle al estilo Dark Souls: efecto (generado) y luego la historia (escrita a mano)
    private void ItemDetail(ItemDefinition item)
    {
        Ui.Header(item.DisplayName, g.Wallet.Essence);
        Ui.Dim($"{Labels.CategoryName(item.Category)}  ·  {Loc.T("ui.base_price", item.BasePrice)}");
        if (ItemTags(item) != "") Ui.Dim(ItemTags(item).Trim());
        if (item is EquipmentItem { RestrictionText: not "" } restricted) Ui.Dim(restricted.RestrictionText);
        Ui.WriteLine();

        if (item.EffectText != "")
        {
            Ui.Section(Loc.T("ui.effect"));
            Ui.Wrap(item.EffectText);
            Ui.WriteLine();
        }

        Ui.Section(Loc.T("ui.story"));
        Ui.Wrap(item.Lore, color: ConsoleColor.Gray);
        Ui.Pause();
    }

    private void LanguageMenu()
    {
        Ui.Header("Idioma / Language", g.Wallet.Essence);

        var languages = Loc.Languages.ToList();
        Ui.Rows(languages.Select(l => (
            Loc.NameOf(l),
            l == Loc.Current ? "● actual" : "",
            l == Loc.BaseLanguage ? "idioma base" : $"{Loc.MissingKeys(l).Count()} claves sin traducir")).ToList(), numbered: true);

        var missingItemText = g.Database.GetAll()
            .Where(i => !Loc.Has($"item.{i.Id}.name") || !Loc.Has($"item.{i.Id}.lore")).ToList();
        Ui.WriteLine();
        if (missingItemText.Count == 0) Ui.Dim("Todos los ítems tienen nombre e historia en el idioma base.");
        else Ui.Warn($"Ítems sin texto en el idioma base: {string.Join(", ", missingItemText.Select(i => i.Id))}");

        Ui.Back();
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, languages.Count);
        if (choice == 0) return;

        Loc.SetLanguage(languages[choice - 1]);
        Ok($"Idioma: {Loc.NameOf(Loc.Current)}.");
    }

    // ---------- Acciones ----------

    private void UseItem()
    {
        Ui.Header("Usar ítem", g.Wallet.Essence);
        var item = PickUsableItem(UseContext.Map);
        if (item == null) return;

        var targets = PickTargets(item);
        if (targets == null) return;

        ReportUse(item, g.Inventory.Use(item, targets, UseContext.Map));
    }

    // Consumibles de la mochila usables en el contexto dado
    private ConsumableItem? PickUsableItem(UseContext context) =>
        PickFromInventory(
            i => i is ConsumableItem c && c.Category == ItemCategory.Consumable && c.UsableIn.HasFlag(context),
            i => $"{Labels.TargetName(((ConsumableItem)i).Target)} · {i.EffectText}") as ConsumableItem;

    // Devuelve los objetivos según el tipo del ítem, o null si se cancela / no está soportado
    private List<Character>? PickTargets(ConsumableItem item)
    {
        switch (item.Target)
        {
            case TargetType.SingleAlly:
                var member = PickMember("¿Sobre quién?");
                return member == null ? null : new List<Character> { member };
            case TargetType.AllAllies:
                return new List<Character>(g.Party.Members);
            default:
                Warn("Los ítems con objetivo enemigo o sin objetivo aún no están en el prototipo.");
                return null;
        }
    }

    private void ReportUse(ConsumableItem item, UseResult result)
    {
        if (result.Check != UseCheck.Ok)
        {
            Warn(UseMessage(result.Check));
            return;
        }
        foreach (var r in result.Results)
        {
            if (r.Applied) Ok(r.Message); else Warn(r.Message);
        }
        Ui.Dim($"Te quedan {g.Inventory.GetCount(item.Id)} de {item.DisplayName}.");
    }

    private static string UseMessage(UseCheck check) => check switch
    {
        UseCheck.NoEffect => "No tendría ningún efecto. No se gastó el ítem.",
        UseCheck.WrongContext => "Ese ítem no se puede usar aquí.",
        UseCheck.NotInStock => "No te quedan de ese ítem.",
        UseCheck.NotUsable => "Ese ítem no se puede usar.",
        _ => "Hecho."
    };

    private void DiscardItem()
    {
        Ui.Header("Tirar ítem", g.Wallet.Essence);
        var item = PickFromInventory(_ => true, i => i.IsDiscardable ? "" : "no se puede tirar");
        if (item == null) return;

        int max = Math.Max(1, g.Inventory.GetCount(item.Id));
        int qty = Ui.ReadInt(1, max, "Cantidad a tirar");
        var result = g.Inventory.TryDiscard(item, qty);
        if (result == TradeResult.Success) Ok($"Tiraste {item.DisplayName} x{qty}.");
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
            else { Ui.Write(eq.DisplayName); Ui.WriteLine($"  ({Labels.Describe(eq)})", ConsoleColor.DarkGray); }
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
        var picked = PickFromInventory(i => i is EquipmentItem,
            i => ((EquipmentItem)i).EffectText + (((EquipmentItem)i).RestrictionText is { Length: > 0 } r ? $"  [{r}]" : ""));
        if (picked is not EquipmentItem item) return;

        if (!item.CanBeUsedBy(member))
        {
            Fail($"{member.Name} no puede equipar {item.DisplayName}. {item.RestrictionText}");
            return;
        }

        // Comprobamos antes que el ítem que se va a reemplazar tenga hueco en la mochila
        var current = member.Equipped.GetValueOrDefault(item.Slot);
        if (current != null && current.Id != item.Id && g.Inventory.GetCount(current.Id) >= current.MaxStack)
        {
            Fail($"No hay espacio en la mochila para {current.DisplayName}.");
            return;
        }

        g.Inventory.Remove(item.Id, 1);
        var previous = member.Equip(item);
        if (previous != null) g.Inventory.Add(previous, 1);

        Ok($"{member.Name} equipa {item.DisplayName}.");
        if (previous != null) Ui.Dim($"{previous.DisplayName} vuelve a la mochila.");
    }

    private void UnequipSlot(Character member)
    {
        var slots = Enum.GetValues<EquipSlot>().Where(member.Equipped.ContainsKey).ToList();
        if (slots.Count == 0) { Warn($"{member.Name} no lleva nada equipado."); return; }

        Ui.Header($"Desequipar a {member.Name}", g.Wallet.Essence);
        Ui.Rows(slots.Select(s => (Labels.SlotName(s), member.Equipped[s].DisplayName, "")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, slots.Count);
        if (choice == 0) return;

        var equipped = member.Equipped[slots[choice - 1]];
        if (g.Inventory.GetCount(equipped.Id) >= equipped.MaxStack)
        {
            Fail($"No hay espacio en la mochila para {equipped.DisplayName}.");
            return;
        }

        var item = member.Unequip(slots[choice - 1])!;
        g.Inventory.Add(item, 1);
        Ok($"{item.DisplayName} vuelve a la mochila.");
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

        Ui.Rows(stock.Select(i => (i.DisplayName, $"{shop.GetBuyPrice(i)} Esencia", $"tienes {g.Inventory.GetCount(i.Id)}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, stock.Count);
        if (choice == 0) return;

        var item = stock[choice - 1];
        int qty = Ui.ReadInt(1, item.MaxStack, "Cantidad");
        int total = shop.GetBuyPrice(item) * qty;
        var result = shop.TryBuy(g.Inventory, g.Wallet, item, qty);
        if (result == TradeResult.Success) Ok($"Compraste {item.DisplayName} x{qty} por {total} Esencia.");
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
        if (result == TradeResult.Success) Ok($"Vendiste {item.DisplayName} x{qty} por {total} Esencia.");
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
        Ui.Line(9, "Dar experiencia");
        Ui.Back();
        Ui.WriteLine();

        switch (Ui.ReadInt(0, 9))
        {
            case 1: { var c = PickMember("¿A quién?"); if (c != null) { c.TakeDamage(30); Ok($"{c.Name} recibe 30 de daño."); } break; }
            case 2: { var c = PickMember("¿A quién?"); if (c != null) { c.AddStatus(StatusType.Poison, 3); Ok($"{c.Name} está envenenado."); } break; }
            case 3: { var c = PickMember("¿A quién?"); if (c != null) { c.SetHealth(c.GetStat(Stat.MaxHealth)); Ok($"{c.Name} recupera todos sus PV."); } break; }
            case 4: g.Wallet.Add(500); Ok("+500 de Esencia."); break;
            case 5: DebugAddItem(); break;
            case 6: DebugDefeat(); break;
            case 7: DebugRecruit(); break;
            case 8: DebugToggleFlag(); break;
            case 9: DebugExperience(); break;
        }
    }

    private void DebugAddItem()
    {
        Ui.Header("Debug · Añadir ítem", g.Wallet.Essence);
        var all = g.Database.GetAll().OrderBy(i => i.DisplayName).ToList();
        Ui.Rows(all.Select(i => (i.DisplayName, Labels.CategoryName(i.Category), $"tope {i.MaxStack}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, all.Count);
        if (choice == 0) return;

        var item = all[choice - 1];
        int qty = Ui.ReadInt(1, 200, "Cantidad");
        int added = g.Inventory.Add(item, qty);
        if (added == qty) Ok($"Añadidos {item.DisplayName} x{added}.");
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

    private void DebugExperience()
    {
        var c = PickMember("¿A quién?");
        if (c == null) return;
        int amount = Ui.ReadInt(1, 100000, "Experiencia");
        int before = c.Level;
        int levels = c.AddExperience(amount);
        Ok($"{c.Name} gana {amount} XP." + (levels > 0 ? $" ¡Sube de nivel {before} → {c.Level}!" : ""));
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
           .OrderBy(e => e.Def.Category).ThenBy(e => e.Def.DisplayName)
           .ToList();

    // Lista numerada del inventario; devuelve la definición elegida o null si cancela
    private ItemDefinition? PickFromInventory(Func<ItemDefinition, bool> filter, Func<ItemDefinition, string>? note = null)
    {
        var entries = GetInventoryEntries(filter);
        if (entries.Count == 0) { Warn("No hay ítems disponibles."); return null; }

        Ui.Rows(entries.Select(e => (e.Def.DisplayName, $"x{e.Qty}", note?.Invoke(e.Def) ?? e.Def.EffectText)).ToList(), numbered: true);
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
            $"PV {m.CurrentHealth}/{m.GetStat(Stat.MaxHealth)}",
            $"Solidez {m.CurrentSolidity}/{m.GetStat(Stat.MaxSolidity)}")).ToList(), numbered: true);
        Ui.Back("Cancelar");
        Ui.WriteLine();

        int choice = Ui.ReadInt(0, members.Count);
        return choice == 0 ? null : members[choice - 1];
    }

    private static string ItemTags(ItemDefinition item)
    {
        var tags = new List<string>();
        if (!item.IsSellable) tags.Add("no vendible");
        if (!item.IsDiscardable) tags.Add("no se puede tirar");
        return tags.Count == 0 ? "" : $"  [{string.Join(", ", tags)}]";
    }

    private static string ItemNote(ItemDefinition item) => item.EffectText + ItemTags(item);

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
