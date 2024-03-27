using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Security;

[CreateAssetMenu(menuName ="A Star/A* Map")]
public class AStarMap : ScriptableObject, ISearchableMap
{
    public Tilemap MapData;

    [field: Header("Movement Costs")]
    public int DiagonalMoveCost;
    public int AdjacentMoveCost;

    public int Size => (int)MapData.size.magnitude;

    public List<Vector3> GetNeighbors(Vector3 position)
    {
        //Debug.Log($"Finding neighbors for {position}...");

        Vector3Int locus = Vector3Int.FloorToInt(position);
        Vector3Int searchPos = Vector3Int.zero;
        List<Vector3> neighbors = new();

        // Search around the position for neighbors
        for (int x = locus.x - 1; x <= locus.x + 1; x++)
        {
            for (int y = locus.y - 1; y <= locus.y + 1; y++)
            {
                searchPos.x = x;
                searchPos.y = y;

                // If we have the neighbor, add it
                if (MapData.HasTile(searchPos))
                {
                    neighbors.Add(searchPos);
                }
            }
        }

        //Debug.Log($"Found {neighbors.Count} neighbor(s)");

        return neighbors;
    }

    /// <summary>
    /// Checks if the given position exists in the map
    /// </summary>
    /// <param name="position">The position to find</param>
    /// <returns>True if the map has the position; false otherwise</returns>
    public bool HasPosition(Vector3 position)
    {
        return MapData.HasTile(Vector3Int.FloorToInt(position));
    }

    /// <summary>
    /// Convert the given position into a point on the map
    /// </summary>
    /// <param name="position">The raw position</param>
    /// <returns>The position on the map</returns>
    public Vector3 ToMapPosition(Vector3 position)
    {
        return Vector3Int.FloorToInt(position);
    }

    /// <summary>
    /// Get the cost of traveling from the start position to the end using vector distance
    /// </summary>
    /// <param name="startPos">The starting position</param>
    /// <param name="endPos">The ending position</param>
    /// <returns>The positive integer vector distance between the two positions</returns>
    public int GetTravelCost(Vector3 startPos, Vector3 endPos)
    {
        int xDistance = (int)Mathf.Abs(startPos.x - endPos.x);
        int yDistance = (int)Mathf.Abs(startPos.y - endPos.y);

        if (xDistance > yDistance)
        {
            return DiagonalMoveCost * yDistance + AdjacentMoveCost * (xDistance - yDistance);
        }

        return DiagonalMoveCost * xDistance + AdjacentMoveCost * (yDistance - xDistance);
    }

    private void OnEnable()
    {
        if (MapData == null)
        {
            Debug.LogWarning("Map Data is not set! Paths will not be able to be found.");
        }
    }
}
