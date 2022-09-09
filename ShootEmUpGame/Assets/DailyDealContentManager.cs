using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyDealContentManager : MonoBehaviour
{
    private static DailyDealContentManager instance = null;
    public static DailyDealContentManager Instance{
        get { return instance; }
    }
    [SerializeField]
    private TextMeshProUGUI countDownText;
    [SerializeField]
    private TabberController tabberController;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    void Start(){
        DailyDealManager.Instance.InitContent();
    }

    void FixedUpdate(){
        DailyDealManager.Instance.UpdateCounter();
    }
    public void UpdateDealPanel(List<DailyDealManager.Deal> dealList){
        int i = 0;
        foreach(Transform child in gameObject.transform){
            child.GetComponent<DailyDealPanelManager>().SetDeal(dealList[i]);
            i += 1;
        }
    }
    public void SetTimeCounter(string Counter){
        countDownText.text = Counter;
    }
    public bool IsInTab(){
        return tabberController.currentTab == 4;
    }
}
