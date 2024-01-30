using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents an eviction task queue that only allows for 1 task of type T per source type S.
/// </summary>
/// <typeparam name="S">The type of sources to manage</typeparam>
/// <typeparam name="T">The type of tasks to queue</typeparam>
public class TaskQueue<S, T>
{
    /// <summary>
    /// A dictionary to keep track of which sources have tasks in the queue. Maps a source to the index of the task.
    /// </summary>
    private readonly Dictionary<S, int> mQueuedSources;

    /// <summary>
    /// An circular array to serve as the queue.
    /// </summary>
    private TaskEntry<S, T>[] mTaskQueue;

    /// <summary>
    /// The index of the array to enqueue.
    /// </summary>
    private int mEnqueueIndex;

    /// <summary>
    /// The index of the array to dequeue.
    /// </summary>
    private int mDequeueIndex;

    /// <summary>
    /// The capacity of the array.
    /// </summary>
    private int mCapacity;

    /// <summary>
    /// Create a new TaskQueue with the given capacity
    /// </summary>
    /// <param name="capacity">The starting capacity of the TaskQueue</param>
    public TaskQueue(int capacity)
    {
        mCapacity = capacity + 1;
        mEnqueueIndex = 0;
        mDequeueIndex = 0;

        mQueuedSources = new();
        mTaskQueue = new TaskEntry<S, T>[mCapacity];
    }

    /// <summary>
    /// Default constructor to create a TaskQueue with a capacity of 5
    /// </summary>
    public TaskQueue() : this(5) { }

    /// <summary>
    /// The number of tasks in the queue.
    /// </summary>
    public int Count
    {
        get 
        {
            if (mDequeueIndex > mEnqueueIndex)
            { 
                return mCapacity - (mDequeueIndex - mEnqueueIndex);
            }

            return mEnqueueIndex - mDequeueIndex;
        }
    }

    /// <summary>
    /// The maximum capacity of the task queue.
    /// </summary>
    public int Capacity
    {
        get { return mCapacity - 1; }
    }

    /// <summary>
    /// Check if there are any tasks in the queue.
    /// </summary>
    /// <returns>True if there are no tasks in the queue; false otherwise</returns>
    public bool IsEmpty()
    {
        return mEnqueueIndex == mDequeueIndex;
    }

    /// <summary>
    /// Check if the task queue is full
    /// </summary>
    /// <returns>True if the queue is full; false otherwise</returns>
    public bool IsFull()
    {
        return (mEnqueueIndex + 1) % mCapacity == mDequeueIndex;
    }

    /// <summary>
    /// Enqueue a new task to the task queue for the given source.
    /// </summary>
    /// <param name="source">The source of the task</param>
    /// <param name="task">The task to queue</param>
    public void Enqueue(S source, T task)
    {
        // Ensure the source is not null
        if (source == null)
        {
            throw new ArgumentNullException(paramName: nameof(source), "Source cannot be null");
        }

        // Check if source has a task in the queue
        if (mQueuedSources.TryGetValue(source, out int index))
        {
            // Set the index to the last item to remove it
            mTaskQueue[index] = mTaskQueue[mEnqueueIndex - 1];

            // Move the Enqueue index
            mEnqueueIndex = (mEnqueueIndex - 1) % mCapacity;
        }

        // If queue is full, grow the capacity
        if (IsFull())
        {
            ResizeTaskQueue();
        }

        // Add new task
        mTaskQueue[mEnqueueIndex] = new TaskEntry<S, T>(source, task);
        mQueuedSources[source] = mEnqueueIndex;

        // Move the enqueue index
        mEnqueueIndex = (mEnqueueIndex + 1) % mCapacity;
    }

    /// <summary>
    /// Dequeue a task.
    /// </summary>
    /// <returns>A task</returns>
    public T Dequeue()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Cannot dequeue from an empty task queue!");
        }

        // Get the item at the dequeue index
        TaskEntry<S, T> toReturn = mTaskQueue[mDequeueIndex];

        // Remove the task from the task queue
        mTaskQueue[mDequeueIndex] = null;

        // Remove the source from the queued sources
        mQueuedSources.Remove(toReturn.Origin);

        // Update the dequeue index
        mDequeueIndex = (mDequeueIndex + 1) % mCapacity;

        return toReturn.Data;
    }

    /// <summary>
    /// Resizes the task queue by doubling the capacity
    /// </summary>
    private void ResizeTaskQueue()
    {
        // Double the capacity
        mCapacity *= 2;

        TaskEntry<S, T>[] newQueue = new TaskEntry<S, T>[mCapacity];
        Array.Copy(mTaskQueue, newQueue, Mathf.Min(mTaskQueue.Length, mCapacity));

        mTaskQueue = newQueue;
    }

    /// <summary>
    /// Internal class to represent elements stored in a task queue
    /// </summary>
    /// <typeparam name="I">The identifying type</typeparam>
    /// <typeparam name="D">The type of the data</typeparam>
    private class TaskEntry<I, D>
    {
        public I Origin;
        public D Data;

        public TaskEntry(I id, D data)
        {
            Origin = id;
            Data = data;
        }
    }
}
