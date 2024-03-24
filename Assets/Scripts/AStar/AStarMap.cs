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

    private readonly Dictionary<Vector3Int, AStarPosition> m_Positions = new();

    public int Size => m_Positions.Count;

    /// <summary>
    /// Build the collection of A* positions from the base tilemap
    /// </summary>
    public void BuildMap()
    {
        if (MapData == null)
        {
            Debug.LogWarning($"{name}: No tilemap data set.");
            return;
        }

        // Empty the A* positions
        m_Positions.Clear();

        Vector3Int buildPos = Vector3Int.zero;

        // Add an A* Position for each position in the tilemap
        for (int x = MapData.cellBounds.xMin; x < MapData.cellBounds.xMax; x++)
        {
            for (int y = MapData.cellBounds.yMin; y < MapData.cellBounds.yMax; y++)
            {
                buildPos.x = x; 
                buildPos.y = y;

                if (MapData.HasTile(buildPos))
                {
                    m_Positions.Add(buildPos, new AStarPosition(buildPos));
                }
            }
        }
    }

    public List<Vector3> GetNeighbors(Vector3 position)
    {
        Debug.Log($"Finding neighbors for {position}...");

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

        Debug.Log($"Found {neighbors.Count} neighbor(s)");

        return neighbors;
    }

    /// <summary>
    /// Get the AStarPosition at the given position
    /// </summary>
    /// <param name="position">The vector position</param>
    /// <returns>The AStarPosition at the given position. Returns null if
    /// the position does not exist.</returns>
    public AStarPosition GetPosition(Vector3Int position)
    {
        if (m_Positions.TryGetValue(position, out AStarPosition astarPosition))
        {
            return astarPosition;
        }

        return null;
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
        BuildMap();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(AStarMap))]
public class AStarMapEditor : Editor
{
    SerializedProperty MapDataProp;
    SerializedProperty DiagonalMoveCostProp;
    SerializedProperty AdjacentMoveCostProp;

    private void OnEnable()
    {
        MapDataProp = serializedObject.FindProperty("MapData");
        DiagonalMoveCostProp = serializedObject.FindProperty("DiagonalMoveCost");
        AdjacentMoveCostProp = serializedObject.FindProperty("AdjacentMoveCost");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        AStarMap targetMap = target as AStarMap;
        
        targetMap.MapData = EditorGUILayout.ObjectField("MapData", MapDataProp.objectReferenceValue, typeof(Tilemap), true) as Tilemap;
       
        EditorGUILayout.PropertyField(DiagonalMoveCostProp);
        EditorGUILayout.PropertyField(AdjacentMoveCostProp);

        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild Map"))
        {
            targetMap.BuildMap();

            EditorUtility.SetDirty(targetMap);
        }

        if (targetMap.Size > 0)
        {
            EditorGUILayout.HelpBox("Map Data has been built!", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
