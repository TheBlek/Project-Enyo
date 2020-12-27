using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Building building_in_cell { get; set; }

    public bool Buildable()
    {
        return building_in_cell == null;
    }
}
