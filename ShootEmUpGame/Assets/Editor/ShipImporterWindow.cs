using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShipImporterWindow : EditorWindow
{
    private DOShipProgressData shipProgressData;
    private int levelSize;
    [MenuItem("Window/Ship Importer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ShipImporterWindow));
    }

    string Absolute2RelativeDirectory(string path)
    {
        return "Assets" + path.Substring(Application.dataPath.Length);
    }

    void OnGUI()
    {
        shipProgressData = (DOShipProgressData)EditorGUILayout.ObjectField("Ship Progress Data ", shipProgressData, typeof(DOShipProgressData));
        levelSize = EditorGUILayout.IntField("Ship Number of Levels ", levelSize);
        if (GUILayout.Button("Load Ship Progress"))
        {
            LoadCSV("LevelDesign - ShipLevelUp");
            SaveChanges();
        }
    }

    void LoadCSV(string CSVDirectory)
    {
        List<Dictionary<string, string>> sheet = CSVReader.Read(CSVDirectory);
        foreach (Dictionary<string, string> row in sheet)
        {
            string level = row["Level"].ToString();
            int power = int.Parse(row["Power"].ToString());
            int gold = int.Parse(row["Gold"].ToString());
            Debug.LogError("Level " + level + " Power " + power + " Upgrade Cost " + gold);

            //AssetDatabase.CreateAsset(asset, string.Format("Assets/Resource/RandomLevelDesignData/{0}.asset", row["Level"].ToString()));
            //AssetDatabase.SaveAssets();

        }
    }
}
