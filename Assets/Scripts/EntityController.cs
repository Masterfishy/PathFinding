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
    private List<ISearchablePosition> mDebugPath;

    public void OnPathFound(List<ISearchablePosition> path, bool pathSuccess)
    {
        Debug.Log($"Path Received! {pathSuccess}");
        if (pathSuccess)
        {
            Debug.Log($"Path length: {path.Count}");
            mDebugPath = path;
        }
    }

    private void RequestPath(ISearchablePosition start, ISearchablePosition end)
    {
        PathRequest request = new(1, start, end, OnPathFound);
        PathRequestEvent.RaiseEvent(request);
    }

    private void OnEnable()
    {
        mDebugPath = new();
    }

    private void Update()
    {
        if (mDebugPath.Count > 1 &&
            (Vector3Int.Distance(Vector3Int.FloorToInt(transform.position), 
                                 Vector3Int.FloorToInt(mDebugPath[0].Position)) > 2f ||
             Vector3Int.Distance(Vector3Int.FloorToInt(EndPos.position), 
                                 Vector3Int.FloorToInt(mDebugPath[^1].Position)) > 2f))
        {
            RequestPath(new AStarPosition(Vector3Int.FloorToInt(transform.position)), new AStarPosition(Vector3Int.FloorToInt(EndPos.position)));
            return;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            return;
        }

        // Draw the path
        for (int i = 1; i < mDebugPath.Count; i++)
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
