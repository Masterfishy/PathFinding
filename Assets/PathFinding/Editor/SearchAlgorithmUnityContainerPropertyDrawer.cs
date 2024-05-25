using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;

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

