using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This interface defines the API of a search algorithm
/// </summary>
public interface ISearchAlgorithm
{
    /// <summary>
    /// Find a path from the start position to the end position.
    /// </summary>
    /// <param name="pathRequest">The request to find a path for</param>
    /// <param name="callback">The callback to report the found path</param>
    public IEnumerator FindPath(PathRequest pathRequest, Action<PathResponse> callback);
}
