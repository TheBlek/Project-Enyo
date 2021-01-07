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
    private Building[] buildings_prefab;

    private void Start()
    {
        preview_obj = GameObject.Instantiate(preview_prefab);
        preview_obj.SetActive(false);

        buildings_prefab = new Building[prefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
            buildings_prefab[i] = prefabs[i].GetComponent<Building>();
    }

    private Vector2 CalculateShift(Buildings building_type)
    {
        Vector2 building_size = prefabs[(int)building_type].GetComponent<Building>().GetSize();
        return building_size - Vector2.one;
    }

    public void SwitchPreviewState(bool state)
    {
        preview_obj.SetActive(state);
    }

    public void MatchPreviewSize(Buildings building_type, float grid)
    {
        preview_obj.GetComponent<PreviewBuilding>().MatchSizeWithBuilding(prefabs[(int)building_type].GetComponent<Building>(), grid);
    }

    public void Preview(Camera player_camera, Buildings building_type, float grid)
    {
        Vector3 shift = CalculateShift(building_type);
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % grid - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2;
        position.y -= position.y % grid - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        preview_obj.transform.position = position;
    }

    public void Build(Buildings building_type, GameManager gameManager)
    {
        if (!gameManager.IsAffordable(buildings_prefab[(int)building_type]))
            return;

        Vector3 position = preview_obj.transform.position;
        if (!gameManager.IsRectBuildable(position, buildings_prefab[(int)building_type].GetSize()))
            return;
        
        GameObject building = GameObject.Instantiate(prefabs[(int)building_type], position, Quaternion.identity);

        onBuild(building.GetComponent<Building>());
    }
}
