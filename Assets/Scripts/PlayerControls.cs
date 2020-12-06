using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{ 
    [SerializeField] private Camera player_camera;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Builder builder;
    [SerializeField] private Shooter shooter;
    [SerializeField] private Walker walker;
    private buildings building_type = buildings.building0;

    private float grid;

    private bool building_mode = false;

    private Vector2 input_movement;
    
    public enum buildings
    {
        building0,
        building1,
        mine,
        wall,
        building4,
        building5,
        building6,
        building7,
        building8,
        building9
    }

    private void Start()
    {
        grid = gameManager.GetGridSize();
    }

    void Update()
    {
        HandleNumInput();
        if (Input.GetKeyDown(KeyCode.E))
        {
            building_mode = !building_mode;
            builder.SwitchPreviewState(building_mode);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (building_mode)
                builder.Build(building_type, gameManager);
            else
                shooter.Shoot();
        }

        if (building_mode)
        {
            builder.Preview(player_camera, building_type, grid);
        }

        walker.LookAtMouse(player_camera);
        input_movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        walker.Walk(input_movement);
    }

    private void HandleNumInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            building_type = buildings.building0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            building_type = buildings.building1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            building_type = buildings.mine;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            building_type = buildings.wall;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            building_type = buildings.building4;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            building_type = buildings.building5;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            building_type = buildings.building6;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            building_type = buildings.building7;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            building_type = buildings.building8;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            building_type = buildings.building9;
        builder.MatchPreviewSize(building_type, grid);
    }
}
