using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DOShipProgressData))]
public class CustomEditorShipProgress : Editor
{
    DOShipProgressData targetData;
    string powerCSV = "LevelDesign - ShipLevelUp";
    string costCSV = "LevelDesign - Ship Unlock Cost";
    private SerializedProperty powerProperty;
    private SerializedProperty upgradeCostProperty;
    //List<int> _buyCostList = serializedObject.FindProperty("m_Obj");
    public void OnEnable()
    {
        powerProperty = serializedObject.FindProperty("_shipPowers");
        upgradeCostProperty = serializedObject.FindProperty("_shipUpgradeCosts");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        targetData = (target as DOShipProgressData);
        GUILayout.Label("From file " + powerCSV + ".csv");
        if (GUILayout.Button("Update Power and Cost"))
        {
            LoadPowerAndUpgradeCSV();
        }
        GUILayout.Label("From file " + costCSV + ".csv");
        if (GUILayout.Button("Update Buy Cost"))
        {
            LoadBuyCostCSV();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void LoadPowerAndUpgradeCSV()
    {
        List<Dictionary<string, string>> sheet = CSVReader.Read(powerCSV);
        powerProperty.ClearArray();
        upgradeCostProperty.ClearArray();
        int i = 0;
        foreach (Dictionary<string, string> row in sheet)
        {
            int level = int.Parse(row["Level"].ToString());
            int power = int.Parse(row["Power"].ToString());
            int upgradeCost = int.Parse(row["Gold"].ToString());
            powerProperty.InsertArrayElementAtIndex(i);
            powerProperty.GetArrayElementAtIndex(i).intValue = power;
            upgradeCostProperty.InsertArrayElementAtIndex(i);
            upgradeCostProperty.GetArrayElementAtIndex(i).intValue = upgradeCost;
            i++;
            if (i == targetData.MaxLevel)
            {
                break;
            }
        }
    }
    void LoadBuyCostCSV()
    {
        List<Dictionary<string, string>> sheet = CSVReader.Read(costCSV);
        List<int> costList = new List<int>();
        foreach (Dictionary<string, string> row in sheet)
        {
            string name = row["Name"].ToString();
            int buyCost = int.Parse(row["Diamond"].ToString());
            costList.Add(buyCost);
        }
    }
}
