using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipResourceCanvasManager : MonoBehaviour
{
    private static ShipResourceCanvasManager instance;
    public static ShipResourceCanvasManager Instance
    {
        get { return instance; }
    }
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private GameObject panelDescription;
    [SerializeField]
    private TextMeshProUGUI txtShipName;
    [SerializeField]
    private GameObject panelLevel;
    [SerializeField]
    private TextMeshProUGUI txtLevelPrev;
    [SerializeField]
    private TextMeshProUGUI txtLevelPost;
    [SerializeField]
    private GameObject panelPower;
    [SerializeField]
    private TextMeshProUGUI txtPowerPrev;
    [SerializeField]
    private TextMeshProUGUI txtPowerPost;

    // Start is called before the first frame update
    public ShipResourceCanvasManager()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Show(string Type, int Amount)
    {
        gameObject.SetActive(true);
        Debug.LogError("Show " + Type);
        //resourcePanel.GetComponent<ResourcePanelManager>().SetReward(Type, Amount);
        //LeanTween.cancel(content);
        //content.transform.localScale = new Vector3(1.0f, 0.75f);
        //LeanTween.scale(content, new Vector3(1.0f, 1.0f), 0.75f).setEase(LeanTweenType.easeOutElastic);
    }

    public void SetContentUnlockShip(string name)
    {
        panelDescription.SetActive(true);
        panelLevel.SetActive(false);
        panelPower.SetActive(false);
        txtShipName.text = name;
        // DataManager.Instance.playerData.GetShipProgress(shipIndex).shipLevel += 1;
    }

    public void SetContentShowUpdateShip(int levelPrev, int powerPrev, int levelPost, int powerPost)
    {
        panelDescription.SetActive(false);
        panelLevel.SetActive(true);
        panelPower.SetActive(true);
        txtLevelPrev.text = levelPrev.ToString();
        txtLevelPost.text = levelPost.ToString();
        txtPowerPrev.text = powerPrev.ToString();
        txtPowerPost.text = powerPost.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
