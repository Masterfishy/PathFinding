using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="A* Map")]
public class AStarMap : ScriptableObject, ISearchableMap
{
    public ISearchablePosition[] MapPositions => throw new System.NotImplementedException();

    public ISearchablePosition[] GetNeighbors(ISearchablePosition position)
    {
        throw new System.NotImplementedException();
    }

    public int GetTravelCost(ISearchablePosition startPos, ISearchablePosition endPos)
    {
        throw new System.NotImplementedException();
    }
}
