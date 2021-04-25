using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Influence Expander Behaviour")]
public class InfluenceExpanderBehaviour : Behaviour
{

    public override bool Trigger(State state)
    {
        Vector2Int map_size = state.GetMapSize();
        float best_option = Mathf.Infinity;
        Vector2Int result = default;
        float[,] influence_map = state.GetMapByType(MapTypes.Influence);
        float[,] mineral_map = state.GetMapByType(MapTypes.Mineral);
        float[,] buildability_map = state.GetMapByType(MapTypes.Buildability);
        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                bool better_option = mineral_map[x, y] != 1f && mineral_map[x, y] < best_option;
                if (better_option && influence_map[x, y] == 1f && buildability_map[x, y] == 1f)
                {
                    best_option = mineral_map[x, y];
                    result = new Vector2Int(x, y);
                }
            }
        }
        if (result != default)
        {
            Instructions[0].Parameters = new object[] { result, Buildings.InfluenceExpander };
            AssignValue();
            return true;
        }

        return false;

        void AssignValue()
        {
            if (state.EstimateEarnings == 0) Value = _max_value;
            else Value = SigmoidActivation(state.EstimateProfit / state.EstimateEarnings);
        }
    }

}
