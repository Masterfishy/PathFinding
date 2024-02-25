using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="A* Map")]
public class AStarMap : ScriptableObject, ISearchableMap
{
    public GameObject MapGridPrefab;

    private GameObject Map;
    private Tilemap MapTilemap;
    private Dictionary<Vector3Int, AStarPosition> Positions = new();

    [field: Header("Movement Costs")]
    public int DiagonalMoveCost;
    public int AdjacentMoveCost;

    /// <summary>
    /// Instantiate the grid prefab
    /// </summary>
    public void CreateGrid()
    {
        Map = Instantiate(MapGridPrefab);
        if (Map == null)
        {
            Debug.LogError("Failed to instatiate the grid prefab!");
            return;
        }

        Tilemap tm = Map.GetComponentInChildren<Tilemap>();
        if (tm == null)
        {
            Debug.LogError("No tilemap found as a child of the grid prefab!");
            return;
        }

        MapTilemap = tm;

        Vector3Int newPos = Vector3Int.zero;
        BoundsInt bounds = MapTilemap.cellBounds;

        Debug.Log($"Tile map size: {bounds}");

        for (int x = bounds.x; x < bounds.xMax; x++)
        {
            for (int y = bounds.y; y < bounds.yMax; y++)
            {
                newPos.x = x;
                newPos.y = y;

                Positions.Add(newPos, new AStarPosition(newPos));
            }
        }
        
    }

    /// <summary>
    /// Destory the created grid
    /// </summary>
    public void DestroyGrid()
    {
        if (Map != null)
        {
            Destroy(Map);

            Map = null;
            MapTilemap = null;

            Positions.Clear();
        }
    }

    public int Size {
        get
        {
            return Positions.Count;
        }
    }

    public List<ISearchablePosition> MapPositions => Positions.Values.ToList<ISearchablePosition>();

    public List<ISearchablePosition> GetNeighbors(ISearchablePosition position)
    {
        Debug.Log($"Finding neighbors for {position.Position}...");

        Vector3Int locus = Vector3Int.FloorToInt(position.Position);
        Vector3Int searchPos = Vector3Int.zero;
        List<ISearchablePosition> neighbors = new();

        // Search around the position for neighbors
        for (int x = locus.x - 1; x <= locus.x + 1; x++)
        {
            for (int y = locus.y - 1; y <= locus.y + 1; y++)
            {
                searchPos.x = x;
                searchPos.y = y;

                // If we have the neighbor, add it
                if (Positions.TryGetValue(searchPos, out AStarPosition neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        Debug.Log($"Found {neighbors.Count} neighbor(s)");

        return neighbors;
    }

    /// <summary>
    /// Get the AStarPosition at the given position
    /// </summary>
    /// <param name="position">The vector position</param>
    /// <returns>The AStarPosition at the given position. Returns null if the 
    /// position does not exist.</returns>
    public ISearchablePosition GetPosition(Vector3Int position)
    {
        if (Positions.TryGetValue(position, out AStarPosition searchablePosition))
        {
            return searchablePosition;
        }

        return null;
    }

    /// <summary>
    /// Get the cost of traveling from the start position to the end using vector distance
    /// </summary>
    /// <param name="startPos">The starting position</param>
    /// <param name="endPos">The ending position</param>
    /// <returns>The positive integer vector distance between the two positions</returns>
    public int GetTravelCost(ISearchablePosition startPos, ISearchablePosition endPos)
    {
        int xDistance = (int)Mathf.Abs(startPos.Position.x - endPos.Position.x);
        int yDistance = (int)Mathf.Abs(startPos.Position.y - endPos.Position.y);

        if (xDistance > yDistance)
        {
            return DiagonalMoveCost * yDistance + AdjacentMoveCost * (xDistance - yDistance);
        }

        return DiagonalMoveCost * xDistance + AdjacentMoveCost * (yDistance - xDistance);
    }
}
