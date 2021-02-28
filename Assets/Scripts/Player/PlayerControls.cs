using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ChangeRequestType
{
    Anyway,
    OnlyIfOff, //off already meaning this request would only turn it on
    OnlyIfOn
}

public class PlayerControls : MonoBehaviour
{ 
    [SerializeField] private Camera player_camera;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Builder builder;
    [SerializeField] private Shooter shooter;
    [SerializeField] private Walker walker;

    private float cell_size;

    private bool building_mode = false;

    private Vector2 input_movement;
    

    private void Start()
    {
        cell_size = gameManager.GetMapManager().GetCellSize();
    }

    void Update()
    {
        //HandleNumInput(); 

        HandleMouseInput();

        if (building_mode)
            builder.Preview(player_camera, cell_size);

        input_movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void HandleMouseInput()
    {
        bool lShiftInput = Input.GetKey(KeyCode.LeftShift);
        if (building_mode && GetMouseInput(lShiftInput))
        {
            builder.Build(gameManager);
            if (!lShiftInput)
                ChangeBuildingStateRequest(ChangeRequestType.Anyway);
        }
        else if (GetMouseInput(false))
            shooter.Shoot();
    }

    private bool GetMouseInput(bool _is_multiple_during_press) => // Freaking long and complex line. Should I divide it?
        (_is_multiple_during_press ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) && !EventSystem.current.IsPointerOverGameObject();

    public void ChangeBuildingStateRequest(ChangeRequestType type)
    {
        
        switch (type)
        {
            case ChangeRequestType.Anyway:
                building_mode = !building_mode;
                break;

            case ChangeRequestType.OnlyIfOff:
                building_mode = true;
                break;

            case ChangeRequestType.OnlyIfOn:
                building_mode = false;
                break;

            default:
                Debug.Log("Somehow this type is not processing");
                break;
        }
        builder.SwitchPreviewState(building_mode);
    }

    public Buildings BuildingType
    {
        set
        {
            builder.SetBuildingType(value);
        }
    }

    private void FixedUpdate()
    {
        walker.LookAtMouse(player_camera);
        walker.Walk(input_movement);
    }

    private void HandleNumInput()
    {
        builder.SetBuildingType(GetBuilding());
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
            return Buildings.Cannon;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            return Buildings.building6;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            return Buildings.building7;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            return Buildings.building8;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            return Buildings.building9;
        return Buildings.AboutToDie; // :) processing as do not change current state
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Builder GetBuilder() => builder;
}
