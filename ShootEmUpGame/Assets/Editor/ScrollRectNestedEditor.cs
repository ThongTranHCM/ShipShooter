using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
 
[CustomEditor(typeof(ScrollRectNested))]
public class ScrollRectNestedEditor : ScrollRectEditor
{
    SerializedProperty parentScrollRectProp;
    SerializedProperty scrollSnapRectProp;
    GUIContent parentScrollRectGUIContent = new GUIContent("Parent ScrollRect");
    GUIContent scrollSnapRectGUIContent = new GUIContent("ScrollSnap");

    protected override void OnEnable()
    {
        base.OnEnable();
        parentScrollRectProp = serializedObject.FindProperty("parentScrollRect");
        scrollSnapRectProp = serializedObject.FindProperty("scrollSnap");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(parentScrollRectProp, parentScrollRectGUIContent);
        EditorGUILayout.PropertyField(scrollSnapRectProp, scrollSnapRectGUIContent);
        serializedObject.ApplyModifiedProperties();
    }
}