using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Barrack Building Behaviour")]
public class BarrackBuildingBehaviour : Behaviour
{
    [SerializeField] private float _start_value;
    public override bool Trigger(State state)
    {
        if (state.EstimateProfit < 0) return false;

        float[,] influence_map = state.GetMapByType(MapTypes.Influence);
        Vector2Int map_size = state.GetMapSize();
        for (int x = 0; x < map_size.x; x++)
        {
            for (int y = 0; y < map_size.y; y++)
            {
                RectInt rect = new RectInt(x, y, 2, 2);
                if (influence_map[x, y] == 1f && state.IsRectBuildable(rect) && !state.IsMineralsInRect(rect))
                {
                    Instructions[0].Parameters = new object[] { new Vector2Int(x, y), Buildings.Barrack};
                    AssignValue();
                    return true;
                }
            }
        }

        return false;

        void AssignValue()
        {
            if (state.EstimateEarnings == 0) Value = _max_value;
            else Value = SigmoidActivation(state.EstimateSpendings / state.EstimateEarnings);
        }
    }
}