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
    [SerializeField] private Builder _builder;
    [SerializeField] private Shooter _shooter;
    [SerializeField] private Walker _walker;
    [SerializeField] private Looker _looker;
    [SerializeField] private Damagable _damagable;
    private GameManager _gameManager;

    private float cell_size;

    private bool building_mode = false;

    private Vector2 input_movement;
    

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        cell_size = _gameManager.GetMapManager().GetCellSize();
    }

    void Update()
    {
        HandleMouseInput();

        if (building_mode)
            _builder.Preview(player_camera, cell_size);

        input_movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuManager.GameIsPaused)
                _gameManager.PauseMenuManager.Resume();
            else
                _gameManager.PauseMenuManager.Pause();
        }
    }

    private void HandleMouseInput()
    {
        bool lShiftInput = Input.GetKey(KeyCode.LeftShift);
        if (building_mode && GetMouseInput(lShiftInput))
        {
            _builder.Build(_gameManager);
            if (!lShiftInput)
                ChangeBuildingStateRequest(ChangeRequestType.Anyway);
        }
        else if (GetMouseInput(false))
            _shooter.Shoot();
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
        _builder.SwitchPreviewState(building_mode);
    }


    private void FixedUpdate()
    {
        _looker.Target = player_camera.ScreenToWorldPoint(Input.mousePosition);
        _walker.Walk(input_movement);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Builder GetBuilder() => _builder;

    public Damagable Damagable => _damagable;

    public Buildings BuildingType
    {
        set
        {
            _builder.SetBuildingType(value);
        }
    }
}
