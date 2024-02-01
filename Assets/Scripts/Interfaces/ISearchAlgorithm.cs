using System;
using System.Reflection;
using UnityEngine;


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
    /// <param name="start">The start position</param>
    /// <param name="end">The end position</param>
    /// <param name="map">The search able map the positions are found on</param>
    /// <param name="callback">The callback to report the found path</param>
    public void FindPath(ISearchablePosition start, ISearchablePosition end, ISearchableMap map, Action<ISearchablePosition[]> callback);
}

/// <summary>
/// This container class wraps the interface to allow objects that implement 
/// the interface to appear in the unity editor as serializable
/// </summary>
[Serializable]
public class SearchAlgorithmUnityContainer
{
    [UnityEngine.SerializeField]
    private UnityEngine.Object searchAlgorithmObject;

    public ISearchAlgorithm SearchAlgorithm
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
        algoProxy.SearchAlgorithm = EditorGUI.ObjectField(position, label, algoProxy.SearchAlgorithm as UnityEngine.Object, typeof(ISearchAlgorithm), true) as ISearchAlgorithm;
        if (EditorGUI.EndChangeCheck())
        {
            _ = EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}
#endif
