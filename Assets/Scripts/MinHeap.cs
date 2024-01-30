using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a min heap
/// </summary>
/// <typeparam name="T">The type of class to store that implements the IHeapItem interface</typeparam>
public class MinHeap<T> where T : IHeapItem<T>
{
    private T[] mItems;
    private int mItemCount;

    public int Count
    {
        get
        {
            return mItemCount;
        }
    }

    /// <summary>
    /// Create a new heap object with maximum capacity maxHeapSize.
    /// </summary>
    /// <param name="maxHeapSize">The maximum number of items the heap can store</param>
    public MinHeap(int maxHeapSize)
    {
        mItemCount = 0;
        mItems = new T[maxHeapSize];
    }

    /// <summary>
    /// Default constructor for a new heap object with max capacity of 500
    /// </summary>
    public MinHeap() : this(500) { }

    /// <summary>
    /// Add an item to the heap.
    /// </summary>
    /// <param name="item">The item to add</param>
    public void Push(T item)
    {
        if (IsFull())
        {
            throw new InvalidOperationException("Cannot push to a full heap.");
        }

        item.HeapIndex = mItemCount;
        mItems[mItemCount] = item;

        SortUp(item);
        mItemCount++;
    }

    /// <summary>
    /// Removes and returns the first item in the heap.
    /// </summary>
    /// <returns>The first item of type <c>T</c> in the heap</returns>
    public T Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Cannot pop from an empty heap.");
        }

        T firstItem = mItems[0];
        mItemCount--;

        mItems[0] = mItems[mItemCount];
        mItems[0].HeapIndex = 0;

        SortDown(mItems[0]);

        return firstItem;
    }

    /// <summary>
    /// Peak the item on the top of the heap.
    /// </summary>
    /// <returns>The first item of type <c>T</c> in the heap</returns>
    public T Peak()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Cannot peak from an empty heap.");
        }

        return mItems[0];
    }

    /// <summary>
    /// Update the priority of an item if the heap contains it.
    /// </summary>
    /// <param name="item">The updated item</param>
    public void UpdateItem(T item)
    {
        if (!Contains(item))
        {
            return;
        }

        // Sort the item to its proper location by shaking it up and down
        SortUp(item);
        SortDown(item);
    }

    /// <summary>
    /// Check if an item is contained in the heap.
    /// </summary>
    /// <param name="item">The item to search for</param>
    /// <returns>True if the item is in the heap, false otherwise</returns>
    public bool Contains(T item)
    {
        return Equals(mItems[item.HeapIndex], item);
    }

    /// <summary>
    /// Check if the heap is full
    /// </summary>
    /// <returns>True if the heap is full; false otherwise</returns>
    public bool IsFull()
    {
        return mItemCount == mItems.Length;
    }

    /// <summary>
    /// Check if the heap is empty
    /// </summary>
    /// <returns>True if the heap is empty; false otherwise</returns>
    public bool IsEmpty()
    {
        return mItemCount == 0;
    }

    /// <summary>
    /// Sorts an item down the heap to its proper location.
    /// </summary>
    /// <param name="item">The item to sort</param>
    private void SortDown(T item)
    {
        while (true)
        {
            int swapIndex;
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;

            // If the item has a child
            if (childIndexLeft < mItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < mItemCount)
                {
                    // If the left child has a higher priority than the right child
                    if (mItems[childIndexLeft].CompareTo(mItems[childIndexRight]) < 0)
                    {
                        // Swap with the smaller one
                        swapIndex = childIndexRight;
                    }
                }

                // If the item has a higher priority than its child
                if (item.CompareTo(mItems[swapIndex]) < 0)
                {
                    Swap(item, mItems[swapIndex]);
                }
                else
                {
                    // Otherwise we are done sorting
                    return;
                }
            }
            else
            {
                // Otherwise we are done sorting
                return;
            }
        }
    }

    /// <summary>
    /// Sorts an item up the heap to its proper location.
    /// </summary>
    /// <param name="item">The item to sort</param>
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = mItems[parentIndex];

            // If the item's priority is smaller than the parent, swap them
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                // Otherwise we are done here.
                return;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /// <summary>
    /// Swaps two items in the items array.
    /// </summary>
    /// <param name="itemA">The first item</param>
    /// <param name="itemB">The second item</param>
    private void Swap(T itemA, T itemB)
    {
        mItems[itemA.HeapIndex] = itemB;
        mItems[itemB.HeapIndex] = itemA;

        (itemB.HeapIndex, itemA.HeapIndex) = (itemA.HeapIndex, itemB.HeapIndex);
    }
}

/// <summary>
/// Interface for items that will be added to a heap.
/// </summary>
/// <typeparam name="T">Type of the item being stored in a heap</typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex
    {
        get;
        set;
    }
}