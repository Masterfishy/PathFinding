using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISearchAlgorithm
{
    /// <summary>
    /// Find a path from the start position to the end position.
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position</param>
    /// <param name="callback">The callback to report the found path</param>
    public void FindPath(ISearchablePosition start, ISearchablePosition end, Action<ISearchablePosition[]> callback);
}
