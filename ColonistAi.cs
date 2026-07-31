using System.Collections;
using UnityEngine;

public class ColonistAi : MonoBehaviour
{
    [SerializeField] private ColonistMovement movement;
    [SerializeField] private JobManager jobManager;

    private Job currentJob;

    private void OnEnable()
    {
        jobManager.JobsAvailable += HandleJobsAvailable;
    }

    private void OnDisable()
    {
        jobManager.JobsAvailable -= HandleJobsAvailable;
    }

    private void Start()
    {
        SeekJob();  
    }

    private void HandleJobsAvailable()
    {
        if (currentJob == null && !movement.isMoving)
            SeekJob();
    }

    private void SeekJob()
    {
        currentJob = jobManager.GetAvailableJob();

        if (currentJob == null)
            return;

        movement.MoveTo(currentJob.TargetPosition, StartWorking);
    }

    private void StartWorking(bool startWorking)
    {
        if (!startWorking)
        {
            currentJob = null;
            SeekJob();
            return;
        }

        StartCoroutine(PerformWork());
    }

    private IEnumerator PerformWork()
    {
        yield return new WaitForSeconds(3f);

        CompleteCurrentJob();
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
