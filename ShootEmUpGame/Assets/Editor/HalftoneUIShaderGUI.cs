using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtil;
using UnityEditor;

public class HalftoneUIShaderGUI : ShaderGUI 
{
    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI (materialEditor, properties);
        Material targetMat = materialEditor.target as Material;
        
        float _hueShift = targetMat.GetFloat("_HueShift");
        float _contrast = targetMat.GetFloat("_Contrast");
        Color _litHue = targetMat.GetColor("_LitHue");
        Color _shadeHue = targetMat.GetColor("_ShadeHue");

        
        //Color _lit = MathUtil.ColorMath.HueShift(_base, _litHue, _hueShift, _contrast);
        Color _lit = targetMat.GetColor("_Color");
        _contrast *= MathUtil.ColorMath.Value(_lit);
        Color _shade = MathUtil.ColorMath.HueShift(_lit, _shadeHue, _hueShift * _contrast * (1 - MathUtil.ColorMath.Chroma(_lit)), -_contrast);
        targetMat.SetColor("_Lit", _lit);
        targetMat.SetColor("_Shade", _shade);
    }
}