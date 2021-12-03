using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    GridManager grid;
    PathRequestManager requestManager;

    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;

    private void Awake()
    {
        grid = GetComponent<GridManager>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSucces = false;

        GridTile startTile = grid.GetTileFromWorldPoint(startPos);
        GridTile targetTile = grid.GetTileFromWorldPoint(targetPos);

        if (startTile.walkable && targetTile.walkable)
        {
            List<GridTile> openSet = new List<GridTile>();
            HashSet<GridTile> closedSet = new HashSet<GridTile>();
            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                GridTile currentTile = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost)
                    {
                        currentTile = openSet[i];
                    }
                }

                openSet.Remove(currentTile);
                closedSet.Add(currentTile);

                if (currentTile == targetTile)
                {
                    pathSucces = true;
                    break;
                }

                foreach (GridTile neighbour in grid.GetNeighbours(currentTile))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentTile.gCost + GetDistance(currentTile, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetTile);
                        neighbour.previousTile = currentTile;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        yield return null; //Null makes it wait 1 frame
        if (pathSucces)
            waypoints = RetracePath(startTile, targetTile);
        requestManager.FinishedProcessingPath(waypoints, pathSucces);
    }

    // this function puts the path in the correct order
    Vector3[] RetracePath(GridTile startTile, GridTile endTile)
    {
        List<GridTile> path = new List<GridTile>();
        GridTile currentTile = endTile;
            
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.previousTile;
        }
        Vector3[] points = WorldPoints(path);
        Array.Reverse(points);
        grid.foundPath = path;
        return points;
    }

    Vector3[] WorldPoints(List<GridTile> path)
    {
        List<Vector3> wPoints = new List<Vector3>();
        foreach (GridTile t in path)
        {
            wPoints.Add(t.worldPosition);
        }

        return wPoints.ToArray();
    }

    int GetDistance(GridTile tileA, GridTile tileB)
    {
        int xDistance = Mathf.Abs(tileA.gridX - tileB.gridX);
        int zDistance = Mathf.Abs(tileA.gridZ - tileB.gridZ);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }



}
