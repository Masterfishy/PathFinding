using System;
using UnityEngine;

/// <summary>
/// This container class wraps the ISearchablemap interface to allow objects
/// that implement the interface to be drag and dropped into the property field
/// </summary>
[Serializable]
public class SearchableMapUnityContainer
{
    [SerializeField]
    private UnityEngine.Object searchableMapObject;

    public ISearchableMap Contents
    {
        get { return searchableMapObject as ISearchableMap; }
        set { searchableMapObject = value as UnityEngine.Object; }
    }
}
