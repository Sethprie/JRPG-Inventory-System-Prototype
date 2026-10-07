namespace JRPG_Inventory_System_Prototype;

// De dónde sale cada número: partes sumadas, bono porcentual y resultado final
public record StatBreakdown(Stat Stat, List<(string Label, float Value)> Parts, float Percent, int Final)
{
    public string Describe()
    {
        string sum = string.Join(" + ", Parts.Where(p => p.Value != 0).Select(p => $"{p.Value:0.#} {p.Label}"));
        if (sum == "") sum = "0";
        string text = Percent != 0 ? $"({sum}) × {1 + Percent:0.##}" : sum;
        float raw = Parts.Sum(p => p.Value) * (1 + Percent);
        if (StatFormulas.Caps.TryGetValue(Stat, out int cap) && raw > cap) text += $"  → {Loc.T("breakdown.cap", cap)}";
        return text;
    }
}
