using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceData", order = 1)]
public class ResourceData : ScriptableObject
{
    [System.Serializable]
    public class Type{
        public string id;
        public string name;
        public GameObject BigIconGameObject;
        public GameObject SmallIconGameObject;
    }

    public List<Type> typeList;
    public Type GetType(string id){
        foreach (Type x in typeList){
            if(x.id == id){
                return x;
            }
        }
        return null;
    }

    public int GetResourceAmount(string ID){
        return 0;
    }

    public void AddResourceAmount(string ID, int Amount){
        return;
    }

    public bool CheckResourceAmount(string ID, int Amount){
        return true;
    }

    public void ReduceResourceAmount(string ID, int Amount){
        return;
    }
}