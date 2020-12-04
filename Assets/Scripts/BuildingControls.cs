using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    [SerializeField] private GameObject building_prefab;
    [SerializeField] private GameObject building_preview_prefab;
    [SerializeField] private Camera player_camera;
    [SerializeField] private float grid;

    private GameObject preview_obj;
    private List<Bounds> buildings_bounds;
    private bool building_mode = false;
    private Vector2 shift;

    private void Start()
    {
        Vector2 building_size = building_prefab.GetComponent<Building>().GetSize();
        shift = building_size - Vector2.one;
        buildings_bounds = new List<Bounds>();
        preview_obj = GameObject.Instantiate(building_preview_prefab);
        preview_obj.SetActive(false);
    }
    void Update()
    {
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
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2;
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        GameObject building = GameObject.Instantiate(building_prefab, position, Quaternion.identity);
        Bounds bounds = building.GetComponent<BoxCollider>().bounds;

        foreach (Bounds building_bounds in buildings_bounds)
        {
            if (building_bounds.Intersects(bounds))
            {
                Object.Destroy(building);
                return;
            }
        }
        buildings_bounds.Add(bounds);
    }
}
