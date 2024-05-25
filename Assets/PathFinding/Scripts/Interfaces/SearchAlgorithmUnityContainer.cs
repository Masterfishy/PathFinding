using System;
using UnityEngine;

/// <summary>
/// This container class wraps the ISearchAlgorithm interface to allow objects
/// that implement the interface to be drag and dropped into the property field
/// </summary>
[Serializable]
public class SearchAlgorithmUnityContainer
{
    [SerializeField]
    private UnityEngine.Object searchAlgorithmObject;

    public ISearchAlgorithm Contents
    {
        get { return searchAlgorithmObject as ISearchAlgorithm; }
        set { searchAlgorithmObject = value as UnityEngine.Object; }
    }
}
