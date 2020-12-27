using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Wall
{
    [SerializeField] private LayerMask player_mask;
    [SerializeField] private float reaction_time;

    private float time_player_in = 0;

    private BoxCollider2D collider;

    private void Start()
    {
        collider = transform.GetComponent<BoxCollider2D>();
    }


    public override void SelfUpdate(GameManager gameManager)
    {
        if (Physics2D.OverlapCircle(transform.position, gameManager.GetGridSize(), player_mask))
        {
            time_player_in += Time.deltaTime;
        }
        else
        {
            time_player_in = 0;
            collider.isTrigger = false;
        }

        if (time_player_in >= reaction_time)
        {
            collider.isTrigger = true;
        }
    }

}
