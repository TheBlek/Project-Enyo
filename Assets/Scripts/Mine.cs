using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Building
{
    private int cost = 300;

    private float time_since_last_addition = 0;
    private int MpS = 20;

    public override void SelfUpdate(GameManager gameManager)
    {
        time_since_last_addition += Time.deltaTime;
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            gameManager.AddMetals(MpS);
        }
    }

    public override int GetCost()
    {
        return cost;
    }
}
