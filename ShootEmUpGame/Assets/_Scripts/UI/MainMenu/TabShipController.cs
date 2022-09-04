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

    private void PlayShipTransitionAnimation()
    {
        float scale = _defaultScaleX;
        LeanTween.cancel(displayShipGameObject);
        //LTSeq seq = LeanTween.sequence();
        //seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        UpdateShipInfo(shipIndex);
        UpdateShipStats(shipIndex);
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
        _intShipCost = 50 + _intShipLevel;
        _intShipPower = 2000 + _intShipLevel * 50;
        //Level
        _txtShipLevel.text = "Level " + _intShipLevel;
        _txtShipPower.text = "Power " + _intShipPower;
        //Button
        List<PurchaseResourceButtonManager.Reward> rewards = new List<PurchaseResourceButtonManager.Reward>();
        bool showPurchaseButton = (_intShipLevel > 0);
        bool showUpgradeButton = (_intShipLevel <= 0);
        _purchaseButton.gameObject.SetActive(showPurchaseButton);
        _upgradeButton.gameObject.SetActive(showUpgradeButton);
        if (showPurchaseButton)
        {
            _purchaseButton.SetCost("diamond", _intShipCost);
            rewards.Add(new PurchaseResourceButtonManager.Reward("diamond", 3));
            _purchaseButton.SetReward(rewards);
        }
        if (showUpgradeButton)
        {
            _upgradeButton.SetCost("gold", _intShipCost);
            rewards.Add(new PurchaseResourceButtonManager.Reward("gold", 3));
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
        _layoutShip.callbackUIClick = OnShipLayoutClick;
        UpdateShipInfo(shipIndex);
        UpdateShipStats(shipIndex);
    }

    public void OnShipLayoutClick(int index)
    {
        shipIndex = index;
        PlayShipTransitionAnimation();
    }

    public void OnShipUpgrade()
    {
        Debug.LogError("Upgrade");
        UpdateShipStats(shipIndex);
    }

    public void OnShipUpgradeUIClick()
    {
        DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel += 1;
        RewardResourceManager.Instance.Purchase("gold", _intShipCost, new List<(string, int)>());
        OnShipUpgrade();
    }
}
