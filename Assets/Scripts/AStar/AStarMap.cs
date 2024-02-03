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

    /// <summary>
    /// Instantiate the grid prefab
    /// </summary>
    public void CreateGrid()
    {
        Map = Instantiate(MapGridPrefab);
        if (Map != null)
        {
            if (Map.TryGetComponent(out Tilemap tm))
            {
                MapTilemap = tm;

                Vector3Int newPos = Vector3Int.zero;
                BoundsInt bounds = MapTilemap.cellBounds;
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
        }
    }

    public int Size {
        get
        {
            if (MapTilemap == null)
                return 0;

            return (int)MapTilemap.size.magnitude;
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
    /// Get the cost of traveling from the start position to the end using vector distance
    /// </summary>
    /// <param name="startPos">The starting position</param>
    /// <param name="endPos">The ending position</param>
    /// <returns>The positive integer vector distance between the two positions</returns>
    public int GetTravelCost(ISearchablePosition startPos, ISearchablePosition endPos)
    {        
        return (int)Mathf.Abs(Vector3.Distance(startPos.Position, endPos.Position));
    }
}
