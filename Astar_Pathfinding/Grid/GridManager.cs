using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GridTile[,] grid;

    public LayerMask unwalkableMask;
    public LayerMask highGroundMask;
    public Vector2 gridWorldSize;
    Vector3 gridWorldOffset;

    public float tileDiameter = 1f;
    float tileRadius;
    int gridSizeX, gridSizeZ;

    RaycastHit hit;
    float tileGroundOffset = 0.1f;
    [SerializeField] float gridSpawnHeight = 10;

    [Header("Debug")]
    public bool drawDebugGrid;
    [HideInInspector]public GridTile cursorTile;
    public Color _ColTile, _ColUnwalk, _ColHigh, _ColPath;
    public float gizmoSize = 0.3f;
    [HideInInspector] public List<GridTile> foundPath;

    private void Awake()
    {
        tileRadius = tileDiameter / 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / tileDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.y / tileDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GridTile[gridSizeX, gridSizeZ];

        //this makes it so that the transfrom of the object this script is on will always be in the center of the grid for easy placement in the world
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                //calculates the world position of where each tile needs to be
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * tileDiameter + tileRadius) + Vector3.forward * (z * tileDiameter + tileRadius);
                
                //this raycast allows you to to place the grid over an enviroment where it will place the tiles over all the objects in the map
                if (Physics.Raycast(new Vector3(worldPoint.x, gridSpawnHeight, worldPoint.z), Vector3.down, out hit, 70f))
                {
                    worldPoint.y = hit.point.y + tileGroundOffset;
                }

                bool walkable = !(Physics.CheckSphere(worldPoint, tileRadius, unwalkableMask));
                bool highGround = (Physics.CheckSphere(worldPoint, tileRadius, highGroundMask));
                grid[x, z] = new GridTile(walkable, highGround, worldPoint, x, z);
            }
        }
    }

    public List<GridTile> GetNeighbours(GridTile tile)
    {
        List<GridTile> neighbours = new List<GridTile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = tile.gridX + x;
                int checkZ = tile.gridZ + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbours.Add(grid[checkX, checkZ]);
                }
            }
        }
        return neighbours;
    }

    public GridTile GetTileFromWorldPoint(Vector3 worldPosition)
    {
        gridWorldOffset = transform.position;

        float percentX = (worldPosition.x - gridWorldOffset.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - gridWorldOffset.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX); 
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeZ - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && drawDebugGrid)
        {
            foreach (GridTile n in grid)
            {
                Gizmos.color = _ColTile;
                if (!n.walkable)
                {
                    Gizmos.color = _ColUnwalk;
                }
                else if (n.highGround)
                {
                    Gizmos.color = _ColHigh;
                }

                if (foundPath != null)
                {
                    if (foundPath.Contains(n))
                    {
                        Gizmos.color = _ColPath;
                    }
                }
                if (cursorTile == n)
                {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (tileDiameter - gizmoSize));
            }
        } 
    }

}
