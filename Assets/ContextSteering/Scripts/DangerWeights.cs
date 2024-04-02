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

    private List<DangerWeightEntry> m_EditingEntries;

    private float m_InspectorWidth;

    public void OnEnable()
    {
        DangerWeights dangerWeights = target as DangerWeights;
        dangerWeights.DangerLayerWeights ??= new();

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
        m_EditingEntries = new();
        foreach (DangerWeightEntry entry in dangerWeights.DangerLayerWeights)
        {
            m_EditingEntries.Add(entry);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DangerWeights dangerWeights = target as DangerWeights;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Entry"))
        {
            DangerWeightEntry newEntry = new(0, 0f);
            m_EditingEntries.Add(newEntry);
        }

        EditorGUILayout.LabelField("Danger Layer Weights");

        EditorGUILayout.LabelField(m_EditingEntries.Count.ToString());

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;

        for (int i = 0; i < m_EditingEntries.Count; i++)
        {
            DangerWeightEntry entry = m_EditingEntries[i];
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Entry {i}", GUILayout.MaxWidth(60));
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(60)))
            {
                m_EditingEntries.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Layer");
            entry.Mask = EditorGUILayout.MaskField(entry.Mask, m_Layers);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();            
            EditorGUILayout.LabelField("Weight");
            entry.Weight = EditorGUILayout.Slider(entry.Weight, 0, 1);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--;

        if (serializedObject.ApplyModifiedProperties())
        {
            // Update the dangerWeights.DangerLayerWeights with m_EditingEntries
            foreach (DangerWeightEntry entry in m_EditingEntries)
            {
                if (!dangerWeights.DangerLayerWeights.Add(entry))
                {
                    Debug.LogWarning($"Duplicate layer weight, {entry}, could not be added!");
                }
            }

            dangerWeights.CreateLayerMask();
        }
    }
}

#endif
