using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class AStarPosition : ISearchablePosition, IHeapItem<AStarPosition>
{
    public int GCost;
    public int HCost;
    public AStarPosition Parent;
    private Vector3Int mPosition;

    private int mHeapIndex;
    
    public int HeapIndex { get => mHeapIndex; set => mHeapIndex = value; }

    /// <summary>
    /// The total cost of moving to this position
    /// </summary>
    public int Cost => GCost + HCost;

    public Vector3 Position => mPosition;

    /// <summary>
    /// Create an A* position with the given coordinates
    /// </summary>
    /// <param name="position">The position the A* position represents</param>
    public AStarPosition(Vector3Int position)
    {
        GCost = 0;
        HCost = 0;
        Parent = null;

        mPosition = position;
    }

    /// <summary>
    /// Create an A* position with a default position of <c>Vector3Int.zero</c>
    /// </summary>
    public AStarPosition() : this(Vector3Int.zero) { }

    /// <summary>
    /// Compare the costs between this Position and another. Compare the HCost if Cost is the same.
    /// </summary>
    /// <param name="other">The position to compare with</param>
    /// <returns>The </returns>
    public int CompareTo(AStarPosition position)
    {
        int compareResult = position.Cost.CompareTo(Cost);

        if (compareResult == 0)
        {
            compareResult = position.HCost.CompareTo(HCost);
        }    

        return compareResult;
    }
}
