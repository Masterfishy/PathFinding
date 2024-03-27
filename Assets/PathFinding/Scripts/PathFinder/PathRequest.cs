using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a request for path from the PathFinder
/// </summary>
public class PathRequest
{
    public int Id { get; private set; }
    public Vector3 PathStart { get; private set; }
    public Vector3 PathEnd { get; private set; }
    public IPathRequester Requester { get; private set; }

    /// <summary>
    /// A delegate callback that returns a discovered path and a bool to indicate the success of the request.
    /// </summary>
    public Action<PathResponse> OnPathComplete { get; private set; }

    /// <summary>
    /// Create a path request.
    /// </summary>
    /// <param name="pathStart">The starting position of the path</param>
    /// <param name="pathEnd">The ending position of the path</param>
    /// <param name="onPathComplete">The callback to trigger to return the discovered path</param>
    public PathRequest(IPathRequester requester, Vector3 pathStart, Vector3 pathEnd)
    {
        //Id = id;
        PathStart = pathStart;
        PathEnd = pathEnd;
        //OnPathComplete = onPathComplete;
        Requester = requester;
    }
}
