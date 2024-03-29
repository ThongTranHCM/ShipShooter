using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentRewardCanvasManager : MonoBehaviour
{
    private static FragmentRewardCanvasManager instance;
    public static FragmentRewardCanvasManager Instance{
        get {return instance;}
    }
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private AddOnUIItem addOnItem;
    [SerializeField]
    private AddOnEquipData addOnEquipData;

    FragmentRewardCanvasManager(){
        if(instance == null){
            instance = this;
        }
    }

    public void Show(string Id, int Amount){
        gameObject.SetActive(true);
        AddFragment(Id, Amount);
        AnimationSeq();
    }

    private void AnimationSeq(){
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.75f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);  
        return;
    }

    private void AddFragment(string Id, int Amount){
        IAddOnData addOnData = addOnEquipData.GetAddOnData(Id);
        int oldFragment = addOnData.GetFragment;
        int newFragment = oldFragment + Amount;
        int cost;

        if (addOnData.GetLevel < 1)
        {
            cost = GameInformation.Instance.addOnEquipData.GetUnlockCost();
        }
        else
        {
            cost = GameInformation.Instance.addOnEquipData.GetUpgradeCost(addOnData.GetLevel);
        }
        addOnItem.Install(addOnData.GetSprite, (int)addOnData.GetLevel, addOnData.GetFragment, cost);
        addOnItem.UpdateFragment(newFragment, cost, 1.0f, 0.1f);
    }

    public void Close(){
        gameObject.SetActive(false);
    }
}
