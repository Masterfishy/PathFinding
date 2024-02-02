using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityController : MonoBehaviour
{
    public PathRequestEvent PathRequestEvent;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Debug")]
    public Color DebugPathColor;
    private ISearchablePosition[] mDebugPath;

    public void OnPathFound(ISearchablePosition[] path, bool pathSuccess)
    {
        Debug.Log($"Path Received! {pathSuccess}");
        if (pathSuccess)
        {
            mDebugPath = path;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            return;
        }

        // Draw the path
        for (int i = 1; i < mDebugPath.Length; i++)
        {
            Gizmos.color = DebugPathColor;
            Gizmos.DrawSphere(mDebugPath[i].Position, 0.05f);
            Gizmos.DrawLine(mDebugPath[i - 1].Position, mDebugPath[i].Position);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityController))]
public class EntityControllerEditor : Editor
{
    public SerializedProperty PathRequestEventProp;
    public SerializedProperty DebugPathColorProp;

    public SerializedProperty StartPosProp;
    public SerializedProperty EndPosProp;

    private void OnEnable()
    {
        PathRequestEventProp = serializedObject.FindProperty("PathRequestEvent");
        DebugPathColorProp = serializedObject.FindProperty("DebugPathColor");
        StartPosProp = serializedObject.FindProperty("StartPos");
        EndPosProp = serializedObject.FindProperty("EndPos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(PathRequestEventProp);
        EditorGUILayout.PropertyField(DebugPathColorProp);
        EditorGUILayout.PropertyField(StartPosProp);
        EditorGUILayout.PropertyField(EndPosProp);

        serializedObject.ApplyModifiedProperties();

        Transform startPos = StartPosProp.objectReferenceValue as Transform;
        Transform endPos = EndPosProp.objectReferenceValue as Transform;

        EntityController targetEvent = target as EntityController;

        if (GUILayout.Button("Request Path") && startPos != null && endPos != null)
        {
            PathRequest request = new(1, new AStarPosition(Vector3Int.FloorToInt(startPos.position)), new AStarPosition(Vector3Int.FloorToInt(endPos.position)), targetEvent.OnPathFound);
            targetEvent.PathRequestEvent.RaiseEvent(request);
        }
        
    }
}
#endif
