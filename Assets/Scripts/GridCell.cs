using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private Building building_in_cell;

    public void SetBuilding(Building building)
    {
        building_in_cell = building;
    }

    public bool Buildable()
    {
        if (building_in_cell == null)
            return true;
        return false;
    }

    public Building GetBuilding()
    {
        return building_in_cell;
    }
    
}
