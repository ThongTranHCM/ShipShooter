using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceTextManager : MonoBehaviour
{
    [SerializeField]
    private GameObject iconPlaceHolderGameObject;
    [SerializeField]
    private ResourceData resourceData;
    [SerializeField]
    private string text;
    [SerializeField]
    private string id;

    void Awake(){
        SetValue(text, id);
    }

    public void SetValue(string Id, string Text){
        SetText(Text);
        SetResource(Id);
    }

    public void SetText(string Text){
        text = Text;
        GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SetResource(string Id){
        if(Id != ""){
            ResourceData.Type type = resourceData.GetType(Id);
            GameObject icon = type.SmallIconGameObject;
            foreach (Transform child in iconPlaceHolderGameObject.transform) {
                GameObject.Destroy(child.gameObject);
            }
            Object.Instantiate(icon, iconPlaceHolderGameObject.transform.position, Quaternion.identity, iconPlaceHolderGameObject.transform);
        }
    }
}
