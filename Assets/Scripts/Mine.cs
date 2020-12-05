using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Building
{
    private float time_since_last_addition;
    private int MpS = 40;

    public void SelfUpdate()
    {
        Debug.Log("Ok I'm trying");
        time_since_last_addition += Time.deltaTime; 
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            gameManager.AddMetals(MpS);
        }
    }
}
