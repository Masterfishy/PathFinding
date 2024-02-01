using UnityEngine;

/// <summary>
/// This interface defines the API for a position that is searchable
/// </summary>
public interface ISearchablePosition
{
    /// <summary>
    /// The positional data
    /// </summary>
    public Vector3 Position { get; }
}
