using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathRequester
{
    /// <summary>
    /// A callback for a path request
    /// </summary>
    /// <param name="response">The response to a path request</param>
    public void OnPathFound(PathResponse response);
}
