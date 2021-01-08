using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mine : Building
{
    private float time_since_last_addition = 0;
    private int MpS = 20;
    private GameManager gameManager;

    private void Start()
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

    private void PlayBlowAnimation()
    {
        transform.GetComponent<Animator>().Play("Blow");
    }
}
