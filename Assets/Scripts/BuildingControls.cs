using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject preview_prefab;
    [SerializeField] private Camera player_camera;
    [SerializeField] private GameManager gameManager;

    private float grid;
    private GameObject preview_obj;
    private List<Bounds> buildings_bounds;
    private bool building_mode = false;
    private int building_num = 1;
    private Vector2 shift;

    private void Start()
    {
        grid = gameManager.GetGridSize();
        RecalculateShift();
        buildings_bounds = new List<Bounds>();
        preview_obj = GameObject.Instantiate(preview_prefab);
        preview_obj.SetActive(false);
    }
    void Update()
    {
        HandleInput();

        if (Input.GetKeyDown(KeyCode.E))
        {
            building_mode = !building_mode;
            preview_obj.SetActive(building_mode);
        }


        if (building_mode && Input.GetMouseButtonDown(0))
        {
            Build();
        }

        if (building_mode)
        {
            Preview();
        }
    }

    private void HandleInput()
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
        RecalculateShift();
        preview_obj.GetComponent<PreviewBuilding>().MatchSizeWithBuilding(prefabs[building_num].GetComponent<Building>());
    }

    private void RecalculateShift()
    {
        Vector2 building_size = prefabs[building_num].GetComponent<Building>().GetSize();
        shift = building_size - Vector2.one;
    }

    public void ChangeBuildingNum(int building_number)
    {
        if (building_number > 9 || building_number < 0)
            return;

        building_num = building_number;
    }

    private void Preview()
    {
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2;
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        preview_obj.transform.position = position;
    }

    private void Build()
    {
        Bounds bounds = preview_obj.GetComponent<BoxCollider>().bounds;

        foreach (Bounds building_bounds in buildings_bounds)
        {
            if (building_bounds.Intersects(bounds))
            {
                Debug.Log("Denied");
                return;
            }
        }
        Vector3 position = preview_obj.transform.position;
        GameObject building = GameObject.Instantiate(prefabs[building_num], position, Quaternion.identity);
        buildings_bounds.Add(building.GetComponent<BoxCollider>().bounds);
    }
}
