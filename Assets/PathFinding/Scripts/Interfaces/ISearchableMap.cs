using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This interface defines the API for a map that supports search algorithms
/// </summary>
public interface ISearchableMap 
{
    /// <summary>
    /// Get the neighbors around a given position
    /// </summary>
    /// <param name="position">The position to find neighbors of</param>
    /// <returns>A list of <c>ISearchablePosition</c>s that are neighbors of the given position</returns>
    public List<Vector3> GetNeighbors(Vector3 position);

    /// <summary>
    /// Get the size of the map
    /// </summary>
    public int Size { get; }
}
