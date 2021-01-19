using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

class Superviser : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int number_of_enemies = 4;
    [SerializeField] private float spawn_point_offset = 10;
    [SerializeField] private Vector2 spawn_point;

    private List<Enemy> enemies;
    private List<Enemy> dead_enemies;
    private GameManager gameManager;
    private GridManager gridManager;
    private System.Random random;

    private void Start()
    {
        random = new System.Random();

        gameManager = FindObjectOfType<GameManager>();
        gridManager = gameManager.GetGridManager();

        InitEnemies();
    }

    private void InitEnemies()
    {
        enemies = new List<Enemy>();
        dead_enemies = new List<Enemy>();
        for (int i = 0; i < number_of_enemies; i++)
        {
            enemies.Add(SpawnNewEnemy());
        }
    }

    public void SelfUpdate()
    {
        Vector3 player_pos = gameManager.GetPlayerPosition();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                dead_enemies.Add(enemy);
            else
            {
                if (enemy.GetTarget() == EnemyTargets.player || !gameManager.IsThereAnyBuilding()) // If target is player then set player pos as target
                    enemy.SetTarget(player_pos);
                else
                {
                    if (enemy.IsTargetEleminated && gameManager.IsThereAnyBuilding())
                    {
                        // If previous building or any target was eliminated set new building as target
                        var a = gameManager.GetRandomBuilding();
                        enemy.SetTarget(a.transform.position);
                        enemy.IsTargetEleminated = false;
                    }
                }
                enemy.SelfUpdate();
            }
        }
        foreach (Enemy enemy in dead_enemies)
            HandleVoidEnemy(enemy);
        dead_enemies.Clear();
    }

    private void HandleVoidEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        enemies.Add(SpawnNewEnemy());
    }

    private Enemy SpawnNewEnemy()
    {
        Vector2 relative_pos = spawn_point_offset * new Vector2((float)random.NextDouble(), (float)random.NextDouble());

        while (!gridManager.GetCellFromGlobalPosition(relative_pos + spawn_point).IsWalkable()) // Reroll pos while it's not walkable
            relative_pos = spawn_point_offset * new Vector2((float)random.NextDouble(), (float)random.NextDouble());

        var enemyObj = Instantiate(enemyPrefab, relative_pos + spawn_point, Quaternion.identity);
        return enemyObj.GetComponent<Enemy>();
    }

}
