using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a response to a path request
/// </summary>
public class PathResponse
{
    public int Source;
    public List<Vector3> Path;
    public bool PathSuccess;

    /// <summary>
    /// A delegate callback that returns a discovered path and a bool to indicate the success of the request.
    /// </summary>
    public Action<List<Vector3>, bool> OnPathComplete;
}
