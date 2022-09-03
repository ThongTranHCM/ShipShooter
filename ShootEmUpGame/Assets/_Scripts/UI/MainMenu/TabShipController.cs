using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabShipController : MonoBehaviour
{
    public GameObject shipPanelPrefab;
    [SerializeField]
    private GameObject displayShipGameObject;
    [SerializeField]
    private MeshFilter displayMesh;
    [SerializeField]
    private MeshRenderer displayRenderer;

    [SerializeField]
    private TextMeshProUGUI _txtShipName;
    [SerializeField]
    private TextMeshProUGUI _txtShipLevel;
    [SerializeField]
    private TextMeshProUGUI _txtShipPower;
    [SerializeField]
    private TextMeshProUGUI _txtShipCost;

    [SerializeField]
    private float _defaultScaleX;
    private int shipIndex;
    private int shipCount;
    const float shrinkDuration = 0.1f;
    const float expandDuration = 0.5f;
    private void PlayShipTransitionAnimation()
    {
        float scale = _defaultScaleX;
        LeanTween.cancel(displayShipGameObject);
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        UpdateShipInfo(shipIndex);
        seq.append(LeanTween.scaleX(displayShipGameObject, scale, expandDuration).setEase(LeanTweenType.easeOutElastic));
    }

    private void UpdateShipInfo(int index)
    {
        //Name
        _txtShipName.text = "Name";
        //Level
        _txtShipLevel.text = "Level 12";
        _txtShipPower.text = "Power 2500";
        _txtShipCost.text = "50";
        //Models
        DOShipData shipData = GameInformation.Instance.shipData[index];
        displayMesh.mesh = shipData.meshShip;
        displayRenderer.material = shipData.materialShip;
        DataManager.Instance.selectedShipIndex = index;
    }

    public void OnEnable()
    {
        PlayShipTransitionAnimation();
    }
}
