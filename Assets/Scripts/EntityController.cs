using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityController : MonoBehaviour, IPathRequester // IInputListener
{
    public PathRequestEvent PathRequestEvent;
    public float PointArrivalDistance;

    public float Speed;

    [Header("Input Settings")]
    public InputReader InputReader;

    [Header("Debug")]
    public Color DebugPathColor;

    private int m_CurrentPathPointIndex;
    private List<Vector3> m_Path;

    private ContextSteering m_ContextSteering;
    private Vector3Variable m_MoveDirection;

    public void OnLeftClickAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            RequestPath(transform.position, mousePos);
        }
    }

    public void OnPathFound(PathResponse response)
    {
        Debug.Log($"Path Received! {response.PathSuccess}");
        if (response.PathSuccess)
        {
            Debug.Log($"Path length: {response.Path.Count}");
            m_Path = response.Path;
        }

        OnFollowNewPath();
    }

    public void OnFollowNewPath()
    {
        if (m_Path == null || m_Path.Count <= 0)
        {
            OnFinishedFollowing();
            return;
        }

        m_CurrentPathPointIndex = 0;

        OnFollowNextPoint();
    }

    public void OnFollowNextPoint()
    {
        if (m_ContextSteering == null || m_MoveDirection == null)
        {
            return;
        }

        m_CurrentPathPointIndex++;

        if (m_CurrentPathPointIndex >= m_Path.Count)
        {
            OnFinishedFollowing();
            return;
        }

        m_ContextSteering.StartSteering(m_Path[m_CurrentPathPointIndex], m_MoveDirection);
    }

    public void OnFinishedFollowing()
    {
        if (m_ContextSteering != null && m_ContextSteering.IsSteeringActive())
        {
            m_ContextSteering.StopSteering();
        }
    }

    private void RequestPath(Vector3 start, Vector3 end)
    {
        PathRequest request = new(this, start, end);
        PathRequestEvent.RaiseEvent(request);
    }

    private void FollowPath()
    {
        if (m_Path == null || m_Path.Count <= 0 ||
            m_ContextSteering == null || !m_ContextSteering.IsSteeringActive())
        {
            return;
        }

        // If we reach the current point
        float distanceToPoint = Vector3.Distance(transform.position, m_Path[m_CurrentPathPointIndex]);
        if (distanceToPoint < PointArrivalDistance)
        {
            OnFollowNextPoint();
        }
    }

    /// <summary>
    /// Use reflection to find a UnityAction named ActionName from InputReader and register the corresponding EntityController callback
    /// </summary>
    /// <param name="actionName">The name of the UnityAction in InputReader</param>
    /// <param name="actionCallback">The callback to register to the UnityAction</param>
    private void RegisterActionCallback(string actionName, UnityAction<InputAction.CallbackContext> actionCallback)
    {
        if (actionName == null || actionCallback == null || InputReader == null)
            return;

        FieldInfo fieldInfo = InputReader.GetType().GetField(actionName, BindingFlags.Public | BindingFlags.Instance);
        //Debug.Log($"Field Info {fieldInfo}");
        if (fieldInfo != null)
        {
            //Debug.Log($"Field Value {fieldInfo.GetValue(InputReader)}");
            if (fieldInfo.GetValue(InputReader) is UnityEvent<InputAction.CallbackContext> fieldValue)
            {
                //Debug.Log($"Registered {actionCallback} to {fieldValue}");
                fieldValue.AddListener(actionCallback);
            }
        }
    }

    /// <summary>
    /// Use reflection to find a UnityAction named ActionName from InputReader and unregister the corresponding EntityController callback
    /// </summary>
    /// <param name="actionName">The name of the UnityAction in InputReader</param>
    /// <param name="actionCallback">The callback to unregister from the UnityAction</param>
    private void UnregisterActionCallback(string actionName, UnityAction<InputAction.CallbackContext> actionCallback)
    {
        if (actionName == null || actionCallback == null || InputReader == null)
            return;

        FieldInfo fieldInfo = typeof(InputReader).GetField(actionName, BindingFlags.Public | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            if (fieldInfo.GetValue(InputReader) is UnityEvent<InputAction.CallbackContext> fieldValue)
            {
                fieldValue.RemoveListener(actionCallback);
            }
        }
    }

    private void Start()
    {
        m_MoveDirection = ScriptableObject.CreateInstance<Vector3Variable>();

        TryGetComponent(out m_ContextSteering);
    }

    private void OnEnable()
    {
        m_Path = new();
        m_CurrentPathPointIndex = -1;

        InputReader.InputEventLeftClick.AddListener(OnLeftClickAction);
    }

    private void OnDisable()
    {
        InputReader.InputEventLeftClick.RemoveListener(OnLeftClickAction);

        OnFinishedFollowing();
    }

    private void Update()
    {
        transform.position += Speed * Time.deltaTime * m_MoveDirection.Value;

        FollowPath();
    }

    private void OnDrawGizmos()
    {
        if (m_Path != null && m_Path.Count != 0)
        {
            // Draw the path
            for (int i = 1; i < m_Path.Count; i++)
            {
                Vector3 point = m_Path[i];
                Vector3 nextPoint = m_Path[i - 1];

                Gizmos.color = DebugPathColor;
                Gizmos.DrawWireSphere(point, PointArrivalDistance);
                Gizmos.DrawLine(nextPoint, point);
            }
        }
    }
}

#if UNITY_EDITOR
//[CustomEditor(typeof(EntityController))]
public class EntityControllerEditor : Editor
{
    private SerializedProperty serializedProperty;

    private void OnEnable()
    {
        serializedProperty = serializedObject.GetIterator();
        serializedProperty.Next(true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        while (serializedProperty.NextVisible(false))
        {
            EditorGUILayout.PropertyField(serializedProperty, true);
        }

        serializedObject.ApplyModifiedProperties();

        //Transform startPos = StartPosProp.objectReferenceValue as Transform;
        //Transform endPos = EndPosProp.objectReferenceValue as Transform;

        //EntityController targetEvent = target as EntityController;

        //if (GUILayout.Button("Request Path") && startPos != null && endPos != null)
        //{
        //    PathRequest request = new(1, new AStarPosition(Vector3Int.FloorToInt(startPos.position)), new AStarPosition(Vector3Int.FloorToInt(endPos.position)), targetEvent.OnPathFound);
        //    targetEvent.PathRequestEvent.RaiseEvent(request);
        //}
        
    }
}
#endif
