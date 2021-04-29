using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Starter Behaviour")]
public class StarterBehaviour : Behaviour
{

    public override bool Trigger(State state)
    {
        float[,] influence_map = state.GetMapByType(MapTypes.Influence);
        foreach (float value in influence_map)
        {
            if (value != 0f)
            {
                return false;
            }
        }

        // copy - paste from MineBuildingBehaviour
        int best_option = 0;
        for (int x = 0; x < influence_map.GetLength(0); x++)
        {
            for (int y = 0; y < influence_map.GetLength(1); y++)
            {
                RectInt rect = new RectInt(new Vector2Int(x, y), 2 * Vector2Int.one);
                if (state.GetNumberOfMinerals(rect) > best_option && state.IsRectBuildable(rect))
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
