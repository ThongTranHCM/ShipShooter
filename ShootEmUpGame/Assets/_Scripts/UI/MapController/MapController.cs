using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Transform itemParentTf;
    public LineRenderer mapLineRenderer;

    [ContextMenu("Update MapLine")]
    public void UpdateMapLine()
    {
        mapLineRenderer.positionCount = itemParentTf.childCount;
        for (int i = 0; i < itemParentTf.childCount; i++)
        {
            mapLineRenderer.SetPosition(i, itemParentTf.GetChild(i).position);
        }
    }
}
