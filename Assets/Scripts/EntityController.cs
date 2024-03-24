using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Reflection;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityController : MonoBehaviour // IInputListener
{
    public PathRequestEvent PathRequestEvent;
    public float RerequestDistance;
    public Transform Target;

    public float Speed;

    [Header("Input Settings")]
    public InputReader InputReader;
    public string PositiveXAction;
    public string NegativeXAction;
    public string PositiveYAction;
    public string NegativeYAction;

    [Header("Debug")]
    public Color DebugPathColor;
    private List<Vector3> mDebugPath;

    private Vector3 mVelocity;

    private bool m_IsRequesting;

    public void OnPositiveXAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mVelocity.x = 1;
        }

        if (context.canceled)
        {
            mVelocity.x = 0;
        }
    }

    public void OnNegativeXAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mVelocity.x = -1;
        }

        if (context.canceled)
        {
            mVelocity.x = 0;
        }
    }

    public void OnPositiveYAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mVelocity.y = 1;
        }

        if (context.canceled)
        {
            mVelocity.y = 0;
        }
    }

    public void OnNegativeYAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mVelocity.y = -1;
        }

        if (context.canceled)
        {
            mVelocity.y = 0;
        }
    }

    public void OnPathFound(List<Vector3> path, bool pathSuccess)
    {
        Debug.Log($"Path Received! {pathSuccess}");
        if (pathSuccess)
        {
            Debug.Log($"Path length: {path.Count}");
            mDebugPath = path;
        }

        m_IsRequesting = false;
    }

    private void RequestPath(Vector3 start, Vector3 end)
    {
        PathRequest request = new(1, start, end, OnPathFound);
        PathRequestEvent.RaiseEvent(request);
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

    /// <summary>
    /// Check if the conditions for requesting a path are met
    /// </summary>
    /// <returns>True if conditions are met; false otherwise</returns>
    private bool CheckRequestConditions()
    {
        if (Target == null)
            return false;

        float targetDistanceFromLastPathPoint = Vector3Int.Distance(Vector3Int.FloorToInt(Target.position), 
                                                                    Vector3Int.FloorToInt(mDebugPath[^1]));
        return m_IsRequesting && targetDistanceFromLastPathPoint > RerequestDistance;
    }

    private void OnEnable()
    {
        mDebugPath = new();
        mVelocity = Vector3.zero;
        m_IsRequesting = false;

        RegisterActionCallback(PositiveXAction, OnPositiveXAction);
        RegisterActionCallback(NegativeXAction, OnNegativeXAction);
        RegisterActionCallback(PositiveYAction, OnPositiveYAction);
        RegisterActionCallback(NegativeYAction, OnNegativeYAction);
    }

    private void OnDisable()
    {
        UnregisterActionCallback(PositiveXAction, OnPositiveXAction);
        UnregisterActionCallback(NegativeXAction, OnNegativeXAction);
        UnregisterActionCallback(PositiveYAction, OnPositiveYAction);
        UnregisterActionCallback(NegativeYAction, OnNegativeYAction);
    }

    private void Update()
    {
        //Debug.Log(mVelocity);
        transform.position += Speed * Time.deltaTime * mVelocity.normalized;

        if (CheckRequestConditions())
        {
            RequestPath(transform.position, Target.position);
            m_IsRequesting = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (mDebugPath != null && mDebugPath.Count != 0)
        {
            // Draw the path
            for (int i = 1; i < mDebugPath.Count; i++)
            {
                Vector3 point = mDebugPath[i];
                point.x += 0.5f;
                point.y += 0.5f;

                Vector3 nextPoint = mDebugPath[i - 1];
                nextPoint.x += 0.5f;
                nextPoint.y += 0.5f;

                Gizmos.color = DebugPathColor;
                Gizmos.DrawWireSphere(point, 0.05f);
                Gizmos.DrawLine(nextPoint, point);
            }

            // Draw rerequest distance
            Gizmos.color = DebugPathColor;
            Vector3 spot = mDebugPath[^1];
            spot.x += 0.5f;
            spot.y += 0.5f;

            Gizmos.DrawWireSphere(spot, RerequestDistance);
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
