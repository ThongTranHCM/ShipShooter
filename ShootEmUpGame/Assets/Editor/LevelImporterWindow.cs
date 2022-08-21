using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelImporterWindow : EditorWindow
{
    private Dictionary<string,EnemyData> enemyDataDict = new Dictionary<string, EnemyData>();
    private AddOnEquipData addOnEquipData;
    [MenuItem ("Window/Level Importer")]
    public static void ShowWindow () {
        EditorWindow.GetWindow(typeof(LevelImporterWindow));
    }
    
    string Absolute2RelativeDirectory(string path){
        return "Assets" + path.Substring(Application.dataPath.Length);
    }

    void OnGUI () {
        addOnEquipData = (AddOnEquipData)EditorGUILayout.ObjectField("Add On Equip Data ",addOnEquipData, typeof(AddOnEquipData));
        if (GUILayout.Button("Select Enemy Data Directory")){
            string texDirectory = EditorUtility.OpenFolderPanel("Select Enemy Data Directory", "", "");
            LoadEnemyData(texDirectory);
        }
        if (enemyDataDict.Count > 0){
            GUILayout.Label(string.Format("Loaded {0} enemy data.", enemyDataDict.Count));
            if (GUILayout.Button("Load Level Design CSV")){
                LoadCSV("LevelDesign");
            }
        }
    }

    void LoadEnemyData(string EnemyDataDirectory){
        string[] files = new string[0];
        enemyDataDict = new Dictionary<string, EnemyData>();
        EnemyData tmp;
        string key;
        files = Directory.GetFiles(EnemyDataDirectory, "*.asset", SearchOption.TopDirectoryOnly);
        foreach(string file in files){
            tmp = (EnemyData)AssetDatabase.LoadAssetAtPath(Absolute2RelativeDirectory(file),typeof(EnemyData));
            key = file.Substring(EnemyDataDirectory.Length + 1).Replace(".asset","");
            enemyDataDict.Add(key,tmp);
        }
    }

    void LoadCSV(string CSVDirectory){
        List<Dictionary<string, string>> sheet = CSVReader.Read(CSVDirectory);
        foreach(Dictionary<string, string> row in sheet){
            string level = row["Level"].ToString();
            string[] enemyDataString = row["EnemyData"].Split(',');
            string[] addOnDropPoolListString = row["AddOnDropPool"].Split(',');
            int numWave = int.Parse(row["NumWave"].ToString());
            float baseHP = float.Parse(row["BaseHP"].ToString());
            float startHP = float.Parse(row["StartHP"].ToString());
            float endHP = float.Parse(row["EndHP"].ToString());
            float startDense = float.Parse(row["StartDense"].ToString());
            float endDense = float.Parse(row["EndDense"].ToString());
            Debug.Log(endDense);

            List<EnemyData> enemyDataList = new List<EnemyData>();
            foreach(string key in enemyDataString){
                try {
                    enemyDataList.Add(enemyDataDict[key]);
                }
                catch (Exception e) {
                    Debug.Log(string.Format("{0} is not found", key));
                }
            }
            List<RandomLevelDesignData.AddOnDropPool> addOnDropPoolList = new List<RandomLevelDesignData.AddOnDropPool>();
            foreach(string poolKeys in addOnDropPoolListString){
                try {
                    string[] keys = poolKeys.Split('|');
                    List<AddOnEquipData.AddOnType> addOnDropPool = new List<AddOnEquipData.AddOnType>();
                    foreach(string key in keys){
                        addOnDropPool.Add(addOnEquipData.addOnDatas[int.Parse(key)].GetAddOnType);
                    }
                    addOnDropPoolList.Add(new RandomLevelDesignData.AddOnDropPool(addOnDropPool));
                }
                catch (Exception e) {
                }
            }
            RandomLevelDesignData asset = RandomLevelDesignData.CreateInstance(enemyDataList,numWave,baseHP,startHP,endHP,startDense,endDense,addOnDropPoolList,1);
            AssetDatabase.CreateAsset(asset, string.Format("Assets/Resource/RandomLevelDesignData/{0}.asset",row["Level"].ToString()));
            AssetDatabase.SaveAssets();

        }
    }
}
