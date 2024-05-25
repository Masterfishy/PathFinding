using UnityEditor;

[CustomEditor(typeof(LayerWeightList))]
public class LayerWeightListEditor : Editor
{
    private SerializedProperty LayerWeightEntriesProp;

    private void OnEnable()
    {
        LayerWeightEntriesProp = serializedObject.FindProperty("LayerWeightEntries");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(LayerWeightEntriesProp);

        if (serializedObject.ApplyModifiedProperties())
        {
            LayerWeightList list = target as LayerWeightList;

            list.GenerateLayerWeight();
        }
    }
}
