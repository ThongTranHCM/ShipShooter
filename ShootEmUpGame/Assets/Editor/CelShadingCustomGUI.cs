using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CelShadingCustomGUI : ShaderGUI
{
    private Texture _mainTex;
    private Texture _litTex;
    private Texture _unlitTex;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        Material targetMat = materialEditor.target as Material;
        _mainTex = targetMat.GetTexture("_MainTex");
        string mainTexPath = AssetDatabase.GetAssetPath(_mainTex);
        string fileName = Path.GetFileName(mainTexPath);
        string parentPath = mainTexPath.Substring(0, mainTexPath.Length - fileName.Length);
        string litTexPath = parentPath + "Lit/" + fileName;
        string unlitTexPath = parentPath + "Unlit/" + fileName;
        _litTex = (Texture)AssetDatabase.LoadAssetAtPath(litTexPath, typeof(Texture));
        targetMat.SetTexture("_LitTex", _litTex);
        _unlitTex = (Texture)AssetDatabase.LoadAssetAtPath(unlitTexPath, typeof(Texture));
        targetMat.SetTexture("_UnlitTex", _unlitTex);
        materialEditor.Repaint();
    }
}
