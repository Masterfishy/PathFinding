using Codice.Client.BaseCommands;
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

    /// <summary>
    /// The searchable map to explore
    /// </summary>
    public SearchableMapUnityContainer SearchableMap;

    public PathRequestEvent PathRequestEvent;

    private SourceEvictingTaskQueue<int, PathRequest> mTaskQueue;
    private bool mIsProcessingTask;
    private PathRequest mCurrentRequest;

    /// <summary>
    /// Request a path from the start position to the end position
    /// </summary>
    /// <param name="request">The path request</param>
    public void RequestPath(PathRequest request)
    {
        Debug.Log($"Requesting Path: {request}");
        // Add it to the queue
        mTaskQueue.Enqueue(request.Source, request);
    }

    /// <summary>
    /// Callback function triggered when a path request task is processed
    /// </summary>
    /// <param name="path"></param>
    public void OnFinishedProcessingRequest(List<ISearchablePosition> path)
    {
        Debug.Log($"Path Finder callback! {mIsProcessingTask}, {mCurrentRequest}");
        if (!mIsProcessingTask || mCurrentRequest == null)
        {
            return;
        }

        bool success = path.Count != 0;

        // Trigger the callback of the path request
        mCurrentRequest.OnPathComplete(path, success);

        mIsProcessingTask = false;
    }

    private void OnEnable()
    {
        mIsProcessingTask = false;
        mCurrentRequest = null;
        mTaskQueue = new();

        PathRequestEvent.OnEventRaised += RequestPath;
    }

    private void OnDisable()
    {
        PathRequestEvent.OnEventRaised -= RequestPath;
    }

    private void Update()
    {
        if (SearchAlgorithm != null && !mIsProcessingTask && !mTaskQueue.IsEmpty())
        {
            // Dequeue
            mCurrentRequest = mTaskQueue.Dequeue();
            Debug.Log($"Processing the request: {mCurrentRequest}");

            // Process the request
            SearchAlgorithm.Contents.FindPath(mCurrentRequest.PathStart, mCurrentRequest.PathEnd, SearchableMap.Contents, OnFinishedProcessingRequest);

            mIsProcessingTask = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(SearchableMap.Contents.Size, SearchableMap.Contents.Size));
    }
}

/// <summary>
/// This internal class represents a request for a path from the start position to the end position.
/// </summary>
public class PathRequest
{
    public int Source;
    public ISearchablePosition PathStart;
    public ISearchablePosition PathEnd;

    /// <summary>
    /// A delegate callback that returns a discovered path and a bool to indicate the success of the request.
    /// </summary>
    public Action<List<ISearchablePosition>, bool> OnPathComplete;

    /// <summary>
    /// Create a path request.
    /// </summary>
    /// <param name="source">The unique identifier for the entity that made the request</param>
    /// <param name="pathStart">The starting position of the path</param>
    /// <param name="pathEnd">The ending position of the path</param>
    /// <param name="onPathComplete">The callback to trigger to return the discovered path</param>
    public PathRequest(int source, ISearchablePosition pathStart, ISearchablePosition pathEnd, Action<List<ISearchablePosition>, bool> onPathComplete)
    {
        Source = source;
        PathStart = pathStart;
        PathEnd = pathEnd;
        OnPathComplete = onPathComplete;
    }

    public override string ToString()
    {
        return $"Request: Source={Source}, Start={PathStart.Position}, End={PathEnd.Position}, Callback={OnPathComplete}";
    }
}
