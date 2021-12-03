using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorGridpoint : MonoBehaviour
{
    public GridManager gridManager;

    public Unit selectedUnit;

    public GridTile cursorTile;
    GridTile lastCursorTile;

    RaycastHit hit;
    Ray ray;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            lastCursorTile = gridManager.GetTileFromWorldPoint(hit.point);
            if (lastCursorTile.walkable)
                cursorTile = lastCursorTile;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            PathRequestManager.RequestPath(selectedUnit.transform.position, cursorTile.worldPosition, selectedUnit.OnPathFound);
        }

        //its currently just there for the gizmos 1/12/21
        gridManager.cursorTile = cursorTile;
    }
}
