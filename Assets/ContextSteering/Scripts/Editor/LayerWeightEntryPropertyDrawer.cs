using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LayerWeightEntry))]
public class LayerWeightEntryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //-----
        string[] layerNames = GetLayerNames();
        int layerIndex = LayerIndexToIndex(property.FindPropertyRelative("Index").intValue, layerNames);

        //-----
        float spacer = 10f;
        float middlePos = position.width / 2;
        Rect popupPos = new(position.x, position.y, middlePos - (spacer * 2), position.height);
        Rect sliderPos = new(position.x + middlePos - spacer, position.y, middlePos + spacer, position.height);

        //-----
        EditorGUILayout.BeginHorizontal();

        int maskIndex = EditorGUI.Popup(popupPos, layerIndex, layerNames);
        float weight = EditorGUI.Slider(sliderPos, property.FindPropertyRelative("Weight").floatValue, 0, 1);

        EditorGUILayout.EndHorizontal();

        //-----
        property.FindPropertyRelative("Index").intValue = IndexToLayerIndex(maskIndex, layerNames);
        property.FindPropertyRelative("Weight").floatValue = weight;
    }

    /// <summary>
    /// Convert a layer mask to an index
    /// </summary>
    /// <param name="layerIndex">The index of the layer defined by the user presets</param>
    /// <returns>The index value of the layer mask</returns>
    private static int LayerIndexToIndex(int layerIndex, string[] layerNames)
    {
        return LayerNameToIndex(LayerMask.LayerToName(layerIndex), layerNames);
    }

    /// <summary>
    /// Convert an index to a layer mask
    /// </summary>
    /// <param name="index">The index of the layer mask</param>
    /// <returns>The layer index defined by the user presets</returns>
    private static LayerMask IndexToLayerIndex(int index, string[] layerNames)
    {
        return LayerMask.NameToLayer(IndexToLayerName(index, layerNames));
    }

    private static int LayerNameToIndex(string layerName, string[] layerNames)
    {
        return System.Array.IndexOf(layerNames, layerName);
    }

    private static string IndexToLayerName(int index, string[] layerNames)
    {
        if (index > 0 && index < layerNames.Length)
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
}
