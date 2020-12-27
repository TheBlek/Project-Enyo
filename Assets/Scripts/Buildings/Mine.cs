using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mine : Building
{
    [SerializeField] private Sprite[] sprites;
    private float time_since_last_addition = 0;
    private int MpS = 20;
    private SpriteRenderer renderer;

    private void Start()
    {
        renderer = transform.GetComponent<SpriteRenderer>();
    }

    public override void SelfUpdate(GameManager gameManager)
    {
        time_since_last_addition += Time.deltaTime;
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            gameManager.AddMetals(MpS);
        }
        AdjustSpriteToState();
    }

    private void AdjustSpriteToState()
    {
        int number = (int)( (time_since_last_addition) / 0.1f );
        renderer.sprite = sprites[number];
    }
}
