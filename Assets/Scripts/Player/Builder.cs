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

    private Buildings building_type = Buildings.Mine;

    private void Start()
    {
        preview_obj = GameObject.Instantiate(preview_prefab);
        preview_obj.SetActive(false);
        CorrectPreviewSprite();

        buildings_prefab = new Building[prefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
            buildings_prefab[i] = prefabs[i].GetComponent<Building>();
    }

    private Vector2 CalculateShift()
    {
        Vector2 building_size = prefabs[(int)building_type].GetComponent<Building>().GetSize();
        return building_size - Vector2.one;
    }

    public void SwitchPreviewState(bool state)
    {
        preview_obj.SetActive(state);
    }

    public void CorrectPreviewSprite()
    {
        var renderer = preview_obj.GetComponent<SpriteRenderer>();
        renderer.sprite = prefabs[(int)building_type].GetComponent<SpriteRenderer>().sprite;
        renderer.color = Color.white * 0.5f;
    }

    public void Preview(Camera player_camera, float grid)
    {
        Vector3 shift = CalculateShift();
        Vector3 position = player_camera.ScreenToWorldPoint(Input.mousePosition); // in-game position of mouse
        position.z = 0;
        position.x -= position.x % grid - grid / 2 * Mathf.Sign(position.x) - shift.x * grid / 2; // stick to the grid
        position.y -= position.y % grid - grid / 2 * Mathf.Sign(position.y) + shift.y * grid / 2;

        preview_obj.transform.position = position;
    }

    public void SetBuildingType(Buildings _building_type)
    {
        if (_building_type != Buildings.AboutToDie && building_type != _building_type)
        {
            building_type = _building_type;
            CorrectPreviewSprite();
        }
    }

    public void Build(GameManager gameManager)
    {
        if (!gameManager.IsAffordable(buildings_prefab[(int)building_type]))
            return;

        Vector3 position = preview_obj.transform.position;
        if (!gameManager.GetMapManager().IsRectBuildable(position, buildings_prefab[(int)building_type].GetSize()))
            return;
        
        GameObject building = GameObject.Instantiate(prefabs[(int)building_type], position, Quaternion.identity);

        onBuild(building.GetComponent<Building>());
    }
}
