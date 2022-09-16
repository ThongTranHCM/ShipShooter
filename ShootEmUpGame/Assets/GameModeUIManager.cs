using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameModeUIManager : MonoBehaviour
{
    const float shrinkDuration = 0.1f;
    const float expandDuration = 0.5f;
    [SerializeField]
    private GameObject displayEnemyCanvas;
    [SerializeField]
    private GameObject displayShipGameObject;
    [SerializeField]
    private MeshFilter displayMesh;
    [SerializeField]
    private MeshRenderer displayRenderer;
    private int shipIndex;
    private int shipCount;
    [SerializeField]
    private float _defaultScaleX;
    [SerializeField]
    private string _mode;
    private LevelDesignData levelDesignData;
    #region ButtonBehavior

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
    #endregion

    public void Start()
    {
        Install();
    }
    public void Install()
    {
        _mode = Constants.MODE_Story;
        shipIndex = DataManager.Instance.GetLastShipIndex(_mode);
        shipCount = GameInformation.Instance.shipData.Count;
        InstallModel(shipIndex);
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

    protected void DisplayEnemies()
    {
        UpdateLevelDesignData();
        List<EnemyData> enemyList = levelDesignData.GetEnemyDataList();
    }

    public virtual void ResetUI()
    {
        return;
    }

    protected virtual void UpdateLevelDesignData()
    {
        return;
    }
}