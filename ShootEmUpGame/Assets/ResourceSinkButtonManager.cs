using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceSinkButtonManager : MonoBehaviour
{
    [SerializeField]
    protected string costId;
    [SerializeField]
    protected int costAmount;
    [SerializeField]
    protected ResourceTextManager costText;
    [SerializeField]
    private UnityEngine.UI.Image _imgBg;
    protected bool lastSufficentCheck = false;

    void Awake(){
        SetCost(costId, costAmount);
    }

    public void SetCost(string Id, int Amount){
        costId = Id;
        costAmount = Amount;
        costText.SetValue(Id, Amount.ToString());
        if (_imgBg != null)
        {
            if (DataManager.Instance.playerData.CheckPurchaseable(costId, costAmount))
            {
                _imgBg.material = GameInformation.Instance.materialData.matActionBtn;
            }
            else
            {
                _imgBg.material = GameInformation.Instance.materialData.matInvalidBtn;
            }
        }
    } 
}
