using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DangerWeights))]
public class DangerWeightsEditor : Editor
{
    private List<DataEntry> m_TempEntries;
    private string[] m_LayerNames;
    private bool m_HaveEntriesChanged;

    // Set up the icons
    private GUIContent m_AddedIcon;
    private GUIContent m_NotAddedIcon;

    private void OnEnable()
    {
        m_TempEntries = new();
        m_LayerNames = GetLayerNames();
        m_HaveEntriesChanged = false;

        // Create Icons
        m_AddedIcon = EditorGUIUtility.IconContent("TestPassed");
        m_NotAddedIcon = EditorGUIUtility.IconContent("TestFailed");

        // Update the temporary entries
        DangerWeights dangerWeights = target as DangerWeights;

        if (dangerWeights.DangerLayerWeights != null)
        {
            foreach (DangerWeightEntry entry in dangerWeights.DangerLayerWeights)
            {
                int index = LayerNameToIndex(LayerMask.LayerToName(entry.Mask), m_LayerNames);
                DataEntry newEntry = new(index, entry.Weight, true);
                m_TempEntries.Add(newEntry);
            }
        }

    }

    public override void OnInspectorGUI()
    {
        //-----
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Entry"))
        {
            m_TempEntries.Add(new DataEntry(0, 0, false));
            m_HaveEntriesChanged = true;
        }

        EditorGUILayout.LabelField("Danger Layer Weights");
        EditorGUILayout.LabelField(m_TempEntries.Count.ToString());

        EditorGUILayout.EndHorizontal();

        //-----
        foreach (DataEntry entry in m_TempEntries)
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent icon = entry.HasBeenAdded ? m_AddedIcon : m_NotAddedIcon;
            GUILayout.Label(icon, GUILayout.Width(20));

            if (GUILayout.Button("Remove"))
            {
                m_TempEntries.Remove(entry);
                m_HaveEntriesChanged = true;
                break;
            }

            int newIndex = EditorGUILayout.Popup(entry.LayerIndex, m_LayerNames);
            float newWeight = EditorGUILayout.Slider(entry.LayerWeight, 0, 1);

            if (entry.UpdateEntry(newIndex, newWeight))
            {
                entry.HasBeenAdded = false;
                m_HaveEntriesChanged = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        //-----
        GUI.enabled = m_HaveEntriesChanged;
        if (GUILayout.Button("Apply"))
        {
            DangerWeights dangerWeights = target as DangerWeights;
            if (dangerWeights.DangerLayerWeights != null)
            {
                dangerWeights.DangerLayerWeights.Clear();

                foreach (DataEntry entry in m_TempEntries)
                {
                    entry.HasBeenAdded = dangerWeights.DangerLayerWeights.Add(entry.ToDangerEntry(m_LayerNames));
                    if (!entry.HasBeenAdded)
                    {
                        EditorGUILayout.HelpBox($"Duplicate entry, {entry}, will not be added!", MessageType.Warning);
                    }
                }

                dangerWeights.CreateLayerMask();

                m_HaveEntriesChanged = false;
            }
        }
        GUI.enabled = true;
        
    }

    /// <summary>
    /// Get the index of the give layer name
    /// </summary>
    /// <param name="layerName">The name to search for</param>
    /// <param name="layerNames">The collection of layer names</param>
    /// <returns>The index of the name in the name collection; 0 if not found</returns>
    private static int LayerNameToIndex(string layerName, string[] layerNames)
    {
        for (int i = 0; i < layerNames.Length; i++)
        {
            if (layerNames[i] == layerName)
            {
                return i;
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the layer name at the index, safely
    /// </summary>
    /// <param name="index">The index to get</param>
    /// <param name="layerNames">The collection of layer names</param>
    /// <returns>The layer name or empty string if the index is invalid</returns>
    private static string IndexToLayerName(int index, string[] layerNames)
    {
        if (index >= 0 && index < layerNames.Length)
        {
            return layerNames[index];
        }

        return string.Empty;
    }

    /// <summary>
    /// Get the layer names defined in the Unity Editor
    /// </summary>
    /// <returns>A names of all the user defined layers</returns>
    private static string[] GetLayerNames()
    {
        List<string> names = new();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                names.Add(layerName);
            }
        }

        return names.ToArray();
    }

    private class DataEntry
    {
        public int LayerIndex { get; set; }
        public float LayerWeight { get; set; }
        public bool HasBeenAdded { get; set; }

        /// <summary>
        /// Create a new data entry
        /// </summary>
        /// <param name="layerIndex">The layer index</param>
        /// <param name="layerWeight">The layer weight</param>
        /// <param name="addStatus">The status of being added to the final collection</param>
        public DataEntry(int layerIndex, float layerWeight, bool addStatus)
        {
            LayerIndex = layerIndex;
            LayerWeight = layerWeight;
            HasBeenAdded = addStatus;
        }

        public bool UpdateEntry(int layerIndex, float layerWeight)
        {
            bool hasChanged = false;

            if (LayerIndex != layerIndex)
            {
                LayerIndex = layerIndex;
                hasChanged = true;
            }

            if (LayerWeight != layerWeight)
            {
                LayerWeight = layerWeight;
                hasChanged = true;
            }

            return hasChanged;
        }

        /// <summary>
        /// Convert this data entry to a DangerWeightEntry
        /// </summary>
        /// <param name="layerNames">The set of layers</param>
        /// <returns>The converted DangerWeightEntry</returns>
        public DangerWeightEntry ToDangerEntry(string[] layerNames)
        {
            string layerName = IndexToLayerName(LayerIndex, layerNames);

            return new DangerWeightEntry(LayerMask.NameToLayer(layerName), LayerWeight);
        }

        public override string ToString()
        {
            return $"Entry: layer={LayerIndex}, weight={LayerWeight}";
        }
    }
}
