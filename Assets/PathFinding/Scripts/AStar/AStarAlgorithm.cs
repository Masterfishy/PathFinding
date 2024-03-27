using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A class that represents a search algorithm implemented with A*
/// </summary>
[CreateAssetMenu(menuName = "A Star/A* Search Algo")]
public class AStarAlgorithm : ScriptableObject, ISearchAlgorithm
{
    public AStarMap AStarMap;


    //[Header("Debug")]
    //public Color DebugPathColor;
    //public Tilemap DebugTilemap;
    //public TileBase DebugTileBase;
    //public bool DoDebug;

    /// <summary>
    /// A coroutine to find a path using the A* algorithm from start position to end position
    /// </summary>
    public IEnumerator FindPath(PathRequest pathRequest, Action<PathResponse> callback)
    {
        List<Vector3> path = new();
        bool pathSuccess = false;

        // If either positions does not exist in the map, end the search
        if (!IsRequestValid(pathRequest))
        {
            CompleteRequest(pathRequest, callback, path, pathSuccess);
            yield break;
        }

        // Create the request positions based on the map
        AStarPosition startPos = new(AStarMap.ToMapPosition(pathRequest.PathStart));
        AStarPosition endPos = new(AStarMap.ToMapPosition(pathRequest.PathEnd));

        // The pool of all AStarPositions explored
        Dictionary<Vector3, AStarPosition> positionPool = new();
        
        // The openSet stores all discovered nodes with the cheapest one at the top
        MinHeap<AStarPosition> openSet = new(AStarMap.Size * 10);

        // The closedSet stores all the nodes we have visited
        HashSet<AStarPosition> closedSet = new();

        positionPool.Add(startPos.Position, startPos);
        positionPool.Add(endPos.Position, endPos);

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
                // Get the AStarPosition for the neighbor's position
                if (!positionPool.TryGetValue(neighborPos, out AStarPosition neighbor))
                {
                    neighbor = new AStarPosition(neighborPos);
                    positionPool.Add(neighborPos, neighbor);
                }

                //Debug.Log($"Neighbor: {neighbor.Position}, Cost: {neighbor.Cost}");
                if (closedSet.Contains(neighbor))
                {
                    // If we have visited this node, continue
                    continue;
                }

                int neighborCost = currentNode.GCost + AStarMap.GetTravelCost(currentNode.Position, neighbor.Position);
                // If it's cheaper to travel to this neighbor from the current node or we haven't been to this nieghbor
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

            yield return new WaitForEndOfFrame();
        }

        Debug.Log($"Open set count: {openSet.Count}");

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
    //private void DebugPosition(Vector3Int pos)
    //{
    //    Debug.Log($"Explored pos {pos}");
    //    if (!DoDebug || DebugTilemap == null || DebugTileBase == null)
    //    {
    //        return;
    //    }

    //    DebugTilemap.SetTile(pos, DebugTileBase);
    //}

    private void CompleteRequest(PathRequest request, Action<PathResponse> callback, List<Vector3> path, bool success)
    {
        PathResponse response = new(request, path, success);
        callback(response);
    }

    /// <summary>
    /// Check if the start and end positions are in the map and different
    /// </summary>
    /// <param name="request">The path request</param>
    /// <returns>True if the start and end positions are in the map; false otherwise</returns>
    private bool IsRequestValid(PathRequest request)
    {
        return AStarMap.ToMapPosition(request.PathStart) != AStarMap.ToMapPosition(request.PathEnd) && 
               AStarMap.HasPosition(request.PathStart) && AStarMap.HasPosition(request.PathEnd);
    }
}
