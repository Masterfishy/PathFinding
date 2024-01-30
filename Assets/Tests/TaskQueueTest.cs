using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TaskQueueTest
{
    /// <summary>
    /// Test the default constructor of the task queue.
    /// </summary>
    [Test]
    public void TestDefaultConstructor()
    {
        // Arrange
        TaskQueue<string, string> taskQueue = new();

        // Act

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(5));
    }

    /// <summary>
    /// Test the constructor with a user defined starting capacity.
    /// </summary>
    [Test]
    public void TestConstructor()
    {
        // Arrange
        TaskQueue<string, string> taskQueue = new(10);

        // Act

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(10));
    }

    /// <summary>
    /// Test the enqueue task queue function.
    /// </summary>
    [Test]
    public void TestEnqueue()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new();

        // Act
        taskQueue.Enqueue("source1", 1);

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.False);
        Assert.That(taskQueue.Count, Is.EqualTo(1));
        Assert.That(taskQueue.Capacity, Is.EqualTo(5));
    }

    /// <summary>
    /// Test the dequeue task queue function.
    /// </summary>
    [Test]
    public void TestDequeue()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new();
        int expectedResult = 1;
        taskQueue.Enqueue("source1", expectedResult);

        // Act
        int actualResult = taskQueue.Dequeue();

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(5));
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// Test filling the task queue to full
    /// </summary>
    [Test]
    public void TestFull()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new(1);

        // Act
        taskQueue.Enqueue("string1", 1);

        // Assert
        Assert.That(taskQueue.IsFull(), Is.True);
        Assert.That(taskQueue.IsEmpty(), Is.False);
        Assert.That(taskQueue.Count, Is.EqualTo(1));
        Assert.That(taskQueue.Capacity, Is.EqualTo(1));
    }

    /// <summary>
    /// Test the resizing of the task queue when full
    /// </summary>
    [Test]
    public void TestResize()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new(1);

        // Act
        taskQueue.Enqueue("string1", 1);
        taskQueue.Enqueue("string2", 2);

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.False);
        Assert.That(taskQueue.Count, Is.EqualTo(2));
        Assert.That(taskQueue.Capacity, Is.EqualTo(3));
    }

    /// <summary>
    /// Test dequeuing from an empty task queue
    /// </summary>
    [Test]
    public void TestEmptyDequeue()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new(1);
        InvalidOperationException caughtException = null;

        // Act
        try
        {
            _ = taskQueue.Dequeue();
        }
        catch (InvalidOperationException exception)
        {
            caughtException = exception;
        }

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(1));
        Assert.That(caughtException, Is.Not.Null);
    }

    /// <summary>
    /// Test the enqueue operation with the same source.
    /// </summary>
    [Test]
    public void TestSameSourceEnqueue()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new(1);
        int lastTask = 5;
        string source = "source1";
        int actualResult = 0;

        // Act
        taskQueue.Enqueue(source, 1);
        taskQueue.Enqueue(source, 2);
        taskQueue.Enqueue(source, 3);
        taskQueue.Enqueue(source, 4);
        taskQueue.Enqueue(source, lastTask);

        actualResult = taskQueue.Dequeue();

        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(1));
        Assert.That(actualResult, Is.EqualTo(lastTask));
    }

    [Test]
    public void TestNullSourceEnqueue()
    {
        // Arrange
        TaskQueue<string, int> taskQueue = new(1);
        ArgumentNullException caughtException = null;

        // Act
        try
        {
            taskQueue.Enqueue(null, 1);
        }
        catch (ArgumentNullException exception)
        {
            caughtException = exception;
        }

        // Assert
        // Assert
        Assert.That(taskQueue.IsFull(), Is.False);
        Assert.That(taskQueue.IsEmpty(), Is.True);
        Assert.That(taskQueue.Count, Is.EqualTo(0));
        Assert.That(taskQueue.Capacity, Is.EqualTo(1));
        Assert.That(caughtException, Is.Not.Null);
    }
}
