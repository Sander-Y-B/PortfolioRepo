using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile
{
    public bool walkable, highGround;
    public Vector3 worldPosition;
    public int gridX, gridZ;
    public int gCost, hCost;

    public GridTile previousTile;

    // something for yerio
    public bool selected = false;

    public GridTile(bool _walkable, bool _highGround, Vector3 _worldPosition, int _gridX, int _gridZ)
    {
        walkable = _walkable;
        highGround = _highGround;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridZ = _gridZ;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
