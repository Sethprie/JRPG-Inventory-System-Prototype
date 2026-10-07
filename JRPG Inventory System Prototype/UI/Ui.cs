using System.Text;

namespace JRPG_Inventory_System_Prototype;

// Helpers de presentación para la consola (no contienen lógica de juego)
public static class Ui
{
    public const int Width = 64;

    public static void Clear()
    {
        try { if (!Console.IsOutputRedirected) Console.Clear(); }
        catch (IOException) { }
    }

    public static void Write(string text, ConsoleColor? color = null)
    {
        if (color == null) { Console.Write(text); return; }
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color.Value;
        Console.Write(text);
        Console.ForegroundColor = previous;
    }

    public static void WriteLine(string text = "", ConsoleColor? color = null)
    {
        Write(text, color);
        Console.WriteLine();
    }

    // ---------- Estructura ----------

    public static void Banner(string title)
    {
        string line = new('═', Width - 2);
        int left = (Width - 2 - title.Length) / 2;
        WriteLine("╔" + line + "╗", ConsoleColor.Cyan);
        WriteLine("║" + new string(' ', left) + title + new string(' ', Width - 2 - left - title.Length) + "║", ConsoleColor.Cyan);
        WriteLine("╚" + line + "╝", ConsoleColor.Cyan);
    }

    // Cabecera de una pantalla: título a la izquierda, Esencia a la derecha
    public static void Header(string title, int essence)
    {
        Clear();
        string right = $"Esencia: {essence}";
        Write(" " + title.ToUpper(), ConsoleColor.Cyan);
        WriteLine(right.PadLeft(Width - title.Length - 1), ConsoleColor.Yellow);
        WriteLine(new string('═', Width), ConsoleColor.Cyan);
        WriteLine();
    }

    public static void Section(string title)
    {
        Write("── ", ConsoleColor.DarkCyan);
        Write(title, ConsoleColor.DarkCyan);
        WriteLine(" " + new string('─', Math.Max(2, Width - title.Length - 4)), ConsoleColor.DarkCyan);
    }

    // ---------- Mensajes ----------

    public static void Ok(string text) => WriteLine("  ✔ " + text, ConsoleColor.Green);
    public static void Warn(string text) => WriteLine("  ! " + text, ConsoleColor.Yellow);
    public static void Error(string text) => WriteLine("  ✖ " + text, ConsoleColor.Red);
    public static void Dim(string text) => WriteLine("  " + text, ConsoleColor.DarkGray);

    // ---------- Menús ----------

    public static void Key(int number, string text)
    {
        Write($"[{number}]", ConsoleColor.Yellow);
        Write(" " + text);
    }

    // Una fila con grupo y varias opciones: "Grupo   [1] Estado   [3] Usar"
    public static void MenuRow(string group, params (int Number, string Text)[] options)
    {
        Write("  " + group.PadRight(14), ConsoleColor.DarkGray);
        foreach (var (n, text) in options)
        {
            Key(n, text.PadRight(18));
        }
        WriteLine();
    }

    public static void Line(int number, string text) { Write("  "); Key(number, text); WriteLine(); }

    public static void Back(string text = "Volver") => Line(0, text);

    // Filas con columnas alineadas. Si numbered, cada fila lleva [n].
    public static void Rows(IReadOnlyList<(string Name, string Col2, string Note)> rows, bool numbered, int startIndex = 1)
    {
        int nameWidth = rows.Count == 0 ? 0 : rows.Max(r => r.Name.Length) + 2;
        int col2Width = rows.Count == 0 ? 0 : rows.Max(r => r.Col2.Length) + 2;
        for (int i = 0; i < rows.Count; i++)
        {
            Write("  ");
            if (numbered) Write($"[{startIndex + i}] ".PadRight(5), ConsoleColor.Yellow);
            Write(rows[i].Name.PadRight(nameWidth));
            Write(rows[i].Col2.PadRight(col2Width), ConsoleColor.Gray);
            WriteLine(rows[i].Note, ConsoleColor.DarkGray);
        }
    }

    // Imprime un párrafo ajustado al ancho de la pantalla
    public static void Wrap(string text, int indent = 2, ConsoleColor? color = null)
    {
        int width = Width - indent;
        var line = new StringBuilder();
        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.Length > 0 && line.Length + word.Length + 1 > width)
            {
                WriteLine(new string(' ', indent) + line, color);
                line.Clear();
            }
            if (line.Length > 0) line.Append(' ');
            line.Append(word);
        }
        if (line.Length > 0) WriteLine(new string(' ', indent) + line, color);
    }

    public static int ReadInt(int min, int max, string? label = null)
    {
        while (true)
        {
            if (label != null) Dim($"{label} ({min}-{max})");
            Write("  > ", ConsoleColor.Yellow);
            string? line = Console.ReadLine();
            if (line == null) return min; // fin de entrada (si se redirige un archivo)
            if (int.TryParse(line, out int value) && value >= min && value <= max) return value;
            Error("Opción no válida.");
        }
    }

    public static void Pause()
    {
        WriteLine();
        Dim("Pulsa Enter para continuar...");
        Console.ReadLine();
    }

    // ---------- Indicadores ----------

    private static ConsoleColor RatioColor(int cur, int max) =>
        max <= 0 || cur * 4 <= max ? ConsoleColor.Red : cur * 2 <= max ? ConsoleColor.Yellow : ConsoleColor.Green;

    public static void HpBar(int cur, int max, int width = 10)
    {
        int filled = max > 0 ? (int)Math.Round((double)cur / max * width) : 0;
        if (cur > 0 && filled == 0) filled = 1;
        Write(new string('█', filled), RatioColor(cur, max));
        Write(new string('░', width - filled), ConsoleColor.DarkGray);
        Write($" {cur}/{max}".PadRight(9));
    }

    public static void SolidityPips(int cur, int max)
    {
        var color = cur <= 1 ? ConsoleColor.Red : cur * 2 <= max ? ConsoleColor.Yellow : ConsoleColor.Cyan;
        Write(new string('●', cur), color);
        Write(new string('○', Math.Max(0, max - cur)), ConsoleColor.DarkGray);
        Write($" {cur}/{max}");
    }
}
