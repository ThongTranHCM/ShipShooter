using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeAddOnCanvasManager : MonoBehaviour
{
    private static UpgradeAddOnCanvasManager instance;
    public static UpgradeAddOnCanvasManager Instance
    {
        get { return instance; }
    }
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI addOnName;
    [SerializeField]
    private AddOnEquipData addOnEquipData;
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
    public UpgradeAddOnCanvasManager()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetContentShowUpdateAddOn(Sprite sprite, int levelPrev, float powerPrev, int levelPost, float powerPost)
    {
        panelLevel.SetActive(true);
        panelPower.SetActive(true);
        icon.sprite = sprite;
        txtLevelPrev.text = levelPrev.ToString();
        txtLevelPost.text = levelPost.ToString();
        txtPowerPrev.text = powerPrev.ToString();
        txtPowerPost.text = powerPost.ToString();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //resourcePanel.GetComponent<ResourcePanelManager>().SetReward(Type, Amount);
        //LeanTween.cancel(content);
        //content.transform.localScale = new Vector3(1.0f, 0.75f);
        //LeanTween.scale(content, new Vector3(1.0f, 1.0f), 0.75f).setEase(LeanTweenType.easeOutElastic);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
