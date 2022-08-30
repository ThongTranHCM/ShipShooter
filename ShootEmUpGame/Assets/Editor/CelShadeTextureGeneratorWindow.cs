using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CelShadeTextureGeneratorWindow : EditorWindow
{
    private string _texDirectory;
    private Material _mat;
    private string[] _files;
    private string _litDirectory;
    private string _unlitDirectory;
    private RenderTexture _savedRenderTexture;
    private Color _litHue;
    private Color _unlitHue;
    private float _hueShift;
    [MenuItem ("Window/Cel Shade Texture Generator")]
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(CelShadeTextureGeneratorWindow));
    }

    void ExportTexture2D(Texture2D src, string path){
        var Bytes = src.EncodeToPNG();
        File.WriteAllBytes(path, Bytes);
    }

    void ExportRenderTexture(string path){
        Texture2D tex = new Texture2D(_savedRenderTexture.width, _savedRenderTexture.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = _savedRenderTexture;
        tex.ReadPixels(new Rect(0, 0, _savedRenderTexture.width, _savedRenderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = old;
        ExportTexture2D(tex, path);
    }
    
    string Absolute2RelativeDirectory(string path){
        return "Assets" + path.Substring(Application.dataPath.Length);
    }

    void SetMaterialParam(Color TargetHue, float HueShift, float PercentLumiShift, float LumiShift){
        _mat.SetColor("_TargetHue", TargetHue);
        _mat.SetFloat("_HueShift", HueShift);
        _mat.SetFloat("_PercentLumiShift", PercentLumiShift);
        _mat.SetFloat("_LumiShift", LumiShift);
    }

    RenderTexture RenderModifiedTexture(Texture2D src){
        _savedRenderTexture.Release();
        _savedRenderTexture.width = src.height;
        _savedRenderTexture.height = src.height;
        RenderTexture old = RenderTexture.active;
        RenderTexture.active = _savedRenderTexture;
        _savedRenderTexture.Create();
        Graphics.Blit(src, _savedRenderTexture,_mat);
        RenderTexture.active = old;
        return _savedRenderTexture;
    }
    
    void OnGUI () {
        _files = new string[0];
        string _matPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("CelShadeWindow")[0]);
        string _rTexPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("TempRenderTexture")[0]);
        Debug.Log(_matPath);
        Debug.Log(_rTexPath);
        _mat = (Material)AssetDatabase.LoadAssetAtPath(_matPath,typeof(Material));
        _savedRenderTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath(_rTexPath,typeof(RenderTexture));
        _hueShift = EditorGUILayout.FloatField("Hue Shift", _hueShift);
        _litHue = EditorGUILayout.ColorField("Lit Hue", _litHue);
        _unlitHue = EditorGUILayout.ColorField("Unlit Hue", _unlitHue);
        GUILayout.BeginVertical();
        GUILayout.Label("Directory");
        GUILayout.BeginHorizontal();
        GUILayout.TextField(_texDirectory);
        if (GUILayout.Button("Select Directory")){
            _texDirectory = EditorUtility.OpenFolderPanel("Select the Base Texture Directory", "", "");
        }
        try{
            _files = Directory.GetFiles(_texDirectory, "*.png", SearchOption.TopDirectoryOnly);
        } catch (Exception e)  {
            Debug.Log(e);
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Found " + _files.Length + " base Textures.");
        GUILayout.Label("Lit Directory");
        GUILayout.BeginHorizontal();
        GUILayout.TextField(_litDirectory);
        if (GUILayout.Button("Select Directory")){
            _litDirectory = EditorUtility.OpenFolderPanel("Select the Lit Directory", "", "");
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Unlit Directory");
        GUILayout.BeginHorizontal();
        GUILayout.TextField(_unlitDirectory);
        if (GUILayout.Button("Select Directory")){
            _unlitDirectory = EditorUtility.OpenFolderPanel("Select the Unlit Directory", "", "");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        if (Directory.Exists(_unlitDirectory) && Directory.Exists(_litDirectory)){
            if(GUILayout.Button("Generate")){
                foreach(string file in _files){
                    string fileName = Path.GetFileName(file);
                    Texture2D baseTex = (Texture2D)AssetDatabase.LoadAssetAtPath(Absolute2RelativeDirectory(file), typeof(Texture2D));

                    SetMaterialParam(_litHue, _hueShift,0 , 0.2f);
                    _savedRenderTexture = RenderModifiedTexture(baseTex);
                    ExportRenderTexture(_litDirectory + "/" +fileName);

                    SetMaterialParam(_unlitHue, _hueShift, -0.4f, 0);
                    _savedRenderTexture = RenderModifiedTexture(baseTex);
                    ExportRenderTexture(_unlitDirectory + "/" +fileName);
                }
                AssetDatabase.Refresh();
                SaveChanges();
            }
        }
    }
}
