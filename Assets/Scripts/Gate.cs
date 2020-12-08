using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Building
{
    [SerializeField] private LayerMask player_mask;
    [SerializeField] private float reaction_time;

    private float time_player_in = 0;

    private BoxCollider collider;
    private Material material;

    private void Start()
    {
        collider = transform.GetComponent<BoxCollider>();
        material = transform.GetComponent<Renderer>().material;
    }


    public override void SelfUpdate(GameManager gameManager)
    {
        if (Physics.CheckSphere(transform.position, gameManager.GetGridSize(), player_mask))
            time_player_in += Time.deltaTime;
        else
        {
            time_player_in = 0;
            collider.isTrigger = false;
            material.color = Color.yellow;
        }

        if (time_player_in >= reaction_time)
        {
            collider.isTrigger = true;
            material.color = Color.cyan;
        }
    }

}
