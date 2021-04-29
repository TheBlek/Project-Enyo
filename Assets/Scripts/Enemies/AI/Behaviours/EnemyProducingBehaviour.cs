using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Enemy Producing Behaviour")]
public class EnemyProducingBehaviour : Behaviour
{
    [SerializeField] int _optimal_units_number;

    public override bool Trigger(State state)
    {
        Building[] buildings = state.GetBuildingsOfType(Buildings.Barrack);

        foreach (Building building in buildings)
        {
            if (building is Barrack barrack)
            {
                if (barrack.ReadyToProduce)
                {
                    Value = SigmoidActivation(state.GetUnits().Length / (float)_optimal_units_number);
                    Instructions[0].Parameters = new object[] { barrack, Enemies.BlockHead };
                    return true;
                }
            }
        }
        return false;
    }
}