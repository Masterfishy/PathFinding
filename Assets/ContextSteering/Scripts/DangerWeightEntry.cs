using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// A DangerWeight represents a layer and its severity as a danger 0 to 1
/// </summary>
[System.Serializable]
public class DangerWeightEntry
{
    public LayerMask Mask;

    [Range(0, 1)]
    public float Weight;

    /// <summary>
    /// Create a new DangerWeightEntry
    /// </summary>
    /// <param name="mask">The mask the DangerWeightEntry represents</param>
    /// <param name="weight">The weight of the layer</param>
    public DangerWeightEntry(LayerMask mask, float weight)
    {
        Mask = mask;
        Weight = weight;
    }

    /// <summary>
    /// Hash a DangerWeightEntry by its layer mask
    /// </summary>
    /// <returns>The hash code of the stored layer mask</returns>
    public override int GetHashCode()
    {
        return Mask.GetHashCode();
    }
}
