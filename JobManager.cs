using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    [SerializeField] private float timer = 5f;

    public event Action JobsAvailable;

    private readonly List<Job> jobs = new();

    private void Awake()
    {
        // Temporary test job until world objects create real jobs
        //jobs.Add(new Job(jobDestination));
    }

    private void Start()
    {
        StartCoroutine(CreateDelayedTestJob());
    }

    private IEnumerator CreateDelayedTestJob()
    {
        CreateJob(new Vector3Int(2, 3, 0));

        yield return new WaitForSeconds(5);

        CreateJob(new Vector3Int(-3, 4, 0));

        yield return new WaitForSeconds(5);

        CreateJob(new Vector3Int(6, -5, 0));
    }

    public void CreateJob(Vector3Int targetPosition)
    {
        jobs.Add(new Job(targetPosition));
        JobsAvailable?.Invoke();
    }

    public Job GetAvailableJob()
    {
        foreach (Job job in jobs)
        {
            if (job.IsReserved || job.IsComplete)
                return null;

            job.IsReserved = true;
            return job;
        }

        return null;
    }

    public void CompleteJob(Job job)
    {
        if (job == null)
            return;

        job.IsComplete = true;
        jobs.Remove(job);
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
