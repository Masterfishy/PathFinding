using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a request for path from the PathFinder
/// </summary>
public class PathRequest
{
    public int Source;
    public Vector3 PathStart;
    public Vector3 PathEnd;

    /// <summary>
    /// A delegate callback that returns a discovered path and a bool to indicate the success of the request.
    /// </summary>
    public Action<List<Vector3>, bool> OnPathComplete;

    /// <summary>
    /// Create a path request.
    /// </summary>
    /// <param name="source">The unique identifier for the entity that made the request</param>
    /// <param name="pathStart">The starting position of the path</param>
    /// <param name="pathEnd">The ending position of the path</param>
    /// <param name="onPathComplete">The callback to trigger to return the discovered path</param>
    public PathRequest(int source, Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>, bool> onPathComplete)
    {
        Source = source;
        PathStart = pathStart;
        PathEnd = pathEnd;
        OnPathComplete = onPathComplete;
    }

    public override string ToString()
    {
        return $"Request: Source={Source}, Start={PathStart}, End={PathEnd}, Callback={OnPathComplete}";
    }
}
