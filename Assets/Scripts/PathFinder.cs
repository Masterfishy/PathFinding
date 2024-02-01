using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class PathFinder : MonoBehaviour
{
    public MonoBehaviour SearchAlgorithm;
    public ISearchableMap Map;

    private SourceEvictingTaskQueue<int, PathRequest> mTaskQueue;
    private bool mIsProcessingTask;
    private PathRequest mCurrentRequest;

    /// <summary>
    /// Request a path from the start position to the end position
    /// </summary>
    /// <param name="source">The unique identifier of the entity that made the request</param>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position</param>
    /// <param name="callback">The callback to trigger on request completion</param>
    public void RequestPath(int source, ISearchablePosition start, ISearchablePosition end, Action<ISearchablePosition[], bool> callback)
    {
        // Create the PathRequest
        PathRequest newRequest = new(source, start, end, callback);

        // Add it to the queue
        mTaskQueue.Enqueue(source, newRequest);
    }

    /// <summary>
    /// Callback function triggered when a path request task is processed
    /// </summary>
    /// <param name="path"></param>
    public void OnFinishedProcessingRequest(ISearchablePosition[] path)
    {
        if (!mIsProcessingTask || mCurrentRequest == null)
        {
            return;
        }

        bool success = path.Length != 0;

        // Trigger the callback of the path request
        mCurrentRequest.OnPathComplete(path, success);

        mIsProcessingTask = false;
    }

    private void OnValidate()
    {
        if (SearchAlgorithm != null && SearchAlgorithm is not ISearchAlgorithm)
        {
            Debug.LogError($"{SearchAlgorithm.name} does not implement ISearchAlgorithm!");
        }
    }

    private void OnEnable()
    {
        mIsProcessingTask = false;
        mCurrentRequest = null;
        mTaskQueue = new();
    }

    private void Update()
    {
        if (SearchAlgorithm != null && !mIsProcessingTask && !mTaskQueue.IsEmpty())
        {
            // Dequeue
            mCurrentRequest = mTaskQueue.Dequeue();

            // Process the request
            (SearchAlgorithm as ISearchAlgorithm).FindPath(mCurrentRequest.PathStart, mCurrentRequest.PathEnd, Map, OnFinishedProcessingRequest);

            mIsProcessingTask = true;
        }
    }

    /// <summary>
    /// This internal class represents a request for a path from the start position to the end position.
    /// </summary>
    private class PathRequest
    {
        public int Source;
        public ISearchablePosition PathStart;
        public ISearchablePosition PathEnd;

        /// <summary>
        /// A delegate callback that returns a discovered path and a bool to indicate the success of the request.
        /// </summary>
        public Action<ISearchablePosition[], bool> OnPathComplete;

        /// <summary>
        /// Create a path request.
        /// </summary>
        /// <param name="source">The unique identifier for the entity that made the request</param>
        /// <param name="pathStart">The starting position of the path</param>
        /// <param name="pathEnd">The ending position of the path</param>
        /// <param name="onPathComplete">The callback to trigger to return the discovered path</param>
        public PathRequest(int source, ISearchablePosition pathStart, ISearchablePosition pathEnd, Action<ISearchablePosition[], bool> onPathComplete)
        {
            Source = source;
            PathStart = pathStart;
            PathEnd = pathEnd;
            OnPathComplete = onPathComplete;
        }
    }
}
