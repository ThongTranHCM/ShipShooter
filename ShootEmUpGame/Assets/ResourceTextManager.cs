using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceTextManager : MonoBehaviour
{
    [SerializeField]
    private GameObject iconPlaceHolderGameObject;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private ResourceData resourceData;

    public void SetText(string Text){
        text.text = Text;
    }

    public void SetResource(string Id){
        ResourceData.Type type = resourceData.GetType(Id);
        GameObject icon = type.SmallIconGameObject;
        foreach (Transform child in iconPlaceHolderGameObject.transform) {
            GameObject.Destroy(child.gameObject);
        }
        Object.Instantiate(icon, iconPlaceHolderGameObject.transform.position, Quaternion.identity, iconPlaceHolderGameObject.transform);
    }
}
