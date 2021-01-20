using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Wall
{
    [SerializeField] private LayerMask player_mask;
    [SerializeField] private float reaction_time = 1;

    private float time_player_in = 0;

    private BoxCollider2D box_collider;

    private void Start()
    {
        box_collider = transform.GetComponent<BoxCollider2D>();
    }


    public override void SelfUpdate()
    {
        if (Physics2D.OverlapCircle(transform.position, gameManager.GetGridManager().GetCellSize(), player_mask))
        {
            time_player_in += Time.deltaTime;
        }
        else
        {
            time_player_in = 0;
            box_collider.isTrigger = false;
        }

        if (time_player_in >= reaction_time)
        {
            box_collider.isTrigger = true;
        }
    }

}
