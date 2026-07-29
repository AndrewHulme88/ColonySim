using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// Generates a pathfinding grid from the Tilemaps and provides A* pathfinding between two positions.

public class PathfindingGrid : MonoBehaviour
{
    /* Uses A* with Manhattan distance.
     * Movement is currently limited to four directions.
     * Pathfinding data is stored directly on PathNodes. This is suitable while only one search is running at a time.
    */

    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap obstacleTilemap;

    private readonly Dictionary<Vector3Int, PathNode> gridNodes = new Dictionary<Vector3Int, PathNode>();

    // Diagonal movement to be potentially added later.
    private static readonly Vector3Int[] NeighbourDirections =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

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

    public List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int targetPosition)
    {
        PathNode startNode = GetNode(startPosition);
        PathNode targetNode = GetNode(targetPosition);

        if (startNode == null || targetNode == null)
            return null;

        if (!startNode.IsWalkable || !targetNode.IsWalkable)
            return null;

        // Reset temporary search data from the previous search.
        ResetNodes();

        List<PathNode> openNodes = new() { startNode };
        HashSet<PathNode> closedNodes = new();

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, targetNode);

        while (openNodes.Count > 0)
        {
            PathNode currentNode = GetLowestCostNode(openNodes);

            // The destination has been reached. Reconstruct the path by following each node's ParentNode back to the start.
            if (currentNode == targetNode)
                return BuildPath(targetNode);

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            foreach (PathNode neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.IsWalkable || closedNodes.Contains(neighbour))
                    continue;

                int tentativeGCost = currentNode.GCost + CalculateDistance(currentNode, neighbour);

                // Found a shorter route to this neighbour
                if (tentativeGCost < neighbour.GCost)
                {
                    neighbour.ParentNode = currentNode;
                    neighbour.GCost = tentativeGCost;
                    neighbour.HCost = CalculateDistance(neighbour, targetNode);

                    if (!openNodes.Contains(neighbour))
                        openNodes.Add(neighbour);
                }    
            }
        }

        // No valid route found
        return null;
    }

    private void ResetNodes()
    {
        foreach (PathNode node in gridNodes.Values)
        {
            node.GCost = int.MaxValue;
            node.HCost = 0;
            node.ParentNode = null;
        }
    }

    // Manhattan distance is used because movement is restricted to horizontal and vertical for now.
    private int CalculateDistance(PathNode first, PathNode second)
    {
        int xDistance = Mathf.Abs(first.GridPosition.x - second.GridPosition.x);
        int yDistance = Mathf.Abs(first.GridPosition.y - second.GridPosition.y);

        return xDistance + yDistance;
    }

    private PathNode GetLowestCostNode(List<PathNode> nodes)
    {
        PathNode lowestCostNode = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            PathNode candidate = nodes[i];

            if (candidate.FCost < lowestCostNode.FCost ||
                candidate.FCost == lowestCostNode.FCost && candidate.HCost < lowestCostNode.HCost)
            {
                lowestCostNode = candidate;
            }
        }

        return lowestCostNode;
    }

    private List<Vector3Int> BuildPath(PathNode targetNode)
    {
        List<Vector3Int> path = new();

        PathNode currentNode = targetNode;

        while (currentNode != null)
        {
            path.Add(currentNode.GridPosition);
            currentNode = currentNode.ParentNode;
        }

        // ParentNode links from the destination back to the start.
        path.Reverse();

        return path;
    }

    private IEnumerable<PathNode> GetNeighbours(PathNode node)
    {
        foreach (Vector3Int direction in NeighbourDirections)
        {
            Vector3Int neighbourPosition = node.GridPosition + direction;
            PathNode neighbour = GetNode(neighbourPosition);

            if (neighbour != null)
                yield return neighbour;
        }
    }
}

public class PathNode
{
    public Vector3Int GridPosition;
    public bool IsWalkable;

    // A* pathfinding values.
    public int GCost; // Distance from start node
    public int HCost; // Distance to end node
    public int FCost => GCost + HCost; // Total cost

    public PathNode ParentNode; // Used to reconstruct the final path once the search completes.
}