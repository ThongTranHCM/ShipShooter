using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataManagerLoader : MonoBehaviour
{
    public void DoResetData()
    {
        DataManager.ResetToDefault();
        DataManager.Save();
    }

    public void DoResetAddOn()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        for (int i = 0; i < addOnInfos.Count; i++)
        {
            addOnInfos[i].CurrentFragment = 0;
            addOnInfos[i].CurrentLevel = 0;
        }
    }

    public void DoIncreaseAddOnLevelsBy10()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        for (int i = 0; i < addOnInfos.Count; i++)
        {
            addOnInfos[i].CurrentFragment = Mathf.Min(addOnInfos[i].CurrentFragment + 10, 100);
            addOnInfos[i].CurrentLevel = Mathf.Min(addOnInfos[i].CurrentLevel + 10, 100);
        }
    }
    public void DoIncrease3AddOnsRandomBy5Level()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            a = Random.Range(0, addOnInfos.Count);
            addOnInfos[a].CurrentLevel = Mathf.Min(addOnInfos[a].CurrentLevel + 5, 100);
        }
    }
    public void DoIncrease3AddOnsRandomBy5Fragment()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            a = Random.Range(0, addOnInfos.Count);
            addOnInfos[a].CurrentFragment = Mathf.Min(addOnInfos[a].CurrentFragment + 5, 100);
        }
    }


#if UNITY_EDITOR
    [MenuItem("Data/Clear")]
    private static void ResetData()
    {
        Debug.LogError("Reset");
        DataManager.ResetToDefault();
        DataManager.Save();
    }
    [MenuItem("Data/Cheat/+ 1000 Gold")]
    private static void CheatGold()
    {
        DataManager.Instance.playerData.Coin += 1000;
    }
    [MenuItem("Data/Cheat/+ 100 Diamond")]
    private static void CheatDiamond()
    {
        DataManager.Instance.playerData.Diamond += 100;
    }
    [MenuItem("Data/Cheat/Add On/Reset")]
    private static void ResetAddOn()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        for (int i = 0; i < addOnInfos.Count; i++)
        {
            addOnInfos[i].CurrentFragment = 0;
            addOnInfos[i].CurrentLevel = 0;
        }
    }
    [MenuItem("Data/Cheat/Add On/Increase Level 10")]
    private static void IncreaseAddOnLevelsBy10()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        for (int i = 0; i < addOnInfos.Count; i++)
        {
            addOnInfos[i].CurrentFragment = Mathf.Min(addOnInfos[i].CurrentFragment + 10, 100);
            addOnInfos[i].CurrentLevel = Mathf.Min(addOnInfos[i].CurrentLevel + 10, 100);
        }
    }
    [MenuItem("Data/Cheat/Add On/Increase 3 Random By 5 Levels")]
    private static void Increase3AddOnsRandomBy5Level()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            a = Random.Range(0, addOnInfos.Count);
            addOnInfos[a].CurrentLevel = Mathf.Min(addOnInfos[a].CurrentLevel + 5, 100);
        }
    }
    [MenuItem("Data/Cheat/Add On/Increase 3 Random By 5 Fragments")]
    private static void Increase3AddOnsRandomBy5Fragment()
    {
        List<AddOnUserData.AddOnInfo> addOnInfos = DataManager.Instance.addOnUserData.GetListAddOnInfo();
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            a = Random.Range(0, addOnInfos.Count);
            addOnInfos[a].CurrentFragment = Mathf.Min(addOnInfos[a].CurrentFragment + 5, 100);
        }
    }
#endif
}
