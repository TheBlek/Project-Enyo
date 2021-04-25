using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/AI Behaviours/Enemy Addressing Behaviour")]
public class EnemyAddressingBehaviour : Behaviour
{

    public override bool Trigger(State state)
    {
        foreach (Enemy enemy in state.GetUnits())
        {
            if (enemy.TryGetComponent(out PathFollower follower) && follower.Target == default)
            {
                float[,] player_buildings_map = state.GetMapByType(MapTypes.PlayerBuildings);
                Vector2Int map_size = state.GetMapSize();
                List<Vector2Int> targets = new List<Vector2Int>();
                for (int x = 0; x < map_size.x; x++)
                    for (int y = 0; y < map_size.y; y++)
                        if (player_buildings_map[x, y] == 1f)
                            targets.Add(new Vector2Int(x, y));
                if (targets.Count > 0)
                {
                    Instructions[0].Parameters = new object[] { enemy, targets[Random.Range(0, targets.Count - 1)] };
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

}