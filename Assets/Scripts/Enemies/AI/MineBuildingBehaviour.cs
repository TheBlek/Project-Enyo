using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Mine Building Behaviour")]
public class MineBuildingBehaviour : Behaviour
{
    public override bool Trigger(State state)
    {
        float[,] influence_map = state.GetMapByType(MapTypes.Influence);
        for (int x = 0; x < influence_map.GetLength(0); x++)
        {
            for (int y = 0; y < influence_map.GetLength(1); y++)
            {
                bool any_mineral = state.IsMineralsInRect(new Vector2Int(x, y), 2 * Vector2Int.one);
                if (any_mineral && influence_map[x, y] == 1f && state.IsRectBuildable(new Vector2Int(x, y), 2 * Vector2Int.one))
                {
                    Instructions[0].Parameters = new object[] { new Vector2Int(x, y), Buildings.Mine };
                    return true;
                }
            }
        }

        return false;
    }
}
