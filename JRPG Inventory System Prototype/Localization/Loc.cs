using System.Globalization;
using System.Text.Json;

namespace JRPG_Inventory_System_Prototype;

// Tablas de texto por clave (equivalente a los String Tables de Unreal).
// Idioma base: inglés. Si falta una clave en el idioma actual, se usa la del inglés.
public static class Loc
{
    public const string BaseLanguage = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> Tables = new();

    public static string Current { get; private set; } = BaseLanguage;
    public static IEnumerable<string> Languages => Tables.Keys.OrderBy(l => l != BaseLanguage).ThenBy(l => l);

    // Carga todos los .json de la carpeta; el nombre del archivo es el código de idioma (en.json, es.json)
    public static void LoadFromDirectory(string dir)
    {
        foreach (var file in Directory.GetFiles(dir, "*.json"))
        {
            var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file)) ?? new();
            Tables[Path.GetFileNameWithoutExtension(file)] = entries;
        }
        if (!Tables.ContainsKey(BaseLanguage))
            throw new FileNotFoundException($"Falta el idioma base ({BaseLanguage}.json) en {dir}");
    }

    public static void SetLanguage(string language)
    {
        if (Tables.ContainsKey(language)) Current = language;
    }

    public static string NameOf(string language) =>
        Tables.TryGetValue(language, out var t) && t.TryGetValue("language.name", out var name) ? name : language;

    // ¿Existe la clave en el idioma base?
    public static bool Has(string key) => Tables.TryGetValue(BaseLanguage, out var t) && t.ContainsKey(key);

    public static IEnumerable<string> MissingKeys(string language) =>
        Tables[BaseLanguage].Keys.Where(k => !Tables[language].ContainsKey(k));

    // Devuelve el texto con parámetros: T("effect.heal_hp", 50) con plantilla "Restores {0} HP."
    public static string T(string key, params object[] args)
    {
        string? text = null;
        if (!Tables.TryGetValue(Current, out var table) || !table.TryGetValue(key, out text))
            if (Tables.TryGetValue(BaseLanguage, out var baseTable)) baseTable.TryGetValue(key, out text);

        text ??= $"[{key}]"; // clave que falta: visible a propósito para detectarla
        return args.Length == 0 ? text : string.Format(CultureInfo.InvariantCulture, text, args);
    }
}
