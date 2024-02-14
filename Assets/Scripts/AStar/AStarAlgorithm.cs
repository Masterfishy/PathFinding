using Codice.Client.BaseCommands;
using Codice.Client.Common.GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A class that represents a search algorithm implemented with A*
/// </summary>
public class AStarAlgorithm : MonoBehaviour, ISearchAlgorithm
{
    private AStarPosition mStartPosition;
    private AStarPosition mEndPosition;
    private AStarMap mMap;
    private Action<List<ISearchablePosition>, bool> mCallback;

    [Header("Debug")]
    public Color DebugPathColor;
    public Tilemap DebugTilemap;
    public TileBase DebugTileBase;
    public bool DoDebug;

    public void FindPath(ISearchablePosition start, ISearchablePosition end, ISearchableMap map, Action<List<ISearchablePosition>, bool> callback)
    {
        mStartPosition = start as AStarPosition;
        mEndPosition = end as AStarPosition;
        mMap = map as AStarMap;
        mCallback = callback;

        DebugTilemap.ClearAllTiles();

        Debug.Log($"Starting search...: Start={mStartPosition.Position}, End={mEndPosition.Position}, Map={mMap}:{mMap.Size}, Callback={mCallback}");

        StopAllCoroutines();
        StartCoroutine(FindPathCoroutine());
    }

    /// <summary>
    /// A coroutine to find a path using the A* algorithm from start position to end position
    /// </summary>
    private IEnumerator FindPathCoroutine()
    {
        List<ISearchablePosition> path = new();
        bool pathSuccess = false;

        // Get the map's position for start and end
        AStarPosition startPos = mMap.GetPosition(Vector3Int.FloorToInt(mStartPosition.Position)) as AStarPosition;
        AStarPosition endPos = mMap.GetPosition(Vector3Int.FloorToInt(mEndPosition.Position)) as AStarPosition;

        // If either positions does not exist in the map, end the search
        if (startPos == null || endPos == null)
        {
            mCallback(path, pathSuccess);
            yield break;
        }

        // The openSet stores all discovered nodes with the cheapest one at the top
        MinHeap<AStarPosition> openSet = new(mMap.Size * 10);

        // The closedSet stores all the nodes we have visited
        HashSet<AStarPosition> closedSet = new();

        openSet.Push(startPos);

        while (openSet.Count > 0)
        {
            // Find the cheapest node to travel to first
            AStarPosition currentNode = openSet.Pop();
            closedSet.Add(currentNode);

            DebugPosition(Vector3Int.FloorToInt(currentNode.Position));

            // If we reach our target
            if (currentNode.Equals(endPos))
            {
                pathSuccess = true;
                break;
            }

            // Search for the next best neighbor
            List<ISearchablePosition> neighbors = mMap.GetNeighbors(currentNode);
            foreach(AStarPosition neighbor in neighbors.Cast<AStarPosition>())
            {
                Debug.Log($"Neighbor: {neighbor.Position}, Cost: {neighbor.Cost}");
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
                    neighbor.HCost = mMap.GetTravelCost(neighbor, endPos);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Push(neighbor);
                        Debug.Log($"Add to openset with {neighbor.Position}");
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                        Debug.Log($"Update openset with {neighbor.Position}");
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        if (pathSuccess)
        {
            path = RetracePath(startPos, endPos);
        }

        Debug.Log($"AStar found a way {pathSuccess}");
        mCallback(path, pathSuccess);
    }

    /// <summary>
    /// Retrace a path from end to start following the position's parents
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position<param>
    /// <returns>An array of positions from start to end</returns>
    private List<ISearchablePosition> RetracePath(AStarPosition start, AStarPosition end)
    {
        List<ISearchablePosition> path = new();
        AStarPosition currentPos = end;

        // Retrace the path
        while (!start.Equals(currentPos))
        {
            currentPos.GCost = 0;
            currentPos.HCost = 0;

            path.Add(currentPos);
            currentPos = currentPos.Parent;
        }

        path.Reverse();

        // TODO simplify the path

        return path;
    }

    /// <summary>
    /// Place a debug tile on the given position
    /// </summary>
    /// <param name="pos">The searched position</param>
    private void DebugPosition(Vector3Int pos)
    {
        Debug.Log($"Explored pos {pos}");
        if (!DoDebug || DebugTilemap == null || DebugTileBase == null)
        {
            return;
        }

        DebugTilemap.SetTile(pos, DebugTileBase);
    }
}
