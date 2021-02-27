using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utilities;

public class Mine : Building
{
    private float time_since_last_addition = 0;
    [SerializeField] private int money_per_mineral_tile;
    private int MpS = 0;
    private GameManager gameManager;

    private void Awake()
    {
        var d = transform.GetComponent<Damagable>();
        d.death_offset = .5f;
        d.onKill += PlayBlowAnimation;

        gameManager = FindObjectOfType<GameManager>();
    }

    public override void SelfUpdate()
    {
        time_since_last_addition += Time.deltaTime;
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            gameManager.AddMetals(MpS);
        }
    }

    public override void SetUp()
    {
        base.SetUp();

        IsMineCanBeBuildInRect();
    }

    private void PlayBlowAnimation()
    {
        transform.GetComponent<Animator>().Play("Blow");
    }

    public bool IsMineCanBeBuildInRect(Vector2 position=default)
    {
        if (position == default)
            position = transform.position;

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        MapCell[] cells = gameManager.GetMapManager().GetCellsInRect(position, GetSize());

        CountMinerals(cells);

        return MpS != 0;
    }

    private void CountMinerals(MapCell[] cells)
    {
        MpS = 0;
        foreach (MapCell cell in cells)
        {
            if (cell.Tile == MapTiles.Minerals)
            {
                MpS += money_per_mineral_tile;
            }
        }
    }
}
