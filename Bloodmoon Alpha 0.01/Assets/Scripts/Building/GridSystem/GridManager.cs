using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float CellSise = 1;

    public float sise = 1;
    
    private GridCell[,,] grid;
    private void Start()
    {
        grid = new GridCell[10,10,10];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); x++)
            {
                for (int z = 0; z < grid.GetLength(2); x++)
                {
                    grid[x,y,z] = new();
                }
            }
        }
    }
    public void SetFloor(GameObject Floor, List<Vector3> AllFloorPositions)
    {
        foreach ( var p in AllFloorPositions )
        {
            (int x, int y, int z) = WorldToGridPosition(p);
            grid[x, y, z].PlaceFloor(Floor);
        }
    }
    public bool CanBuild(List<Vector3> AllFloorPositions)
    {
        foreach (var p in AllFloorPositions)
        {
            (int x, int y, int z) = WorldToGridPosition(p);
            if (!grid[x,y,z].IsEmpty()) return false;
        }
        return true;
    }
    private (int x, int y, int z) WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - transform.position).x / CellSise);
        int y = Mathf.FloorToInt((worldPosition - transform.position).y / CellSise);
        int z = Mathf.FloorToInt((worldPosition - transform.position).z / CellSise);
        return (x, y, z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        for (int x = 0; x <= sise; x++)
        {
            for (int y = 0; y <= sise; y++)
            {
                Vector3 start = origin + new Vector3(x * CellSise, y * CellSise, 0);
                Vector3 end = origin + new Vector3(x * CellSise, y * CellSise, sise * CellSise);
                Gizmos.DrawLine(start, end);
            }
        }
        for (int z = 0; z <= sise; z++)
        {
            for (int y = 0; y <= sise; y++)
            {
                Vector3 start = origin + new Vector3(0, y * CellSise, z * CellSise);
                Vector3 end = origin + new Vector3(sise * CellSise, y * CellSise, z * CellSise);
                Gizmos.DrawLine(start, end);
            }
        }
        for (int x = 0; x <= sise; x++)
        {
            for (int z = 0; z <= sise; z++)
            {
                Vector3 start = origin + new Vector3(x * CellSise, 0, z * CellSise);
                Vector3 end = origin + new Vector3(x * CellSise, sise * CellSise, z * CellSise);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}

public class GridCell
{
    private GameObject floor;

    public void PlaceFloor(GameObject floor)
    {
        this.floor = floor;
    }

    public bool IsEmpty()
    {
        return floor == null;
    }
}
