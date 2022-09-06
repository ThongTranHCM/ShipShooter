using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DailyOfferContentManager : MonoBehaviour
{
    private static DailyOfferContentManager instance = null;
    public static DailyOfferContentManager Instance{
        get { return instance; }
    }
    [SerializeField]
    private GameObject offerListGameObject;
    [SerializeField]
    private TextMeshProUGUI countDownText;
    [SerializeField]
    private PurchaseResourceButtonManager purchaseButton;
    // Start is called before the first frame update
    void Awake(){
        if(instance == null){
            instance = this;
        }
    }
    void Start(){
        DailyOfferManager.Instance.InitContent();
    }
    void FixedUpdate(){
        DailyOfferManager.Instance.UpdateContent();
    }

    public void UpdateOfferPanel(List<DailyOfferManager.Offer> offerList){
        int i = 0;
        foreach(Transform child in gameObject.transform){
            (string, int) tuple = offerList[i].RewardTuple();
            child.gameObject.GetComponent<ResourcePanelManager>().SetReward(tuple.Item1, tuple.Item2);
            i += 1;
        }
    }

    public void SetCountDownText(string Text){
        countDownText.text = Text;
    }

    public void UpdatePurchaseButton(string ID, int Cost){
        purchaseButton.SetCost(ID, Cost);
    }
}
