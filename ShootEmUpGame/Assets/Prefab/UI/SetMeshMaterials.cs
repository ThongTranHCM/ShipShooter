using UnityEngine;
using System.Collections.Generic;
 
[RequireComponent(typeof(CanvasRenderer))]
[ExecuteInEditMode]
public class SetMeshMaterials : MonoBehaviour
{
    public Mesh TheMesh;
    public List<Material> Materials;
 
    void ResetData ()
    {
        var cr = GetComponent<CanvasRenderer>();
        cr.SetMesh (TheMesh);
        cr.materialCount = Materials.Count;
 
        for (int i = 0; i < Materials.Count; i++)
            cr.SetMaterial (Materials[i], i);
    }
 
    void OnEnable()
    {
        ResetData();
    }
 
    void OnValidate()
    {
        ResetData();
    }
}
 
 