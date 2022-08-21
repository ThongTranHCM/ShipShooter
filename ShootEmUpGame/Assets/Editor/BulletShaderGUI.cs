using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using MathUtil;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI 
{
    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI (materialEditor, properties);
        Material targetMat = materialEditor.target as Material;
        
        float _hueShift = targetMat.GetFloat("_HueShift");
        float _contrast = targetMat.GetFloat("_Contrast");
        Color _litHue = targetMat.GetColor("_LitHue");
        Color _shadeHue = targetMat.GetColor("_ShadeHue");

        Color _base = targetMat.GetColor("_MidColor");
        Color _lit = MathUtil.ColorMath.HueShift(_base, _litHue, _hueShift, _contrast);
        Color _unlit = MathUtil.ColorMath.HueShift(_base, _shadeHue, _hueShift, -_contrast);
        targetMat.SetColor("_StartColor", _unlit);
        targetMat.SetColor("_EndColor", _lit);
    }
}
#endif