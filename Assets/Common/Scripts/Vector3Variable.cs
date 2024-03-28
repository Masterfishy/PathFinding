using UnityEngine;

/// <summary>
/// This class represents a serializable Vector3 variable
/// </summary>
[CreateAssetMenu(menuName = "Variables/Vector3")]
public class Vector3Variable : ScriptableObject
{
    public Vector3 Value;

    public static implicit operator Vector3(Vector3Variable reference)
    {
        return reference.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
