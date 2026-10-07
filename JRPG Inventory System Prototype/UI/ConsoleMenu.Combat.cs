namespace JRPG_Inventory_System_Prototype;

// Simulación de combate: solo prueba el FLUJO de usar ítems (validar → presentación → efecto → fin de turno).
// No hay enemigos ni daño; los "▸" imitan lo que harían el BattleManager, el Director y la Ability en UE.
public partial class ConsoleMenu
{
    private void SimulateCombat()
    {
        int turn = 1, active = 0;
        bool failCinematic = false;

        while (g.Party.Members.Count > 0)
        {
            if (active >= g.Party.Members.Count) active = 0;
            var actor = g.Party.Members[active];

            Ui.Header($"Combate simulado · Turno {turn}", g.Wallet.Essence);
            Ui.WriteLine();
            PartyRows();
            Ui.Write("  Turno de: ", ConsoleColor.DarkGray);
            Ui.WriteLine(actor.Name, ConsoleColor.White);
            Ui.WriteLine();
            PrintNotices();

            Ui.Line(1, "Ítem");
            Ui.Line(2, "Pasar turno");
            Ui.Line(3, $"Simular fallo de la cinemática: {(failCinematic ? "ACTIVADO" : "desactivado")}");
            Ui.Back("Salir del combate");
            Ui.WriteLine();

            bool turnSpent = false;
            switch (Ui.ReadInt(0, 3))
            {
                case 1: turnSpent = CombatItemAction(actor, failCinematic); break;
                case 2: turnSpent = true; break;
                case 3: failCinematic = !failCinematic; break;
                case 0: return;
            }

            if (turnSpent) { turn++; active++; }
        }
    }

    // Devuelve true si se gastó el turno
    private bool CombatItemAction(Character actor, bool failCinematic)
    {
        Ui.Header($"Ítem · {actor.Name}", g.Wallet.Essence);
        var item = PickUsableItem(UseContext.Combat);
        if (item == null) return false;

        var targets = PickTargets(item);
        if (targets == null) return false;

        // 1) Validación: si no sirve, no se gasta ni el ítem ni el turno
        var check = g.Inventory.CanUse(item, targets, UseContext.Combat);
        if (check != UseCheck.Ok) { Warn(UseMessage(check)); return false; }

        // 2) Presentación simulada
        Ui.Header("Ejecutando acción", g.Wallet.Essence);
        Ui.Dim($"▸ BattleManager → ExecuteAbility(Instigator={actor.Name}, Item={item.Name})");
        Ui.Dim(item.Target == TargetType.SingleAlly
            ? $"▸ Director → BlendToTarget({targets[0].Name}, Ally)"
            : "▸ Director → sin Target único: BlendToCam(PanoramicCam)");

        // El efecto se aplica UNA sola vez, ya sea en el frame del Event Track o en el estado seguro
        UseResult? result = null;
        void ApplyOnce() => result ??= g.Inventory.Use(item, targets, UseContext.Combat);

        if (!failCinematic)
        {
            Ui.Dim("▸ Ability → reproduce la animación de usar ítem");
            Ui.Dim("▸ Event Track → frame del efecto: consumir ítem + aplicar efectos");
            ApplyOnce();
            Ui.Dim("▸ Ability → OnStop");
        }
        else
        {
            Ui.Warn("Fallo simulado de la cinemática (carga o secuencia).");
            Ui.Dim("▸ Ability → estado seguro: aplica el efecto igualmente");
            ApplyOnce();
        }
        Ui.Dim("▸ Ability → OnAbilityFinished → el BattleManager avanza el turno");
        Ui.WriteLine();

        foreach (var r in result!.Results)
        {
            if (r.Applied) Ui.Ok(r.Message); else Ui.Warn(r.Message);
        }
        Ui.Dim($"Te quedan {g.Inventory.GetCount(item.Id)} de {item.Name}.");
        Ui.Pause();
        return true;
    }
}
