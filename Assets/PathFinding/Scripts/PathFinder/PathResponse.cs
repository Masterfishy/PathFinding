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
    public IPathRequester Requester { get; private set; }

    /// <summary>
    /// Create a path response
    /// </summary>
    /// <param name="request">The request this response is for</param>
    /// <param name="path">The path requested</param>
    /// <param name="pathSuccess">True if a path was found; false otherwise</param>
    public PathResponse(PathRequest request, List<Vector3> path, bool pathSuccess)
    {
        //Id = id;
        Requester = request.Requester;
        Path = path;
        PathSuccess = pathSuccess;
    }
}
