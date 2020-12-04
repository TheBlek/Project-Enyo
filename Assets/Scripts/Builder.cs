using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject building_prefab;
    [SerializeField] private GameObject building_preview_prefab;
    [SerializeField] private float grid;

    private GameObject preview_obj;
    private List<Bounds> buildings_bounds;

    private void Start()
    {
        buildings_bounds = new List<Bounds>();
        preview_obj = GameObject.Instantiate(building_preview_prefab);
        preview_obj.SetActive(false);
    }

    public void SwitchPreviewState(bool state)
    {
        preview_obj.SetActive(state);
    }

    public void Preview(Camera player_camera)
    {
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x);
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y);

        preview_obj.transform.position = position;
    }

    public void Build(Camera player_camera)
    {
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x);
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y);

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
