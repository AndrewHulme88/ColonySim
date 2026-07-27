using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ColonistMovement : MonoBehaviour
{
    [SerializeField] private PathfindingGrid pathfindingGrid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private float moveSpeed = 3f;

    private Coroutine movementCoroutine;

    public void MoveTo(Vector3Int targetGridPosition)
    {
        Vector3Int startGridPosition = groundTilemap.WorldToCell(transform.position);

        List<Vector3Int> path = pathfindingGrid.FindPath(startGridPosition, targetGridPosition);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"No path found to {targetGridPosition}.");

            return;
        }

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vector3Int> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 targetWorldPosition = groundTilemap.GetCellCenterLocal(path[i]);

            while (Vector3.Distance(transform.position, targetWorldPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, moveSpeed * Time.deltaTime);

                yield return null;
            }

            transform.position = targetWorldPosition;
        }

        movementCoroutine = null;
    }
}
