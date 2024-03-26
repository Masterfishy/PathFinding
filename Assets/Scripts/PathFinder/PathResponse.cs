using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a response to a path request
/// </summary>
public class PathResponse
{
    public int Id { get; private set; }
    public List<Vector3> Path {  get; private set; }
    public bool PathSuccess { get; private set; }

    /// <summary>
    /// Create a path response
    /// </summary>
    /// <param name="id">The identifier for the response to be associated with a request</param>
    /// <param name="path">The path requested</param>
    /// <param name="pathSuccess">True if a path was found; false otherwise</param>
    public PathResponse(int id, List<Vector3> path, bool pathSuccess)
    {
        Id = id;
        Path = path;
        PathSuccess = pathSuccess;
    }
}
