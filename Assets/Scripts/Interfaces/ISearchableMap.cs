using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// This interface defines the API for a map that supports search algorithms
/// </summary>
public interface ISearchableMap 
{
    /// <summary>
    /// Get the neighbors around a given position
    /// </summary>
    /// <param name="position">The position to find neighbors of</param>
    /// <returns>A list of <c>ISearchablePosition</c>s that are neighbors of the given position</returns>
    public List<ISearchablePosition> GetNeighbors(ISearchablePosition position);

    /// <summary>
    /// Get the collection of <c>ISearchablePosition</c>s in the map
    /// </summary>
    public List<ISearchablePosition> MapPositions { get; }

    /// <summary>
    /// Get the size of the map
    /// </summary>
    public int Size { get; }
}

/// <summary>
/// This container class wraps the ISearchablemap interface to allow objects
/// that implement the interface to be drag and dropped into the property field
/// </summary>
[Serializable]
public class SearchableMapUnityContainer
{
    [SerializeField]
    private UnityEngine.Object searchableMapObject;

    public ISearchableMap Contents
    {
        get { return searchableMapObject as ISearchableMap; }
        set { searchableMapObject = value as UnityEngine.Object; }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SearchableMapUnityContainer))]
public class SearchableMapUnityContainerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        UnityEngine.Object mapHolder = property.serializedObject.targetObject;
        FieldInfo fieldInfo = mapHolder.GetType().GetField(property.propertyPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo.GetValue(mapHolder) is not SearchableMapUnityContainer mapProxy)
        {
            mapProxy = new();
            fieldInfo.SetValue(mapHolder, mapProxy);
        }

        EditorGUI.BeginChangeCheck();
        UnityEngine.Object rawObject = EditorGUI.ObjectField(position, label, mapProxy.Contents as UnityEngine.Object, typeof(UnityEngine.Object), true);

        // Handle rawObject is ISearchableMap
        if (rawObject is ISearchableMap searchAlgorithm)
        {
            mapProxy.Contents = searchAlgorithm;
        }

        // Handle rawObject is GameObject
        if (rawObject is GameObject go)
        {
            mapProxy.Contents = go.GetComponent<ISearchableMap>();
        }

        if (EditorGUI.EndChangeCheck())
        {
            _ = EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
#endif
