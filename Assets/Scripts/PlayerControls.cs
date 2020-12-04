using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{ 
    [SerializeField] private Camera player_camera;
    [SerializeField] private Builder builder;
    [SerializeField] private Shooter shooter;

    private bool building_mode = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            building_mode = !building_mode;
            builder.SwitchPreviewState(building_mode);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (building_mode)
                builder.Build(player_camera);
            else
                shooter.Shoot();
        }

        if (building_mode)
        {
            builder.Preview(player_camera);
        }
    }
}
