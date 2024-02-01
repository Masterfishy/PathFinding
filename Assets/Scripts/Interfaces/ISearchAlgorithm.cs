using System;

/// <summary>
/// This interface defines the API of a search algorithm
/// </summary>
public interface ISearchAlgorithm
{
    /// <summary>
    /// Find a path from the start position to the end position.
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="end">The end position</param>
    /// <param name="map">The search able map the positions are found on</param>
    /// <param name="callback">The callback to report the found path</param>
    public void FindPath(ISearchablePosition start, ISearchablePosition end, ISearchableMap map, Action<ISearchablePosition[]> callback);
}
