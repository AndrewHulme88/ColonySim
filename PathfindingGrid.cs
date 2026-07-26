using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap obstacleTilemap;

    private readonly Dictionary<Vector3Int, PathNode> gridNodes = new Dictionary<Vector3Int, PathNode>();

    void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridNodes.Clear();

        BoundsInt bounds = obstacleTilemap.cellBounds;

        foreach(Vector3Int position in bounds.allPositionsWithin)
        {
            // Only create nodes where ground actually exists
            if (!groundTilemap.HasTile(position))
                continue;

            bool isWalkable = !obstacleTilemap.HasTile(position);

            gridNodes[position] = new PathNode
            {
                GridPosition = position,
                IsWalkable = isWalkable
            };
        }
    }

    public PathNode GetNode(Vector3Int gridPosition)
    {
        gridNodes.TryGetValue(gridPosition, out PathNode node);
        return node;
    }

    public bool IsWalkable(Vector3Int gridPosition)
    {
        return GetNode(gridPosition)?.IsWalkable == true;
    }
}

public class PathNode
{
    public Vector3Int GridPosition;
    public bool IsWalkable;
    public int gCost; // Distance from start node
    public int hCost; // Distance to end node
    public int fCost => gCost + hCost; // Total cost

    public PathNode parentNode; // For backtracking the final path
}