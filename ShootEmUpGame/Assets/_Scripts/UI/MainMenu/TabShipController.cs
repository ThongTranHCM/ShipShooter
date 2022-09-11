using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabShipController : MonoBehaviour
{
    [Header("Ship Info")]
    [SerializeField]
    private GameObject displayShipGameObject;
    [SerializeField]
    private MeshFilter displayMesh;
    [SerializeField]
    private MeshRenderer displayRenderer;

    [SerializeField]
    private TextMeshProUGUI _txtShipName;

    [Header("Ship Stats")]
    [SerializeField]
    private TextMeshProUGUI _txtShipLevel;
    [SerializeField]
    private TextMeshProUGUI _txtShipPower;
    [SerializeField]
    private PurchaseResourceButtonManager _purchaseButton;
    [SerializeField]
    private PurchaseResourceButtonManager _upgradeButton;

    [Header("Ship Group")]
    [SerializeField]
    private ShipGroupLayout _layoutShip;

    [SerializeField]
    private float _defaultScaleX;
    private int shipIndex;
    private int shipCount;
    const float shrinkDuration = 0.1f;
    const float expandDuration = 0.5f;
    private string _strName;
    private int _intShipLevel;
    private int _intShipPower;
    private int _intShipCost;
    private string _strCostCurrency;

    private void PlayShipTransitionAnimation()
    {
        float scale = _defaultScaleX;
        LeanTween.cancel(displayShipGameObject);
        //LTSeq seq = LeanTween.sequence();
        //seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        UpdateShipInfo(shipIndex);
        UpdateShipStats(shipIndex);
        _layoutShip.SelectShip(shipIndex);
        //seq.append(LeanTween.scaleX(displayShipGameObject, scale, expandDuration).setEase(LeanTweenType.easeOutElastic));
    }

    private void UpdateShipInfo(int index)
    {
        DOShipData shipData = GameInformation.Instance.shipData[index];
        _strName = shipData.shipName;
        //Name
        _txtShipName.text = _strName;
        //Models
        displayMesh.mesh = shipData.meshShip;
        displayRenderer.material = shipData.materialShip;
        DataManager.Instance.selectedShipIndex = index;
    }

    private void UpdateShipStats(int index)
    {
        _intShipLevel = DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel;
        Debug.LogError("Index " + index);
        int minLevel = Mathf.Max(1, _intShipLevel);
        DOShipData shipData = GameInformation.Instance.GetShipData(shipIndex);
        _intShipCost = (int)shipData.GetUpgradeCostFrom(_intShipLevel);
        _intShipPower = (int)shipData.GetPower(minLevel - 1);
        _strCostCurrency = "gold";
        //Level
        _txtShipLevel.text = "Level " + minLevel;
        _txtShipPower.text = "Power " + _intShipPower;
        //Button
        List<PurchaseResourceButtonManager.Reward> rewards = new List<PurchaseResourceButtonManager.Reward>();
        bool showPurchaseButton = (_intShipLevel <= 0);
        bool showUpgradeButton = (_intShipLevel > 0);
        _purchaseButton.gameObject.SetActive(showPurchaseButton);
        _upgradeButton.gameObject.SetActive(showUpgradeButton);
        if (showPurchaseButton)
        {
            _intShipCost = (int)shipData.ShipCost;
            _strCostCurrency = shipData.ShipCostCurrency;
            _purchaseButton.SetCost(_strCostCurrency, _intShipCost);
            rewards.Add(new PurchaseResourceButtonManager.Reward("ship" + (index + 1) + "Buy", 1));
            _purchaseButton.SetReward(rewards);
        }
        if (showUpgradeButton)
        {
            _upgradeButton.SetCost(_strCostCurrency, _intShipCost);
            rewards.Add(new PurchaseResourceButtonManager.Reward("ship" + (index + 1) + "Upgrade", 1));
            _upgradeButton.SetReward(rewards);
        }
    }

    public void Start()
    {
        Install();
    }

    public void Install()
    {
        shipIndex = 0;
        shipCount = GameInformation.Instance.shipData.Count;
        _layoutShip.Install();
        _layoutShip.callbackUIClick = OnShipLayoutClick;
        UpdateShipInfo(shipIndex);
        UpdateShipStats(shipIndex);
    }

    #region Callback Click
    public void OnShipLayoutClick(int index)
    {
        shipIndex = index;
        PlayShipTransitionAnimation();
    }

    private void OnShipUpgrade()
    {
        UpdateShipStats(shipIndex);
        _layoutShip.Install();
    }

    public void OnShipUpgradeUIClick()
    {
        DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel += 1;
        OnShipUpgrade();
    }

    private void OnShipBuy()
    {
        UpdateShipStats(shipIndex);
        _layoutShip.UpdateInfo();
        _layoutShip.SelectShip(shipIndex);
    }

    public void OnShipBuyUIClick()
    {
        DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel = Mathf.Max(DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel, 1);
        OnShipBuy();
    }
    #endregion
}