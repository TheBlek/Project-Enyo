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
    private Buildings building_type = Buildings.building0;

    private float grid;

    private bool building_mode = false;

    private Vector2 input_movement;
    

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

        if (building_mode && (Input.GetKey(KeyCode.LeftShift) ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) )
        {
            builder.Build(building_type, gameManager);
        }else if (Input.GetMouseButtonDown(0))
            shooter.Shoot();

        if (building_mode)
            builder.Preview(player_camera, building_type, grid);

        input_movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        walker.LookAtMouse(player_camera);
        walker.Walk(input_movement);
    }

    private void HandleNumInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            building_type = Buildings.building0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            building_type = Buildings.building1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            building_type = Buildings.Mine;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            building_type = Buildings.Wall;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            building_type = Buildings.Gate;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            building_type = Buildings.building5;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            building_type = Buildings.building6;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            building_type = Buildings.building7;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            building_type = Buildings.building8;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            building_type = Buildings.building9;
        builder.MatchPreviewSize(building_type, grid);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}
