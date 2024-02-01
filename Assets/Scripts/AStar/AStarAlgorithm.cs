using Codice.Client.BaseCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that represents a search algorithm implemented with A*
/// </summary>
public class AStarAlgorithm : MonoBehaviour, ISearchAlgorithm
{
    private AStarPosition mStartPosition;
    private AStarPosition mEndPosition;
    private AStarMap mMap;
    private Action<AStarPosition[]> mCallback;

    [Header("Debug")]
    public Color DebugPathColor;
    private List<AStarPosition> mDebugPath;

    public void FindPath(ISearchablePosition start, ISearchablePosition end, ISearchableMap map, Action<ISearchablePosition[]> callback)
    {
        mStartPosition = start as AStarPosition;
        mEndPosition = end as AStarPosition;
        mMap = map as AStarMap;
        mCallback = callback;

        StopAllCoroutines();
        StartCoroutine(FindPathCoroutine());
    }

    /// <summary>
    /// A coroutine to find a path using the A* algorithm from start position to end position
    /// </summary>
    private IEnumerator FindPathCoroutine()
    {
        AStarPosition[] path = new AStarPosition[0];
        bool pathSuccess = false;

        // The openSet stores all discovered nodes with the cheapest one at the top
        MinHeap<AStarPosition> openSet = new(mMap.MapPositions.Length);

        // The closedSet stores all the nodes we have visited
        HashSet<AStarPosition> closedSet = new();

        openSet.Push(mStartPosition);

        while (openSet.Count > 0)
        {
            // Find the cheapest node to travel to first
            AStarPosition currentNode = openSet.Pop();
            closedSet.Add(currentNode);

            // If we reach our target
            if (currentNode.Equals(mEndPosition))
            {
                pathSuccess = true;
                break;
            }

            // Search for the next best neighbor
            AStarPosition[] neighbors = mMap.GetNeighbors(currentNode) as AStarPosition[];
            foreach(AStarPosition neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    // If we have visited this node, continue
                    continue;
                }

                int neighborCost = currentNode.GCost + mMap.GetTravelCost(currentNode, neighbor);
                if (neighborCost < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    // Update the neighbor's cost
                    neighbor.GCost = neighborCost;
                    neighbor.HCost = mMap.GetTravelCost(neighbor, mEndPosition);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Push(neighbor);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        if (pathSuccess)
        {
            path = RetracePath(mStartPosition, mEndPosition);
        }

        mCallback(path);
    }

    /// <summary>
    /// Retrace a path from end to start following the position's parents
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position<param>
    /// <returns>An array of positions from start to end</returns>
    private AStarPosition[] RetracePath(AStarPosition start, AStarPosition end)
    {
        List<AStarPosition> path = new();
        AStarPosition currentPos = end;

        // Retrace the path
        while (currentPos != start.Parent)
        {
            path.Add(currentPos);
            currentPos = currentPos.Parent;
        }

        path.Reverse();

        // TODO simplify the path

        mDebugPath = path;

        return path.ToArray();
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            return;
        }

        // Draw the path
        for (int i = 1; i < mDebugPath.Count; i++)
        {
            Gizmos.color = DebugPathColor;
            Gizmos.DrawSphere(mDebugPath[i].Position, 0.05f);
            Gizmos.DrawLine(mDebugPath[i - 1].Position, mDebugPath[i].Position);
        }
    }
}
