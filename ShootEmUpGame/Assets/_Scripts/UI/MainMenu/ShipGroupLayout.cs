using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGroupLayout : MonoBehaviour
{
    public GameObject shipPanelPrefab;
    [SerializeField]
    private GameObject displayShipGameObject;
    [SerializeField]
    private MeshFilter displayMesh;
    [SerializeField]
    private MeshRenderer displayRenderer;


    [SerializeField]
    private float _defaultScaleX;
    private int shipIndex;
    private int shipCount;
    const float shrinkDuration = 0.1f;
    const float expandDuration = 0.5f;


    public void OnNextShipButton()
    {
        shipIndex = (shipIndex + 1) % shipCount;
        PlayShipTransitionAnimation();
        return;
    }

    public void OnPreviousShipButton()
    {
        shipIndex = (shipIndex + shipCount - 1) % shipCount;
        PlayShipTransitionAnimation();
        return;
    }

    private void PlayShipTransitionAnimation()
    {
        float scale = _defaultScaleX;
        LeanTween.cancel(displayShipGameObject);
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        InstallModel(shipIndex);
        seq.append(LeanTween.scaleX(displayShipGameObject, scale, expandDuration).setEase(LeanTweenType.easeOutElastic));
    }

    private void InstallModel(int index)
    {
        DOShipData shipData = GameInformation.Instance.shipData[index];
        displayMesh.mesh = shipData.meshShip;
        displayRenderer.material = shipData.materialShip;
        DataManager.Instance.selectedShipIndex = index;
    }
}