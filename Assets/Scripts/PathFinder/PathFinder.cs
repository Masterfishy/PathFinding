using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a service that provides processing for finding paths
/// from one searchable position to another using a provided search algorithm
/// </summary>
public class PathFinder : MonoBehaviour
{
    /// <summary>
    /// The search algorithm to use for path finding
    /// </summary>
    public SearchAlgorithmUnityContainer SearchAlgorithm;

    public PathRequestEvent PathRequestEvent;

    private SourceEvictingTaskQueue<int, PathRequest> m_TaskQueue;
    private Dictionary<int, Coroutine> m_RunningTasks;

    /// <summary>
    /// Request a path from the start position to the end position
    /// </summary>
    /// <param name="request">The path request</param>
    public void RequestPath(PathRequest request)
    {
        Debug.Log($"Requesting Path: {request}");
        // Add it to the queue
        m_TaskQueue.Enqueue(request.Source, request);
    }

    /// <summary>
    /// Callback function triggered when a path request task is processed
    /// </summary>
    /// <param name="pathResponse">A path result from a search algorithm</param>
    public void OnFinishedProcessingRequest(PathResponse pathResponse)
    {
        Debug.Log($"Path Finder callback! {pathResponse}");
        if (pathResponse == null)
        {
            return;
        }

        // Trigger the callback of the path request
        pathResponse.OnPathComplete(pathResponse.Path, pathResponse.PathSuccess);

        // Stop and remove the request from the running tasks
        StopTask(completedRequest.Source);
        m_RunningTasks.Remove(completedRequest.Source);
    }

    private void OnEnable()
    {
        m_TaskQueue = new();
        m_RunningTasks = new();

        PathRequestEvent.OnEventRaised += RequestPath;
    }

    private void OnDisable()
    {
        PathRequestEvent.OnEventRaised -= RequestPath;

        // Stop all the requests
        foreach(Coroutine task in m_RunningTasks.Values)
        {
            StopCoroutine(task);
        }
    }

    private void Update()
    {
        if (!m_TaskQueue.IsEmpty() && SearchAlgorithm != null)
        {
            // Dequeue
            PathRequest nextRequest = m_TaskQueue.Dequeue();
            Debug.Log($"Processing the request: {nextRequest}");

            // Stop any old tasks for this source
            StopTask(nextRequest.Source);
            m_RunningTasks.Remove(nextRequest.Source);

            // Process the request
            m_RunningTasks[nextRequest.Source] = StartCoroutine(SearchAlgorithm.Contents.FindPath(nextRequest, OnFinishedProcessingRequest));
        }
    }

    /// <summary>
    /// Tries to stop a given task if it is in the m_RunningTasks
    /// </summary>
    /// <param name="source">The source to stop</param>
    private void StopTask(int source)
    {
        if (m_RunningTasks.TryGetValue(source, out Coroutine oldTask))
        {
            if (oldTask == null)
                return;

            StopCoroutine(oldTask);
        }
    }
}
