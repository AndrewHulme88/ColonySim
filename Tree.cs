using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree : MonoBehaviour
{
    [SerializeField] private JobManager jobManager;
    [SerializeField] private Tilemap groundTilemap;

    private bool isMarkedForChopping;

    public void MarkForChopping()
    {
        if (isMarkedForChopping)
            return;

        isMarkedForChopping = true;

        Vector3Int treePosition = groundTilemap.WorldToCell(transform.position);

        jobManager.CreateJob(treePosition, gameObject, TypeOfJob.ChopTree);
    }

    private void OnMouseDown()
    {
        MarkForChopping();
    }
}
