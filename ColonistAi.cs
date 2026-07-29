using UnityEngine;

public class ColonistAi : MonoBehaviour
{
    [SerializeField] private ColonistMovement movement;
    [SerializeField] private JobManager jobManager;

    private Job currentJob;

    private void Start()
    {
        SeekJob();
    }

    private void SeekJob()
    {
        currentJob = jobManager.GetAvailableJob();

        if (currentJob == null)
            return;

        movement.MoveTo(currentJob.TargetPosition, CompleteCurrentJob);
    }

    private void CompleteCurrentJob()
    {
        if (currentJob == null)
            return;

        jobManager.CompleteJob(currentJob);
        currentJob = null;

        SeekJob();
    }
}
