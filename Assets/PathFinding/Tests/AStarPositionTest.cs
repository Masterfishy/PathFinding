using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class AStarPositionTest 
{
    [Test]
    public void TestAStarPosition()
    {
        // ARRANGE
        AStarPosition[] positions = new AStarPosition[2];
        Dictionary<Vector3, AStarPosition> positionDict = new();
        MinHeap<AStarPosition> minHeap = new(2);
        HashSet<AStarPosition> set = new();
        AStarPosition pos = new(new Vector3(1, 1, 1));

        positionDict.Add(pos.Position, pos);
        positions[0] = pos;
        minHeap.Push(pos);

        set.Add(pos);

        // ACT
        pos.GCost = 100;
        minHeap.UpdateItem(pos);
        if (!positionDict.TryGetValue(new Vector3(0, 1, 0), out AStarPosition newPos))
        {
            newPos = new(new Vector3(0, 1, 0));
            positionDict.Add(new Vector3(0, 1, 0), newPos);
        }
        minHeap.Push(newPos);
        positions[1] = newPos;
        set.Add(newPos);

        newPos.GCost = 200;
        minHeap.UpdateItem(newPos);

        newPos = new(Vector3.down);
        newPos.GCost = 300;


        // ASSERT
        Assert.That(positions[0].GCost, Is.EqualTo(100));
        Assert.That(positionDict[new Vector3(1, 1, 1)].GCost, Is.EqualTo(100));

        Assert.That(positions[1].GCost, Is.EqualTo(200));
        Assert.That(positionDict[new Vector3(0, 1, 0)].GCost, Is.EqualTo(200));

        Assert.That(positions[1], Is.EqualTo(positionDict[new Vector3(0, 1, 0)]));
        Assert.That(positions[0], Is.Not.EqualTo(positionDict[new Vector3(0, 1, 0)]));

        Assert.That(minHeap.Contains(pos), Is.True);
        Assert.That(minHeap.Pop(), Is.EqualTo(pos));

        Assert.That(set.Contains(positions[0]), Is.True);
        Assert.That(set.Contains(positionDict[new Vector3(0, 1, 0)]), Is.True);

        Assert.That(positions[1], Is.Not.EqualTo(newPos));

        positionDict[new Vector3(1, 1, 1)].GCost = 500;
        Assert.That(positions[0].GCost, Is.EqualTo(500));
        Assert.That(positionDict[new Vector3(1, 1, 1)].GCost, Is.EqualTo(500));
    }
}
