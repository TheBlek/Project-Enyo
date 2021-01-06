﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

class Superviser : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int number_of_enemies;
    [SerializeField] private float offset;

    private List<Enemy> enemies;
    private List<Enemy> dead_enemies;

    private void Awake()
    {
        var random = new System.Random();
        random.Next();

        enemies = new List<Enemy>();
        dead_enemies = new List<Enemy>();
        for (int i = 0; i < number_of_enemies; i++)
        {
            enemies.Add(SpawnNewEnemy());
        }
    }

    public void SelfUpdate(GameManager gameManager)
    {
        Vector3 player_pos = gameManager.GetPlayerPosition();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                dead_enemies.Add(enemy);
            else
            {
                    if (enemy.GetTarget() == EnemyTargets.player) // If target is player then set player pos as target
                        enemy.SetTarget(player_pos);
                    else
                    {
                        if (enemy.IsTargetEleminated)
                        {
                            // If previous building or any target was eliminated set new building as target
                            var a = gameManager.GetRandomBuilding();
                            if (a != null) // If there are some buildings
                            {
                                enemy.SetTarget(a.transform.position);
                            }
                            else
                                enemy.SetTarget(player_pos);
                            enemy.IsTargetEleminated = false;
                        }
                    }
                enemy.SelfUpdate(gameManager);
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
        var enemyObj = Instantiate(enemyPrefab, (Vector3)(new Vector2(UnityEngine.Random.Range(0 - offset, 0 + offset), UnityEngine.Random.Range(0 - offset, 0 + offset))), Quaternion.identity);
        return enemyObj.GetComponent<Enemy>();
    }

}