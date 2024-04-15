using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ContextSteering : MonoBehaviour
{
    public int NumberOfRays = 8;
    public bool Use2DPhysics = false;

    [Header("Danger Settings")]
    public LayerWeightList DangerWeights;
    public float DangerRange;
    public AnimationCurve DangerCurve;

    [Header("Debug")]
    public bool DoDebug;
    public Color DebugColor = Color.yellow;
    public Color InterestColor = Color.green;
    public Color DangerColor = Color.red;
    public Color ResultColor = Color.magenta;
    
    /// <summary>
    /// The SO to update with the calculated context steering results
    /// </summary>
    private Vector3Variable m_DirectionResult;

    /// <summary>
    /// The directions to check for interests and dangers
    /// </summary>
    private Vector3[] m_DetectionDirections;

    /// <summary>
    /// The target we want to steer towards
    /// </summary>
    private Vector3 m_InterestTarget;

    /// <summary>
    /// The projected interest directions
    /// </summary>
    private float[] m_InterestProjections;

    /// <summary>
    /// The discovered dangers around the owner
    /// </summary>
    private Collider2D[] m_DiscoveredDangers;

    /// <summary>
    /// The projected danager directions
    /// </summary>
    private float[] m_DangerProjections;

    private Coroutine m_SteeringCoroutine;
    private bool m_IsSteering;

    /// <summary>
    /// Check if the ContextSteering is actively running
    /// </summary>
    /// <returns>True if context steering is running; false otherwise</returns>
    public bool IsSteeringActive()
    {
        return m_IsSteering;
    }

    /// <summary>
    /// Start context steering towards the interestTarget
    /// </summary>
    /// <param name="interestTarget">The target we are interested in moving towards</param>
    /// <param name="directionResult">The Vector3Variable that is updated with the direction to move</param>
    public void StartSteering(Vector3 interestTarget, Vector3Variable directionResult)
    {
        if (interestTarget == null || directionResult == null) return;

        m_InterestTarget = interestTarget;
        m_DirectionResult = directionResult;

        StopSteering();
        m_SteeringCoroutine = StartCoroutine(Steering());

        m_IsSteering = true;
    }

    /// <summary>
    /// Stop context steering
    /// </summary>
    public void StopSteering()
    {
        if (m_SteeringCoroutine != null)
        {
            StopCoroutine(m_SteeringCoroutine);
            m_SteeringCoroutine = null;
        }

        if (m_DetectionDirections != null)
        {
            m_DirectionResult.Value = Vector3.zero;
        }

        m_IsSteering = false;
    }

    /// <summary>
    /// Coroutine that drives the context steering
    /// </summary>
    private IEnumerator Steering()
    {
        while (true)
        {
            InterestProjections();
            DangerProjections();
            DetermineDirection();

            if (DoDebug)
            {
                DebugDraw();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Calculate the projection of the m_InterestTarget on the m_DetectionDirections
    /// </summary>
    private void InterestProjections()
    {
        if (m_InterestTarget == null) return;

        Vector3 directionToInterest = m_InterestTarget - transform.position;

        for (int i = 0; i < NumberOfRays; i++)
        {
            float projection = Vector3.Dot(m_DetectionDirections[i], directionToInterest) / directionToInterest.magnitude;

            m_InterestProjections[i] = Mathf.Max(0.1f, projection);
        }
    }

    /// <summary>
    /// Calculate the projection of the m_DiscoveredDanagers on the m_DectionsDirections
    /// </summary>
    private void DangerProjections()
    {
        Debug.Log($"Danger Projection Layer: {DangerWeights.LayerMask.value}");

        for (int i = 0; i < NumberOfRays; ++i)
        {
            float projection = 0;
            LayerMask hitMask = LayerMask.GetMask("Default");

            // Reset the danger
            m_DangerProjections[i] = 0f;

            if (Use2DPhysics)
            {
                RaycastHit2D result2D = Physics2D.Raycast(origin: transform.position, direction: m_DetectionDirections[i], distance: DangerRange, layerMask: DangerWeights.LayerMask);
                if (result2D)
                {
                    projection = 1 - (result2D.distance / DangerRange);
                    hitMask = result2D.transform.gameObject.layer;
                }
            }
            else
            {
                bool hit = Physics.Raycast(origin: transform.position, direction: m_DetectionDirections[i], hitInfo: out RaycastHit result, maxDistance: DangerRange, layerMask: DangerWeights.LayerMask);
                if (hit)
                {
                    projection = 1 - (result.distance / DangerRange);
                    hitMask = result.transform.gameObject.layer;
                }
            }
            
            Debug.Log($"Yo I just hit layer: {hitMask.value} {LayerMask.LayerToName(hitMask)}");
            
            m_DangerProjections[i] = DangerCurve.Evaluate(projection) * DangerWeights.GetLayerWeight(hitMask);
        }
    }

    /// <summary>
    /// Determine the direction to steer
    /// </summary>
    private void DetermineDirection()
    {
        if (m_DirectionResult == null) return;

        m_DirectionResult.Value = Vector3.zero;

        for (int i = 0; i < NumberOfRays; ++i)
        {
            m_DirectionResult.Value += m_DetectionDirections[i] * (m_InterestProjections[i] - m_DangerProjections[i]);
        }

        m_DirectionResult.Value.Normalize();
    }

    private void DebugDraw()
    {
        for (int i = 0; i < NumberOfRays; i++)
        {
            Vector3 debug = m_DetectionDirections[i].normalized;
            Debug.DrawRay(transform.position, debug, DebugColor);

            Vector3 interest = m_DetectionDirections[i] * m_InterestProjections[i];
            Debug.DrawRay(transform.position, interest, InterestColor);

            Vector3 danger = m_DetectionDirections[i] * m_DangerProjections[i];
            Debug.DrawRay(transform.position, danger, DangerColor);
        }

        if (m_DirectionResult != null)
        {
            Debug.DrawRay(transform.position, m_DirectionResult.Value, ResultColor);
        }
    }

    private void Start()
    {
        m_InterestTarget = Vector3.zero;
        m_InterestProjections = new float[NumberOfRays];

        m_DirectionResult = null;

        m_DetectionDirections = new Vector3[NumberOfRays];
        
        m_DangerProjections = new float[NumberOfRays];
        m_DiscoveredDangers = new Collider2D[NumberOfRays];

        // Create the detection rays
        for (int i = 0; i < NumberOfRays; i++)
        {
            float angle = i * 2 * Mathf.PI / NumberOfRays;
            m_DetectionDirections[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        m_IsSteering = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = DangerColor;
        Gizmos.DrawWireSphere(transform.position, DangerRange);
    }
}


#if UNITY_EDITOR
//[CustomEditor(typeof(ContextSteering))]
public class ContextSteeringEditor : Editor
{
    private SerializedProperty NumberOfRaysProp;
    private SerializedProperty Use2DPhysicsProp;
    private SerializedProperty DangerLayerProp;
    private SerializedProperty DangerRangeProp;
    private SerializedProperty DangerWeightProp;
    private SerializedProperty DangerCurveProp;
    private SerializedProperty DoDebugProp;
    private SerializedProperty DebugColorProp;
    private SerializedProperty InterestColorProp;
    private SerializedProperty DangerColorProp;
    private SerializedProperty ResultColorProp;

    private void OnEnable()
    {
        NumberOfRaysProp = serializedObject.FindProperty("NumberOfRays");
        Use2DPhysicsProp = serializedObject.FindProperty("Use2DPhysics");
        DangerLayerProp = serializedObject.FindProperty("DangerLayer");
        DangerRangeProp = serializedObject.FindProperty("DangerRange");
        DangerWeightProp = serializedObject.FindProperty("DangerWeight");
        DangerCurveProp = serializedObject.FindProperty("DangerCurve");
        DoDebugProp = serializedObject.FindProperty("DoDebug");
        DebugColorProp = serializedObject.FindProperty("DebugColor");
        InterestColorProp = serializedObject.FindProperty("InterestColor");
        DangerColorProp = serializedObject.FindProperty("DangerColor");
        ResultColorProp = serializedObject.FindProperty("ResultColor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(NumberOfRaysProp);
        EditorGUILayout.PropertyField(Use2DPhysicsProp);
        EditorGUILayout.PropertyField(DangerLayerProp);
        EditorGUILayout.PropertyField(DangerRangeProp);
        EditorGUILayout.PropertyField(DangerWeightProp);
        EditorGUILayout.PropertyField(DangerCurveProp);
        EditorGUILayout.PropertyField(DoDebugProp);
        EditorGUILayout.PropertyField(DebugColorProp);
        EditorGUILayout.PropertyField(InterestColorProp);
        EditorGUILayout.PropertyField(DangerColorProp);
        EditorGUILayout.PropertyField(ResultColorProp);

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
