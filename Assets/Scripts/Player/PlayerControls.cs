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
    private Buildings building_type = Buildings.building1;

    private float grid;

    private bool building_mode = false;

    private Vector2 input_movement;
    

    private void Start()
    {
        grid = gameManager.GetGridManager().GetCellSize();
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
            builder.Build(building_type);
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
        building_type = GetBuilding();
        builder.MatchPreviewSize(building_type, grid);
    }

    private Buildings GetBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            return Buildings.AboutToDie;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            return Buildings.building1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            return Buildings.Mine;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            return Buildings.Wall;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            return Buildings.Gate;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            return Buildings.building5;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            return Buildings.building6;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            return Buildings.building7;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            return Buildings.building8;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            return Buildings.building9;
        return building_type;
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}
