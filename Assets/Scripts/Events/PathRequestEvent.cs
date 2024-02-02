using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class represents an event to request a path
/// </summary>
[CreateAssetMenu(menuName ="Events/Path Request Event")]
public class PathRequestEvent : ScriptableObject
{
    public UnityAction<PathRequest> OnEventRaised;

    /// <summary>
    /// Invoke the event with the start and end position
    /// </summary>
    /// <param name="payload">The request payload</param>
    public void RaiseEvent(PathRequest payload)
    {
        if (OnEventRaised != null)
        {
            OnEventRaised.Invoke(payload);
        }
    }
}
