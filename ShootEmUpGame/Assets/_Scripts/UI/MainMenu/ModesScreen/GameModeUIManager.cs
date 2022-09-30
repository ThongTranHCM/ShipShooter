using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtLevel;
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtPrevLevel;
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtNextLevel;
    [SerializeField]
    private GameObject _objLevel;
    [SerializeField]
    private GameObject _objPrevLevel;
    [SerializeField]
    private GameObject _objNextLevel;
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
        shipIndex = DataManager.Instance.GetLastShipIndex(_mode);
        shipCount = GameInformation.Instance.shipData.Count;
        InstallModel(shipIndex);
        InstallLevelInfo();
    }
    private void PlayShipTransitionAnimation()
    {
        float scale = _defaultScaleX;
        LeanTween.cancel(displayShipGameObject);
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        InstallModel(shipIndex);
        InstallLevelInfo();
        seq.append(LeanTween.scaleX(displayShipGameObject, scale, expandDuration).setEase(LeanTweenType.easeOutElastic));
    }

    private void InstallModel(int index)
    {
        DOShipData shipData = GameInformation.Instance.shipData[index];
        displayMesh.mesh = shipData.meshShip;
        displayRenderer.material = shipData.materialShip;
        DataManager.Instance.selectedShipIndex = index;
    }

    private void InstallLevelInfo()
    {
        switch (_mode)
        {
            case Constants.MODE_Endless:
                {
                    _txtLevel.text = "Iron";
                    break;
                }
            case Constants.MODE_Challenge:
                {
                    int level = DataManager.Instance.GetLastChallengeIndex(shipIndex);
                    _txtLevel.text = "Challenge " + shipIndex + "_" + level;
                    if (level == 0)
                    {
                        _objPrevLevel.SetActive(false);
                    }
                    else
                    {
                        _objPrevLevel.SetActive(true);
                        _txtPrevLevel.text = "Challenge " + shipIndex + "_" + (level - 1);
                    }
                    if (level > 120)
                    {
                        _objNextLevel.SetActive(false);
                    }
                    else
                    {
                        _objNextLevel.SetActive(true);
                        _txtNextLevel.text = "Challenge " + shipIndex + "_" + (level + 1);
                    }
                    break;
                }
            case Constants.MODE_Story:
            default:
                {
                    _txtLevel.text = "Level " + (DataManager.Instance.LastLevelWin + 1);
                    break;
                }
        }
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