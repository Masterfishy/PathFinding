/// <summary>
/// This interface defines the API for a map that supports search algorithms
/// </summary>
public interface ISearchableMap 
{
    /// <summary>
    /// Get the neighbors around a given position
    /// </summary>
    /// <param name="position">The position to find neighbors of</param>
    /// <returns>An array of <c>ISearchablePosition</c>s that are neighbors of the given position</returns>
    public ISearchablePosition[] GetNeighbors(ISearchablePosition position);

    /// <summary>
    /// Get the collection of <c>ISearchablePosition</c>s in the map
    /// </summary>
    public ISearchablePosition[] MapPositions { get; }
}
