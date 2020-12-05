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

    private Vector2 CalculateShift(int building_num)
    {
        Vector2 building_size = prefabs[building_num].GetComponent<Building>().GetSize();
        return building_size - Vector2.one;
    }

    public void SwitchPreviewState(bool state)
    {
        preview_obj.SetActive(state);
    }

    public void MatchPreviewSize(int building_num, float grid)
    {
        preview_obj.GetComponent<PreviewBuilding>().MatchSizeWithBuilding(prefabs[building_num].GetComponent<Building>(), grid);
    }

    public void Preview(Camera player_camera, int building_num, float grid)
    {
        Vector2 shift = CalculateShift(building_num);
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        position.x -= position.x % (grid) - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2;
        position.y -= position.y % (grid) - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        preview_obj.transform.position = position;
    }

    public void Build(int building_num, GameManager gameManager)
    {
        if (!gameManager.IsAffordable(prefabs[building_num].GetComponent<Building>()))
            return;

        Bounds bounds = preview_obj.GetComponent<BoxCollider>().bounds;

        if (gameManager.IsIntersectingWithBuildings(bounds))
            return;
        
        Vector3 position = preview_obj.transform.position;
        GameObject building = GameObject.Instantiate(prefabs[building_num], position, Quaternion.identity);

        onBuild(building.GetComponent<Building>());
    }
}
