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

    public Building GetBuilding()
    {
        return building_in_cell;
    }
    
}
