using UnityEngine;

public class JobManager : MonoBehaviour
{
    [SerializeField] private Vector3Int jobDestination;

    private Job availableJob;

    private void Awake()
    {
        // Temporary test job until world objects create real jobs
        availableJob = new Job(jobDestination);
    }

    public Job GetAvailableJob()
    {
        if (availableJob == null || availableJob.IsReserved || availableJob.IsComplete)
        {
            return null;
        }

        availableJob.IsReserved = true;
        return availableJob;
    }

    public void CompleteJob(Job job)
    {
        if (job == null)
            return;

        job.IsComplete = true;
        job.IsReserved = false;
    }
}

public class Job
{
    public Vector3Int TargetPosition { get; }
    public bool IsReserved { get; set; }
    public bool IsComplete { get; set; }

    public Job(Vector3Int targetPosition)
    {
        TargetPosition = targetPosition;
    }
}
