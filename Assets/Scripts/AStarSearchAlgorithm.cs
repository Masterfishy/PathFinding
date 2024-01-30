using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that represents a search algorithm implemented with A*
/// </summary>
public class AStarSearchAlgorithm : MonoBehaviour, ISearchAlgorithm
{
    private ISearchablePosition mStartPosition;
    private ISearchablePosition mEndPosition;
    private Action<ISearchablePosition[]> mCallback;

    public void FindPath(ISearchablePosition start, ISearchablePosition end, Action<ISearchablePosition[]> callback)
    {
        mStartPosition = start;
        mEndPosition = end;
        mCallback = callback;

        StopAllCoroutines();
        StartCoroutine(FindPathCoroutine());
    }

    /// <summary>
    /// A coroutine to find a path using the A* algorithm from start position to end position
    /// </summary>

    private IEnumerator FindPathCoroutine()
    {
        ISearchablePosition[] path = new ISearchablePosition[0];
        bool pathSuccess = false;

        

        yield return new WaitForEndOfFrame();

        mCallback(path);
    }
}
