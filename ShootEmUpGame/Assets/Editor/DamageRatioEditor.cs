using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DamageTypeRatio))]
public class DamageRatioEditor : PropertyDrawer
{
    bool showContent = false;
    bool inited = false;
    int numEnum = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!inited) InitValues(property);
        EditorGUI.BeginProperty(position, label, property);
        Rect labelRect = position, ratioRect = position, afterRect = position;
        showContent = EditorGUI.BeginFoldoutHeaderGroup(new Rect(position.x, position.y, 100, 20), showContent, label.text);

        UnityEngine.GUIContent percentageContent = new UnityEngine.GUIContent();
        percentageContent.text = "%";
        UnityEngine.GUIContent triggerDamageContent = new UnityEngine.GUIContent();
        triggerDamageContent.text = "Trigger Damage Ratio";
        if (showContent)
        {
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            // Get the properties
            SerializedProperty damageSources = property.FindPropertyRelative("damageSources");
            SerializedProperty ratios = property.FindPropertyRelative("ratios");
            SerializedProperty triggerDamageRatio = property.FindPropertyRelative("triggerDamageRatio");
            // Calculate rects

            labelRect = new Rect(position.x, position.y + 22, 120, 20);
            ratioRect = EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), triggerDamageContent);
            ratioRect = new Rect(ratioRect.x, ratioRect.y, 70, 20);
            EditorGUI.PropertyField(ratioRect, triggerDamageRatio, GUIContent.none);
            afterRect = new Rect(ratioRect.x + 42, ratioRect.y, 50, 20);
            EditorGUI.LabelField(afterRect, percentageContent);
            if (GUI.Button(new Rect(ratioRect.x, ratioRect.y - 22, 120, 20), "Reset Values"))
            {
                InstantiateValues(property);
            }
            for (int i = 0; i < numEnum; i++)
            {
                UnityEngine.GUIContent labelContent = new UnityEngine.GUIContent();
                labelRect.y += 22;
                labelContent.text = damageSources.GetArrayElementAtIndex(i).enumDisplayNames[damageSources.GetArrayElementAtIndex(i).enumValueIndex];
                EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), labelContent);
                ratioRect.y += 22;
                EditorGUI.PropertyField(ratioRect, ratios.GetArrayElementAtIndex(i), GUIContent.none);
                afterRect.y += 22;
                EditorGUI.LabelField(afterRect, percentageContent);
            }
        }
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty damageSources = property.FindPropertyRelative("damageSources");
        float totalHeight = 22;// EditorGUI.GetPropertyHeight(property, label, true);// + EditorGUIUtility.standardVerticalSpacing;
        if (showContent)
        {
            totalHeight += 22 * damageSources.arraySize + 22;
        }
        return totalHeight;
    }

    public void InstantiateValues(SerializedProperty property)
    {
        SerializedProperty triggerDamageRatio = property.FindPropertyRelative("triggerDamageRatio");
        triggerDamageRatio.floatValue = 1000;
        SerializedProperty damageSources = property.FindPropertyRelative("damageSources");
        SerializedProperty ratios = property.FindPropertyRelative("ratios");
        System.Array array = System.Enum.GetValues(typeof(ApplyEffectData.DamageSource));
        numEnum = array.Length;
        damageSources.ClearArray();
        ratios.ClearArray();
        int index = 0;
        float defaultValue = 100;
        foreach (int i in array)
        {
            damageSources.InsertArrayElementAtIndex(index);
            damageSources.GetArrayElementAtIndex(index).enumValueIndex = index;
            if (damageSources.GetArrayElementAtIndex(index).intValue == (int)ApplyEffectData.DamageSource.FromMonster)
            {
                defaultValue = 0;
            }
            //Index from 0 continously to N. Even if the enum have space. So Enum = i 0, 1, 2, 100, 101, 200, 201 Index still 0, 1, 2, 3, 4, 5, 6.
            ratios.InsertArrayElementAtIndex(index);
            ratios.GetArrayElementAtIndex(index).floatValue = defaultValue;
            index++;
        }
    }

    public void InitValues(SerializedProperty property)
    {
        SerializedProperty damageSources = property.FindPropertyRelative("damageSources");
        numEnum = damageSources.arraySize;
    }
}