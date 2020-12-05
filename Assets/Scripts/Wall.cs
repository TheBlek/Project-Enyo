using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    public override bool IsWalkable()
    {
        return false;
    }
}
