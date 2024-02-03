using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public SearchableMapUnityContainer MapContainer;

    private void OnEnable()
    {
        if (MapContainer != null && MapContainer.Contents is AStarMap map)
        {
            map.CreateGrid();
        }
    }

    private void OnDisable()
    {
        if (MapContainer != null && MapContainer.Contents is AStarMap map)
        {
            map.DestroyGrid();
        }
    }
}
