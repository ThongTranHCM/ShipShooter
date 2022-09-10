using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DOShipData))]
public class CustomEditorShipData : Editor
{
    DOShipData targetData;
    string csvDir = "LevelDesign - Ship1";
    private SerializedProperty powerProperty;
    private SerializedProperty upgradeCostProperty;
    private SerializedProperty buyValueCostProperty;
    private SerializedProperty buyCurrencyProperty;
    public void OnEnable()
    {
        powerProperty = serializedObject.FindProperty("_shipPowers");
        upgradeCostProperty = serializedObject.FindProperty("_upgradeCosts");
        buyValueCostProperty = serializedObject.FindProperty("_buyCost");
        buyCurrencyProperty = serializedObject.FindProperty("_buyCurrency");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        targetData = (target as DOShipData);
        GUILayout.Label("From file " + csvDir + ".csv");
        csvDir = EditorGUILayout.TextField(csvDir);
        if (GUILayout.Button("Update Power and Cost"))
        {
            LoadPowerAndCostCSV();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void LoadPowerAndCostCSV()
    {
        List<Dictionary<string, string>> sheet = CSVReader.Read(csvDir);
        powerProperty.ClearArray();
        upgradeCostProperty.ClearArray();
        int i = 0;
        foreach (Dictionary<string, string> row in sheet)
        {
            int level = int.Parse(row["Level"].ToString());
            float power = float.Parse(row["Power"].ToString());
            float costValue = float.Parse(row["Gold"].ToString());
            powerProperty.InsertArrayElementAtIndex(i);
            powerProperty.GetArrayElementAtIndex(i).floatValue = power;
            upgradeCostProperty.InsertArrayElementAtIndex(i);
            if (i == 0)
            {
                buyValueCostProperty.floatValue = costValue;
                //buyCurrencyProperty.stringValue = row["Currency"].ToString();
            }
            else
            {
                upgradeCostProperty.GetArrayElementAtIndex(i).floatValue = costValue;
            }
            i++;
            if (i == targetData.MaxLevel)
            {
                break;
            }
        }
    }
}
