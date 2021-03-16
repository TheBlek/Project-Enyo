using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Mine Building Behaviour")]
public class MineBuildingBehaviour : Behaviour
{
    public override bool Trigger(State state)
    {
        float[,] influence_map = state.GetMapByType(MapTypes.Influence);
        int best_option = 0;
        for (int x = 0; x < influence_map.GetLength(0); x++)
        {
            for (int y = 0; y < influence_map.GetLength(1); y++)
            {
                bool is_influential = influence_map[x, y] == 1f;
                RectInt rect = new RectInt(new Vector2Int(x, y), 2 * Vector2Int.one);
                if (is_influential && state.GetNumberOfMinerals(rect) > best_option && state.IsRectBuildable(rect))
                {
                    best_option = state.GetNumberOfMinerals(rect);
                    Instructions[0].Parameters = new object[] { rect.position, Buildings.Mine };
                    if (best_option == 4)
                        return true;
                }
            }
        }
        if (best_option == 0)
            return false;

        return true;
    }
}
