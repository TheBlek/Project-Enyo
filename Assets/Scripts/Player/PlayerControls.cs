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

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Builder GetBuilder() => builder;
}
