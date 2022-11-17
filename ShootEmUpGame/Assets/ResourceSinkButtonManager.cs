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
    [SerializeField]
    private GameObject _validButton;
    [SerializeField]
    private GameObject _invalidButton;
    protected bool lastSufficentCheck = false;

    void Awake(){
        SetCost(costId, costAmount);
    }

    public void SetCost(string Id, int Amount){
        costId = Id;
        costAmount = Amount;
        costText.SetValue(Id, Amount.ToString());
        Refresh();
    }

    public void Refresh()
    {
        if (_imgBg != null)
        {
            if (DataManager.Instance.playerData.CheckPurchaseable(costId, costAmount))
            {
                _validButton.SetActive(true);
                _invalidButton.SetActive(false);;
            }
            else
            {
                _validButton.SetActive(false);
                _invalidButton.SetActive(true);
            }
        }
    }
}
