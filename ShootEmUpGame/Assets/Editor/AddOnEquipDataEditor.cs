using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AddOnEquipData))]
public class AddOnEquipDataEditor : Editor
{
    AddOnEquipData targetData;
    string csvDir = "LevelDesign - AddOnLevelUp";
    private SerializedProperty upgradeFragmentCostProperty;
    private SerializedProperty unlockFragmentCostProperty;
    private SerializedProperty levelPowersProperty;
    public void OnEnable()
    {
        upgradeFragmentCostProperty = serializedObject.FindProperty("_upgradeFragmentCosts");
        unlockFragmentCostProperty = serializedObject.FindProperty("_unlockFragmentCost");
        levelPowersProperty = serializedObject.FindProperty("_levelPowers");
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Editor " + " AddOnEquipDataEditor");
        base.OnInspectorGUI();
        targetData = (target as AddOnEquipData);
        GUILayout.Label("From file " + csvDir + ".csv");
        csvDir = EditorGUILayout.TextField(csvDir);
        if (GUILayout.Button("Update Cost"))
        {
            LoadPowerAndCostCSV();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void LoadPowerAndCostCSV()
    {
        List<Dictionary<string, string>> sheet = CSVReader.Read(csvDir);
        upgradeFragmentCostProperty.ClearArray();
        levelPowersProperty.ClearArray();
        int i = 0;
        foreach (Dictionary<string, string> row in sheet)
        {
            int fragmentCount = int.Parse(row["Fragment"].ToString());
            float power = int.Parse(row["Power"].ToString());
            levelPowersProperty.InsertArrayElementAtIndex(i);
            upgradeFragmentCostProperty.InsertArrayElementAtIndex(i);
            levelPowersProperty.GetArrayElementAtIndex(i).floatValue = power;
            if (i == 0)
            {
                unlockFragmentCostProperty.intValue = fragmentCount;
            }
            else
            {
                upgradeFragmentCostProperty.GetArrayElementAtIndex(i).intValue = fragmentCount;
            }
            i++;
            if (i == targetData.MaxLevel)
            {
                break;
            }
        }
    }
}