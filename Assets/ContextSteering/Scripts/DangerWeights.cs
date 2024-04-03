using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;


#if UNITY_EDITOR
using UnityEditor;
using System;
#endif

/// <summary>
/// This scriptable object serializes a collection of DangerWeightEntries
/// </summary>
[CreateAssetMenu(menuName ="ContextSteering/DangerWeights")]
public class DangerWeights : ScriptableObject
{
    public LayerMask DangerLayerMask;
    public HashSet<DangerWeightEntry> DangerLayerWeights;

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

#if UNITY_EDITOR
[CustomEditor(typeof(DangerWeights))]
public class DangerWeightsEditor : Editor
{
    private readonly int m_MaxLayers = 32;
    private string[] m_Layers = new string[32];

    private List<DangerWeightEntry> m_TempEntries;
    private bool m_HaveChangesBeenApplied;

    public void OnEnable()
    {
        // Add only non empty strings to the layers
        int stringIndex = 0;
        for (int i = 0; i < m_MaxLayers; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName != "")
            {
                m_Layers[stringIndex++] = layerName;
            }
        }

        Array.Resize(ref m_Layers, stringIndex);

        // Copy the entries for editing
        DangerWeights dangerWeights = target as DangerWeights;
        m_TempEntries = new();

        if (dangerWeights.DangerLayerWeights != null && dangerWeights.DangerLayerWeights.Count > 0)
        {
            m_TempEntries.AddRange(dangerWeights.DangerLayerWeights);
        }

        Debug.Log("Ehem, enable");

        m_HaveChangesBeenApplied = true;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Entry"))
        {
            DangerWeightEntry newEntry = new(0, 0);
            m_TempEntries.Add(newEntry);

            m_HaveChangesBeenApplied = false;
        }

        EditorGUILayout.LabelField("Danger Layer Weights");
        EditorGUILayout.LabelField(m_TempEntries.Count.ToString());

        EditorGUILayout.EndHorizontal();

        foreach (DangerWeightEntry entry in m_TempEntries)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove"))
            {
                m_TempEntries.Remove(entry);

                m_HaveChangesBeenApplied = false;
                break;
            }

            entry.Mask = EditorGUILayout.MaskField(entry.Mask, m_Layers);
            entry.Weight = EditorGUILayout.Slider(entry.Weight, 0, 1);
            EditorGUILayout.EndHorizontal();
        }

        GUI.enabled = !m_HaveChangesBeenApplied;
        if (GUILayout.Button("Apply"))
        {
            DangerWeights dangerWeights = target as DangerWeights;
            dangerWeights.DangerLayerWeights ??= new();
            dangerWeights.DangerLayerWeights.Clear();
            
            foreach (DangerWeightEntry entry in m_TempEntries)
            {
                if (!dangerWeights.DangerLayerWeights.Add(entry))
                {
                    Debug.LogWarning($"Duplicate entry, {entry}, cannot be added!");
                }
            }

            dangerWeights.CreateLayerMask();

            m_HaveChangesBeenApplied = true;
        }
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();

        
    }  
}

#endif
