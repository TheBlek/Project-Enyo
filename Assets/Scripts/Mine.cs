﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Building
{
    private float time_since_last_addition = 0;
    private int MpS = 40;

    public override void SelfUpdate()
    {
        time_since_last_addition += Time.deltaTime;
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            gameManager.AddMetals(MpS);
        }
    }
}