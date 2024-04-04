using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This scriptable object serializes a collection of DangerWeightEntries
/// </summary>
[CreateAssetMenu(menuName ="ContextSteering/DangerWeights")]
public class DangerWeights : ScriptableObject
{
    public LayerMask DangerLayerMask;

    private HashSet<DangerWeightEntry> m_DangerLayerWeights;
    public HashSet<DangerWeightEntry> DangerLayerWeights
    {
        get
        {
            return m_DangerLayerWeights ??= new();
        }
    }

    /// <summary>
    /// Get the weight of a layer
    /// </summary>
    /// <param name="layer">The layer to get the weight of</param>
    /// <returns>The weight of the given layer; 0 if the layer isn't found</returns>
    public float GetLayerWeight(LayerMask layer)
    {
        DangerWeightEntry searchEntry = new(layer, 0);
        if (DangerLayerWeights.TryGetValue(searchEntry, out DangerWeightEntry layerWeight))
        {
            return layerWeight.Weight;
        }

        return 0f;
    }

    /// <summary>
    /// Create the layer mask from the layer weights
    /// </summary>
    public void CreateLayerMask()
    {
        foreach (DangerWeightEntry entry in DangerLayerWeights)
        {
            DangerLayerMask |= entry.Mask;
        }
    }
}
