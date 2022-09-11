using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcePanelManager : MonoBehaviour
{
    [SerializeField]
    private string resourceID = "gold";
    [SerializeField]
    private int resourceAmount = 0;

    [SerializeField]
    private ResourceData resourceData;
    [SerializeField]
    private GameObject bigIcon;
    [SerializeField]
    private ResourceTextManager resourceText;
    [SerializeField]
    private TextMeshProUGUI nameTxt;

    public void Start(){
        SetReward(resourceID, resourceAmount);
    }

    public void SetReward(string ID, int Amount){
        resourceID = ID;
        resourceAmount = Amount;
        UpdateUI(resourceID, resourceAmount);
    }

    private void UpdateUI(string ID, int Amount){
        ResourceData.Type type = resourceData.GetType(ID);
        SetBigIcon(ID);
        resourceText.SetResource(ID);
        if(Amount <= 1){
            nameTxt.text = type.name;
            nameTxt.gameObject.SetActive(true);
            resourceText.gameObject.SetActive(false);
        } else {
            resourceText.SetText(Amount.ToString());
            nameTxt.gameObject.SetActive(false);
            resourceText.gameObject.SetActive(true);
        }
    }

    private void SetBigIcon(string ID){
        ResourceData.Type type = resourceData.GetType(ID);
        foreach(Transform child in bigIcon.transform){
            Object.Destroy(child.gameObject);
        }
        Debug.LogError("ID" +ID);
        Instantiate(type.BigIconGameObject, bigIcon.transform.position, Quaternion.identity, bigIcon.transform);
    }
}
