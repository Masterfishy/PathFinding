using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A class that represents a search algorithm implemented with A*
/// </summary>
[CreateAssetMenu(menuName = "A Star/A* Search Algo")]
public class AStarAlgorithm : ScriptableObject, ISearchAlgorithm
{
    public AStarMap AStarMap;

    public float SearchRate;

    [Header("Debug")]
    public Color DebugPathColor;
    public Tilemap DebugTilemap;
    public TileBase DebugTileBase;
    public bool DoDebug;

    /// <summary>
    /// A coroutine to find a path using the A* algorithm from start position to end position
    /// </summary>
    public IEnumerator FindPath(PathRequest pathRequest, Action<PathResponse> callback)
    {
        List<Vector3> path = new();
        bool pathSuccess = false;

        // Get the map's position for start and end
        AStarPosition startPos = AStarMap.ToAStarPosition(Vector3Int.FloorToInt(pathRequest.PathStart));
        AStarPosition endPos = AStarMap.ToAStarPosition(Vector3Int.FloorToInt(pathRequest.PathEnd));

        // If either positions does not exist in the map, end the search
        if (startPos == null || endPos == null)
        {
            CompleteRequest(pathRequest, callback, path, pathSuccess);
            yield break;
        }

        // The openSet stores all discovered nodes with the cheapest one at the top
        MinHeap<AStarPosition> openSet = new(AStarMap.Size * 10);

        // The closedSet stores all the nodes we have visited
        HashSet<AStarPosition> closedSet = new();

        openSet.Push(startPos);

        while (openSet.Count > 0)
        {
            // Find the cheapest node to travel to first
            AStarPosition currentNode = openSet.Pop();
            closedSet.Add(currentNode);

            //DebugPosition(Vector3Int.FloorToInt(currentNode.Position));

            // If we reach our target
            if (currentNode.Equals(endPos))
            {
                pathSuccess = true;
                break;
            }

            // Search for the next best neighbor
            List<Vector3> neighbors = AStarMap.GetNeighbors(currentNode.Position);
            foreach(Vector3 neighborPos in neighbors)
            {
                // Get the AStarPosition from the neighbor positions
                AStarPosition neighbor = AStarMap.ToAStarPosition(neighborPos);

                //Debug.Log($"Neighbor: {neighbor.Position}, Cost: {neighbor.Cost}");
                if (closedSet.Contains(neighbor))
                {
                    // If we have visited this node, continue
                    continue;
                }

                int neighborCost = currentNode.GCost + AStarMap.GetTravelCost(currentNode.Position, neighbor.Position);
                if (neighborCost < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    // Update the neighbor's cost
                    neighbor.GCost = neighborCost;
                    neighbor.HCost = AStarMap.GetTravelCost(neighbor.Position, endPos.Position);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Push(neighbor);
                        //Debug.Log($"Add to openset with {neighbor.Position}");
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                        //Debug.Log($"Update openset with {neighbor.Position}");
                    }
                }
            }

            yield return null;
        }

        if (pathSuccess)
        {
            path = RetracePath(startPos, endPos);
        }

        //Debug.Log($"AStar found a way {pathSuccess}");
        CompleteRequest(pathRequest, callback, path, pathSuccess);
    }

    /// <summary>
    /// Retrace a path from end to start following the position's parents
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position<param>
    /// <returns>An array of positions from start to end</returns>
    private List<Vector3> RetracePath(AStarPosition start, AStarPosition end)
    {
        List<Vector3> path = new();
        AStarPosition currentPos = end;

        // Retrace the path
        while (!start.Equals(currentPos))
        {
            currentPos.GCost = 0;
            currentPos.HCost = 0;

            path.Add(currentPos.Position);
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

    private void CompleteRequest(PathRequest request, Action<PathResponse> callback, List<Vector3> path, bool success)
    {
        PathResponse response = new(request.Id, path, success);
        callback(response);
    }
}
