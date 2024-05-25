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

    private Dictionary<IPathRequester, RunningRequest> m_Requests;

    /// <summary>
    /// Request a path from the start position to the end position
    /// </summary>
    /// <param name="request">The path request</param>
    public void RequestPath(PathRequest request)
    {
        Debug.Log($"Path requested for: {request.Requester}");

        // Check if we have a running request for the request's id
        if (m_Requests.TryGetValue(request.Requester, out RunningRequest runningRequest))
        {
            // TODO check timestamp for request limiting

            // If we do, stop it
            if (runningRequest.Routine != null)
            {
                StopCoroutine(runningRequest.Routine);
            }
        }

        // Create a running request for the request
        RunningRequest newRequest = new(request);

        // Start the coroutine
        newRequest.Start(StartCoroutine(SearchAlgorithm.Contents.FindPath(request, OnFinishedProcessingRequest)));

        m_Requests[request.Requester] = newRequest;
    }

    /// <summary>
    /// Callback function triggered when a path request task is processed
    /// </summary>
    /// <param name="pathResponse">A path result from a search algorithm</param>
    public void OnFinishedProcessingRequest(PathResponse response)
    {
        Debug.Log($"Path response for: {response.Requester}");

        // Check if we have a running request for the response's id
        if (m_Requests.TryGetValue(response.Requester, out RunningRequest request))
        {
            // Remove the running request
            m_Requests.Remove(request.Requester);

            // Call the requests callback with the response
            request.End(response);
        }
    }

    private void OnEnable()
    {
        m_Requests = new();

        PathRequestEvent.OnEventRaised += RequestPath;
    }

    private void OnDisable()
    {
        PathRequestEvent.OnEventRaised -= RequestPath;

    }

    /// <summary>
    /// An internal class for tracking running requests
    /// </summary>
    private class RunningRequest
    {
        public int Id { get; private set; }
        public Coroutine Routine { get; private set; }
        public double Timestamp { get; private set; }

        public IPathRequester Requester { get; private set; }

        private readonly PathRequest m_Request;

        /// <summary>
        /// Create an unstarted RunningRequest
        /// </summary>
        /// <param name="id">The unique identifier for the request</param>
        /// <param name="request">The path request that is running</param>
        public RunningRequest(PathRequest request)
        {
            //Id = request.Id;
            Requester = request.Requester;
            m_Request = request;
        }

        /// <summary>
        /// Store the started coroutine and mark the time
        /// </summary>
        /// <param name="coroutine">The coroutine this request tracks</param>
        public void Start(Coroutine coroutine)
        {
            Routine = coroutine;
            Timestamp = Time.realtimeSinceStartupAsDouble;
        }

        /// <summary>
        /// Trigger the request's callback
        /// </summary>
        /// <param name="response">The response for the request</param>
        public void End(PathResponse response)
        {
            m_Request.Requester.OnPathFound(response);
        }
    }
}
