using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISearchablePosition
{
    /// <summary>
    /// Get the neighbors to this position
    /// </summary>
    /// <returns>An array of neighbors to this position.</returns>
    public ISearchablePosition[] GetNeighbors();
}
