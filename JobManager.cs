using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfJob
{
    ChopTree
}

public class JobManager : MonoBehaviour
{
    public event Action JobsAvailable;

    private readonly List<Job> jobs = new();

    public void CreateJob(Vector3Int targetPosition, GameObject targetObject, TypeOfJob jobType)
    {
        jobs.Add(new Job(targetPosition, targetObject, jobType));
        JobsAvailable?.Invoke();
    }

    public Job GetAvailableJob()
    {
        foreach (Job job in jobs)
        {
            if (job.IsReserved)
                continue;

            job.IsReserved = true;
            return job;
        }

        return null;
    }

    public void CompleteJob(Job job)
    {
        if (job == null)
            return;

        jobs.Remove(job);
    }
}

public class Job
{
    public Vector3Int TargetPosition { get; }
    public TypeOfJob JobType { get; }
    public GameObject TargetObject { get; }
    public bool IsReserved { get; set; }


    public Job(Vector3Int targetPosition, GameObject targetObject, TypeOfJob jobType)
    {
        TargetPosition = targetPosition;
        TargetObject = targetObject;
        JobType = jobType;
    }
}
