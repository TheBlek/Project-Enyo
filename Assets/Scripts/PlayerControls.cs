using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{ 
    [SerializeField] private Camera player_camera;
    [SerializeField] private Builder builder;
    [SerializeField] private Shooter shooter;
    private int building_num = 0;

    private bool building_mode = false;
    void Update()
    {
        HandleNumInput();
        if (Input.GetKeyDown(KeyCode.E))
        {
            building_mode = !building_mode;
            builder.SwitchPreviewState(building_mode);
        }

        if (Input.GetMouseButton(0))
        {
            if (building_mode)
                builder.Build(building_num);
            else
                shooter.Shoot();
        }

        if (building_mode)
        {
            builder.Preview(player_camera, building_num);
        }
    }
    public void ChangeBuildingNum(int building_number)
    {
        if (building_number > 9 || building_number < 0)
            return;

        building_num = building_number;
    }

    private void HandleNumInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            building_num = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            building_num = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            building_num = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            building_num = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            building_num = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            building_num = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            building_num = 6;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            building_num = 7;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            building_num = 8;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            building_num = 9;
        builder.MatchPreviewSize(building_num);
    }
}
