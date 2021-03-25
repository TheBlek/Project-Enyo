using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitsHub")]
public class UnitsHub : ScriptableObject
{
    [HideInInspector] public Unit[] Units;

    public Unit GetUnit(Enemies enemy)
    {
        return Units[(int)enemy];
    }
}