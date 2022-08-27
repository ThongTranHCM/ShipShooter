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
    private GameObject smallIcon;
    [SerializeField]
    private TextMeshProUGUI amountTxt;
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
        RemoveIcon();
        Instantiate(type.BigIconGameObject, bigIcon.transform.position, Quaternion.identity, bigIcon.transform);
        Instantiate(type.SmallIconGameObject, smallIcon.transform.position, Quaternion.identity, smallIcon.transform);
        if(Amount <= 1){
            nameTxt.text = type.name;
            nameTxt.gameObject.SetActive(true);
            amountTxt.gameObject.SetActive(false);
        } else {
            amountTxt.text = Amount.ToString();
            nameTxt.gameObject.SetActive(false);
            amountTxt.gameObject.SetActive(true);
        }
    }

    private void RemoveIcon(){
        foreach(Transform child in bigIcon.transform){
            Object.Destroy(child.gameObject);
        }
        foreach(Transform child in smallIcon.transform){
            Object.Destroy(child.gameObject);
        }
    }
}
