using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ValueTestingCamera : MonoBehaviour
{
    public Material _mat;
    // Start is called before the first frame update
    void Start()
    {
        _mat = new Material(Shader.Find("Unlit/MonochromeShader") );
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit (source, destination, _mat);
    }
}
