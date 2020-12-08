using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject preview_prefab;

    public delegate void OnBuild(Building building);
    public OnBuild onBuild = null;

    private GameObject preview_obj;

    private void Start()
    {
        preview_obj = GameObject.Instantiate(preview_prefab);
        preview_obj.SetActive(false);
    }

    private Vector2 CalculateShift(PlayerControls.buildings building_type)
    {
        Vector2 building_size = prefabs[(int)building_type].GetComponent<Building>().GetSize();
        return building_size - Vector2.one;
    }

    public void SwitchPreviewState(bool state)
    {
        preview_obj.SetActive(state);
    }

    public void MatchPreviewSize(PlayerControls.buildings building_type, float grid)
    {
        preview_obj.GetComponent<PreviewBuilding>().MatchSizeWithBuilding(prefabs[(int)building_type].GetComponent<Building>(), grid);
    }

    public void Preview(Camera player_camera, PlayerControls.buildings building_type, float grid)
    {
        Vector2 shift = CalculateShift(building_type);
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2;
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        preview_obj.transform.position = position;
    }

    public void Build(PlayerControls.buildings building_type, GameManager gameManager)
    {
        if (!gameManager.IsAffordable(prefabs[(int)building_type].GetComponent<Building>()))
            return;

        if (IsIntersectingWithBuildings(preview_obj, gameManager))
            return;
        
        Vector3 position = preview_obj.transform.position;
        GameObject building = GameObject.Instantiate(prefabs[(int)building_type], position, Quaternion.identity);

        onBuild(building.GetComponent<Building>());
    }

    public bool IsIntersectingWithBuildings(GameObject obj, GameManager gameManager)
    {
        Bounds bounds = obj.GetComponent<BoxCollider>().bounds;
        var buildings_bounds = gameManager.GetBuildingsBounds();
        foreach (Bounds building_bounds in buildings_bounds)
        {
            if (building_bounds.Intersects(bounds))
                return true;
        }
        return false;
    }
}
