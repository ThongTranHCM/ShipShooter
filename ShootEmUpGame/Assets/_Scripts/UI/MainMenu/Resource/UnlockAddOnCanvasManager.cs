using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockAddOnCanvasManager : MonoBehaviour
{
    private static UnlockAddOnCanvasManager instance;
    public static UnlockAddOnCanvasManager Instance{
        get {return instance;}
    }
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI name;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private AddOnEquipData addOnEquipData;

    public UnlockAddOnCanvasManager(){
        if(instance == null){
            instance = this;
        }
    }

    private void AnimationSeq(){
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.75f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);  
        return;
    }

    public void Show(string Id){
        SetAddOn(Id);
        gameObject.SetActive(true);
        AnimationSeq();
        return;
    }

    public void Close(){
        gameObject.SetActive(false);
    }

    private void SetAddOn(string Id){
        IAddOnData addOnData = addOnEquipData.GetAddOnData(Id);
        icon.sprite = addOnData.GetSprite;
        name.text = Id;
    }
}
