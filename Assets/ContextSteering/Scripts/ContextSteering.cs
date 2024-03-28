using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSteering : MonoBehaviour
{
    public LayerMask DangerLayerMask;
    public float DangerRange;
    public float DangerWeight;

    public int NumberOfRays = 8;

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

        m_SteeringCoroutine = StartCoroutine(Steering());
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
    /// Calculate the projection of the m_DiscoveredDanagers on teh m_DectionsDirections
    /// </summary>
    private void DangerProjections()
    {
        for (int i = 0; i < NumberOfRays; ++i)
        {
            // Reset the danger
            m_DangerProjections[i] = 0f;

            bool hit = Physics.Raycast(transform.position, m_DetectionDirections[i], out RaycastHit result, DangerRange, DangerLayerMask);
            if (hit)
            {
                float projection = 1 - (result.distance / DangerRange);
                m_DangerProjections[i] = projection * DangerWeight;
            }
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

        Gizmos.color = DangerColor;
        Gizmos.DrawWireSphere(transform.position, DangerRange);
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
    }
}
