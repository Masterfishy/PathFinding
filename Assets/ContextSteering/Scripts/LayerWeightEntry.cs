using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A LayerWeightEntry represents a layer and its weight 0 to 1
/// </summary>
[System.Serializable]
public class LayerWeightEntry
{
    /// <summary>
    /// The LayerIndex
    /// </summary>
    public int Index;

    /// <summary>
    /// The weight valued assigned to the layer mask
    /// </summary>
    public float Weight;

    /// <summary>
    /// The LayerMask
    /// </summary>
    public LayerMask Mask
    {
        get
        {
            return LayerIndexToLayerMask(Index);
        }
    }

    /// <summary>
    /// Default contructor
    /// </summary>
    public LayerWeightEntry()
    {
        Index = 0;
        Weight = 0;
    }

    /// <summary>
    /// Create a new LayerWeightEntry
    /// </summary>
    /// <param name="layerIndex">The index of the mask this entry represents</param>
    /// <param name="weight">The weight of the layer</param>
    public LayerWeightEntry(int layerIndex, float weight)
    {
        Index = layerIndex;
        Weight = weight;
    }

    /// <summary>
    /// Create a new LayerWeightEntry
    /// </summary>
    /// <param name="layerMask">The index of the mask this entry represents</param>
    /// <param name="weight">The weight of the layer</param>
    public LayerWeightEntry(LayerMask layerMask, float weight)
    {
        Index = LayerMaskToLayerIndex(layerMask);
        Weight = weight;
    }


    /// <summary>
    /// Hash a LayerWeightEntry by its layer mask
    /// </summary>
    /// <returns>The hash code of the stored layer mask</returns>
    public override int GetHashCode()
    {
        return Index;
    }

    /// <summary>
    /// Check if the two LayerWeightEntries are equal
    /// </summary>
    /// <param name="obj">The other LayerWeightEntry</param>
    /// <returns>True if the LayerIndex are equal</returns>
    public override bool Equals(object obj)
    {
        return obj is LayerWeightEntry other && other.Index == Index;
    }

    public override string ToString()
    {
        return $"Entry=[Mask: {LayerMask.LayerToName(Index)}, Weight: {Weight}]";
    }

    /// <summary>
    /// Convert a LayerMask to its LayerIndex using Log base 2
    /// </summary>
    /// <param name="mask">The mask to convert</param>
    /// <returns>The index of the mask defined by user presets</returns>
    private int LayerMaskToLayerIndex(LayerMask mask)
    {
        return (int)Mathf.Log(mask.value, 2);
    }

    /// <summary>
    /// Convert a LayerIndex to its LayerMask using bit shifting
    /// </summary>
    /// <param name="index">The index to convert</param>
    /// <returns>The layer mask of the index defined by the user presets</returns>
    private LayerMask LayerIndexToLayerMask(int index)
    {
        return 1 << index;
    }
}
