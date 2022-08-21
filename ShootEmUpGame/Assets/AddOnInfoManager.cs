using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnInfoManager : MonoBehaviour
{
    public void CloseAddOnInfo(){
        this.transform.gameObject.SetActive(false);
    }

    public void OpenAddOnInfo(){
        this.transform.gameObject.SetActive(true);
    }

    public void LoadAddOnAdata(){
        
    }
}
