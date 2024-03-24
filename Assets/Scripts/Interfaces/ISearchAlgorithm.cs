using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// This interface defines the API of a search algorithm
/// </summary>
public interface ISearchAlgorithm
{
    /// <summary>
    /// Find a path from the start position to the end position.
    /// </summary>
    /// <param name="pathRequest">The request to find a path for</param>
    /// <param name="callback">The callback to report the found path</param>
    public IEnumerator FindPath(PathRequest pathRequest, Action<PathRequest> callback);
}

/// <summary>
/// This container class wraps the ISearchAlgorithm interface to allow objects
/// that implement the interface to be drag and dropped into the property field
/// </summary>
[Serializable]
public class SearchAlgorithmUnityContainer
{
    [SerializeField]
    private UnityEngine.Object searchAlgorithmObject;

    public ISearchAlgorithm Contents
    {
        get { return searchAlgorithmObject as ISearchAlgorithm; }
        set { searchAlgorithmObject = value as UnityEngine.Object; }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SearchAlgorithmUnityContainer))]
public class SearchAlgorithmUnityContainerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        UnityEngine.Object algoHolder = property.serializedObject.targetObject;
        FieldInfo fieldInfo = algoHolder.GetType().GetField(property.propertyPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo.GetValue(algoHolder) is not SearchAlgorithmUnityContainer algoProxy)
        {
            algoProxy = new();
            fieldInfo.SetValue(algoHolder, algoProxy);
        }

        EditorGUI.BeginChangeCheck();
        UnityEngine.Object rawObject = EditorGUI.ObjectField(position, label, algoProxy.Contents as UnityEngine.Object, typeof(UnityEngine.Object), true);

        // Handle rawObject is ISearchAlgorithm
        if (rawObject is ISearchAlgorithm searchAlgorithm)
        {
            algoProxy.Contents = searchAlgorithm;
        }

        // Handle rawObject is GameObject
        if (rawObject is GameObject go)
        {
            algoProxy.Contents = go.GetComponent<ISearchAlgorithm>();
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
