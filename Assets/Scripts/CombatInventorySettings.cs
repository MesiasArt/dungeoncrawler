using UnityEngine;

[System.Serializable]
public class CombatItemDefinition
{
    public bool keyItem;
    public string id;
    public string displayName;
    [Min(0)] public int healHP;
    [Min(0)] public int startingQuantity;
    public CombatItemDefinition(string name, int heal, int count)
    { id=System.Guid.NewGuid().ToString("N");displayName=name; healHP=heal; startingQuantity=count; }
}
[CreateAssetMenu(menuName="Dungeon/Combat Inventory Settings")]
public class CombatInventorySettings : ScriptableObject
{
    void OnValidate(){var ids=new System.Collections.Generic.HashSet<string>();foreach(var item in items??new CombatItemDefinition[0])if(item!=null){if(string.IsNullOrEmpty(item.id)||!ids.Add(item.id)){item.id=System.Guid.NewGuid().ToString("N");ids.Add(item.id);}}}
    [Min(1)] public int maximumHP=30;
    public CombatItemDefinition[] items={
        new CombatItemDefinition("Poci\u00f3n peque\u00f1a",25,3),
        new CombatItemDefinition("Poci\u00f3n mediana",50,2),
        new CombatItemDefinition("Poci\u00f3n grande",100,1)
    };
}
