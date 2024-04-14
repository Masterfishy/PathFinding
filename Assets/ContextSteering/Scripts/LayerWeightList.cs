using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This scriptable object serializes a collection of DangerWeightEntries
/// </summary>
[CreateAssetMenu(menuName = "ContextSteering/LayerWeightList")]
public class LayerWeightList : ScriptableObject
{
    public LayerMask LayerMask;
    public LayerWeightEntry[] LayerWeightEntries;

    private HashSet<LayerWeightEntry> m_LayerWeightEntries;

    /// <summary>
    /// Get the weight of a layer
    /// </summary>
    /// <param name="layer">The layer to get the weight of</param>
    /// <returns>The weight of the given layer; 0 if the layer isn't found</returns>
    public float GetLayerWeight(LayerMask layer)
    {
        Debug.Log($"Get weight of layer: {layer.value} {LayerMask.LayerToName(layer)}");

        if (m_LayerWeightEntries == null)
        {
            Debug.Log("Yeah, m_layerweightentries is not initialized");
            return 0f;
        }

        LayerWeightEntry searchEntry = new(layer.value, 0f);
        Debug.Log($"Search Entry {searchEntry}");
        if (m_LayerWeightEntries.TryGetValue(searchEntry, out LayerWeightEntry entry)) 
        {
            Debug.Log($"Found entry: {entry}");
            return entry.Weight;
        }

        Debug.Log("No entry found for layer");

        return 0f;
    }

    /// <summary>
    /// Create the layer mask from the layer weights
    /// </summary>
    public void CreateLayerMask()
    {
        LayerMask = 0;

        foreach (LayerWeightEntry entry in m_LayerWeightEntries)
        {
            Debug.Log($"ORing {entry}");
            Debug.Log($"Mask name : {LayerMask.LayerToName(entry.Index)}");
            Debug.Log($"Mask index: {entry.Index}");
            Debug.Log($"Mask      : {entry.Mask.value}");
            LayerMask |= entry.Mask;
        }

        Debug.Log($"Created Layer Mask: {LayerMask.value}");
    }

    public void GenerateLayerWeight()
    {
        if (m_LayerWeightEntries == null)
        {
            m_LayerWeightEntries = new();
        }

        m_LayerWeightEntries.Clear();

        foreach (LayerWeightEntry entry in LayerWeightEntries)
        {
            if (!m_LayerWeightEntries.Add(entry))
            {
                Debug.LogWarning($"Duplicate entry, {entry}, will be unused!");
                continue;
            }

            Debug.Log($"Added {entry}");
        }

        CreateLayerMask();
    }

    private void OnEnable()
    {
        GenerateLayerWeight();
    }
}
