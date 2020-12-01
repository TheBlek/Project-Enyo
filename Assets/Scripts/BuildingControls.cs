using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControls : MonoBehaviour
{
    [SerializeField] private GameObject building_prefab;
    [SerializeField] private GameObject building_preview_prefab;
    [SerializeField] private Camera player_camera;

    private GameObject preview_obj;
    private List<Bounds> buildings_bounds;
    private bool building_mode = false;

    private void Start()
    {
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
        preview_obj.transform.position = position;
    }

    private void Build()
    {
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        GameObject building = GameObject.Instantiate(building_prefab, position, Quaternion.identity);
        Bounds bounds = building.GetComponent<BoxCollider>().bounds;
        bool flag = false;
        for (int i = 0; i < buildings_bounds.Count; i++)
        {
            if (buildings_bounds[i].Intersects(bounds))
            {
                Object.Destroy(building);
                flag = true;
                break;
            }
        }
        if (!flag)
            buildings_bounds.Add(bounds);
    }
}
