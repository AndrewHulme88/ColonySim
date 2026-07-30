using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfJob
{
    ChopTree
}

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
        //StartCoroutine(CreateDelayedTestJob());
    }

    //private IEnumerator CreateDelayedTestJob()
    //{
    //    CreateJob(new Vector3Int(2, 3, 0));

    //    yield return new WaitForSeconds(2);

    //    CreateJob(new Vector3Int(-3, 4, 0));

    //    yield return new WaitForSeconds(2);

    //    CreateJob(new Vector3Int(6, -5, 0));
    //}

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
