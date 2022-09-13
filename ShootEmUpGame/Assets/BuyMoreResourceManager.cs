using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyMoreResourceManager : MonoBehaviour
{
    private static BuyMoreResourceManager instance;
    public static BuyMoreResourceManager Instance{
        get {return instance;}
    }

    BuyMoreResourceManager(){
        if(instance == null){
            instance = this;
        }
    }

    public void OpenPopUp(string Id){
        gameObject.SetActive(true);
        foreach(Transform transform in gameObject.GetComponentsInChildren<Transform>(true)){
            if( transform.gameObject.name == Id){
                transform.gameObject.SetActive(true);
            }
        }
    }

    public void ClosePopUp(string Id){
        foreach(Transform transform in gameObject.GetComponentsInChildren<Transform>(true)){
            if( transform.gameObject.name == Id){
                transform.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }
}
