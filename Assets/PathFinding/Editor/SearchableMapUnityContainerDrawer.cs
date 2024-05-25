using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
