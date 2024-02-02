using Codice.CM.Client.Differences.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="A* Map")]
public class AStarMap : ScriptableObject, ISearchableMap
{
    public int DiagonalMoveCost = 14;
    public int AdjacentMoveCost = 10;

    [SerializeField]
    private int mapSize;
    private AStarPosition[,] mPositions;

    private void OnEnable()
    {
        mPositions = new AStarPosition[mapSize, mapSize];

        int half = mapSize / 2;

        for (int x = -half; x < half; x++)
        {
            for (int y = -half; y < half; y++)
            {
                mPositions[x + half, y + half] = new AStarPosition(new Vector3Int(x, y));
            }
        }
    }

    public int Size => mapSize * mapSize;

    public ISearchablePosition[] MapPositions => null;

    public List<ISearchablePosition> GetNeighbors(ISearchablePosition position)
    {
        int x = (int)position.Position.x;
        int y = (int)position.Position.y;
        int half = mapSize / 2;

        int left = Mathf.Max(-half, x - 1);
        int right = Mathf.Min(half - 1, x + 1);

        int top = Mathf.Max(-half, y - 1);
        int bot = Mathf.Min(half - 1, y + 1);

        List<ISearchablePosition> positions = new();

        for (int _x = left; _x < right; _x++)
        {
            for (int _y = top; _y <= bot; _y++)
            {
                if (_x == x && _y == y)
                {
                    continue;
                }

                Debug.Log($"x:{_x + half}, y:{_y + half}");

                positions.Add(mPositions[_x + half, _y + half]);
            }
        }

        return positions;
    }

    public int GetTravelCost(ISearchablePosition startPos, ISearchablePosition endPos)
    {
        int _distX = (int)Mathf.Abs(startPos.Position.x - endPos.Position.x);
        int _distY = (int)Mathf.Abs(startPos.Position.y - endPos.Position.y);

        if (_distX > _distY)
        {
            return DiagonalMoveCost * _distY + AdjacentMoveCost * (_distX - _distY);
        }

        return DiagonalMoveCost * _distX + AdjacentMoveCost * (_distY - _distX);
    }
}
