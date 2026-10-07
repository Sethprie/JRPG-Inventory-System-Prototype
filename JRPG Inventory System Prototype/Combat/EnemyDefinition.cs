namespace JRPG_Inventory_System_Prototype;

// Mínimo necesario para probar el daño. Multiplicador de afinidad: 1.5 débil, 0.5 resiste, 0 inmune, negativo absorbe.
public record EnemyDefinition(string Id, int Defense, int MagicDefense, Dictionary<Element, float> Affinities)
{
    public string DisplayName => Loc.T($"enemy.{Id}.name");
    public float GetAffinity(Element e) => e == Element.None ? 1f : Affinities.GetValueOrDefault(e, 1f);
    public int GetDefense(DamageCategory c) => c == DamageCategory.Physical ? Defense : MagicDefense;
}
