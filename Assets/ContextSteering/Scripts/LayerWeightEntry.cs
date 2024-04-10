using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A DangerWeight represents a layer and its severity as a danger 0 to 1
/// </summary>
[System.Serializable]
public class LayerWeightEntry
{
    public LayerMask Mask;
    public float Weight;

    /// <summary>
    /// Create a new LayerWeightEntry
    /// </summary>
    /// <param name="mask">The mask the LayerWeightEntry represents</param>
    /// <param name="weight">The weight of the layer</param>
    public LayerWeightEntry(LayerMask mask, float weight)
    {
        Mask = mask;
        Weight = weight;
    }

    /// <summary>
    /// Hash a LayerWeightEntry by its layer mask
    /// </summary>
    /// <returns>The hash code of the stored layer mask</returns>
    public override int GetHashCode()
    {
        return Mask.value;
    }

    /// <summary>
    /// Compare the stored LayerMasks
    /// </summary>
    /// <param name="obj">The other LayerWeightEntry</param>
    /// <returns>True if the LayerMasks are equal</returns>
    public override bool Equals(object obj)
    {
        return obj is LayerWeightEntry other && other.Mask == Mask;
    }

    public override string ToString()
    {
        return $"Entry=[Mask: {LayerMask.LayerToName(Mask)}, Weight: {Weight}]";
    }
}
