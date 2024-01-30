using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MinHeapTest
{
    /// <summary>
    /// Test the min heap default constructor
    /// </summary>
    [Test]
    public void TestConstructor()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new();

        // Act

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(0));
    }

    /// <summary>
    /// Test the push operation of the min heap
    /// </summary>
    [Test]
    public void TestPush()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(3);
        DummyHeapItem item = new(1);

        // Act
        minHeap.Push(item);

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(1));
        Assert.That(minHeap.Peak(), Is.EqualTo(item));
    }

    [Test]
    public void TestFullPush()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(1);
        InvalidOperationException caughtException = null;
        minHeap.Push(new DummyHeapItem(2));

        // Act
        try
        {
            minHeap.Push(new DummyHeapItem(1));
        }
        catch (InvalidOperationException exception)
        {
            caughtException = exception;
        }

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(1));
        Assert.That(minHeap.IsFull(), Is.True);
        Assert.That(caughtException, Is.Not.Null);
    }

    /// <summary>
    /// Test the pop operation of the min heap
    /// </summary>
    [Test]
    public void TestPop()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(3);
        int expectedValue = 1;
        minHeap.Push(new DummyHeapItem(expectedValue));

        // Act
        DummyHeapItem item = minHeap.Pop();

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(0));
        Assert.That(item.Value, Is.EqualTo(expectedValue));
    }

    /// <summary>
    /// Test the pop operation on an empty heap
    /// </summary>
    [Test]
    public void TestEmptyPop()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(1);
        InvalidOperationException caughtException = null;

        // Act
        try
        {
            minHeap.Pop();
        }
        catch (InvalidOperationException exception)
        {
            caughtException = exception;
        }

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(0));
        Assert.That(minHeap.IsEmpty(), Is.True);
        Assert.That(caughtException, Is.Not.Null);
    }

    /// <summary>
    /// Test the peak operation with an empty heap
    /// </summary>
    [Test]
    public void TestEmptyPeak()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(1);
        InvalidOperationException caughtException = null;

        // Act
        try
        {
            minHeap.Peak();
        }
        catch (InvalidOperationException exception)
        {
            caughtException = exception;
        }

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(0));
        Assert.That(minHeap.IsEmpty(), Is.True);
        Assert.That(caughtException, Is.Not.Null);
    }

    /// <summary>
    /// Test the update operation of the min heap
    /// </summary>
    [Test]
    public void TestUpdate()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(3);
        DummyHeapItem item1 = new(1);
        DummyHeapItem item2 = new(2);

        int expectedValue = 3;

        minHeap.Push(item1);
        minHeap.Push(item2);

        Assert.That(minHeap.Peak(), Is.EqualTo(item1));

        // Act
        item1.Value = expectedValue;
        minHeap.UpdateItem(item1);

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(2));
        Assert.That(minHeap.Peak(), Is.EqualTo(item2));
    }

    /// <summary>
    /// Test the update operation of the min heap when the item is not in the heap
    /// </summary>
    [Test]
    public void TestUncontianedUpdate()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(3);
        DummyHeapItem item1 = new(1);
        DummyHeapItem item2 = new(4);

        int expectedValue = 3;

        minHeap.Push(item2);

        // Act
        item1.Value = expectedValue;
        minHeap.UpdateItem(item1);

        // Assert
        Assert.That(minHeap.Count, Is.EqualTo(1));
        Assert.That(minHeap.Peak(), Is.EqualTo(item2));
    }

    /// <summary>
    /// Test multiple operations to ensure correct order
    /// </summary>
    [Test]
    public void TestMinHeap()
    {
        // Arrange
        MinHeap<DummyHeapItem> minHeap = new(5);

        DummyHeapItem item1 = new(1);
        DummyHeapItem item2 = new(2);
        DummyHeapItem item3 = new(3);
        DummyHeapItem item4 = new(4);
        DummyHeapItem item5 = new(5);

        // Act & Assert
        minHeap.Push(item4);
        minHeap.Push(item2);

        // Make sure that item2 is the top item (miniest)
        Assert.That(minHeap.Peak(), Is.EqualTo(item2));

        minHeap.Push(item3);
        minHeap.Push(item5);

        Assert.That(minHeap.Count, Is.EqualTo(4));

        minHeap.Push(item1);

        Assert.That(minHeap.Peak(), Is.EqualTo(item1));
        Assert.That(minHeap.Count, Is.EqualTo(5));

        // Pop everything and check order
        Assert.That(minHeap.Pop(), Is.EqualTo(item1));
        Assert.That(minHeap.Pop(), Is.EqualTo(item2));
        Assert.That(minHeap.Pop(), Is.EqualTo(item3));
        Assert.That(minHeap.Pop(), Is.EqualTo(item4));
        Assert.That(minHeap.Pop(), Is.EqualTo(item5));
    }

    private class DummyHeapItem : IHeapItem<DummyHeapItem>
    {
        public int Value;

        public DummyHeapItem(int val)
        {
            Value = val;
        }

        private int mHeapIndex;
        public int HeapIndex { get => mHeapIndex; set => mHeapIndex = value; }

        public int CompareTo(DummyHeapItem other)
        {
            return other.Value - Value;
        }
    }
}
