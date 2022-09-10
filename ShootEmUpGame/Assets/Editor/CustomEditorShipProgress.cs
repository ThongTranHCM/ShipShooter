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
        List<int> powerList = new List<int>(new int[targetData.MaxLevel]);
        List<int> costList = new List<int>(new int[targetData.MaxLevel]);
        foreach (Dictionary<string, string> row in sheet)
        {
            int level = int.Parse(row["Level"].ToString());
            int power = int.Parse(row["Power"].ToString());
            int upgradeCost = int.Parse(row["Gold"].ToString());
            powerList[level - 1] = power;
            costList[level - 1] = upgradeCost;
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
