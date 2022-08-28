using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityCanvasManager : MonoBehaviour
{
    [System.Serializable]
    public class OpenBoxCanvas{
        public GameObject canvas;
        public string id;
    }

    private static UtilityCanvasManager instance = null;
    public static UtilityCanvasManager Instance{
        get { return instance; }
    }
    [Header("Utility Canvas")]
    [SerializeField]
    private GameObject rewardResourceCanvas;
    [SerializeField]
    private GameObject rewardAddOnCanvas;
    [SerializeField]
    private List<OpenBoxCanvas> openBoxCanvasList;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    public void ShowRewardResource(string Type, int Amount){
        rewardResourceCanvas.SetActive(true);
        GameObject content = rewardResourceCanvas.transform.GetChild(0).gameObject;
        GameObject resourcePanel = rewardResourceCanvas.transform.Find("Content/ResourcePanel").gameObject;
        Debug.Log(Type);
        resourcePanel.GetComponent<ResourcePanelManager>().SetReward(Type, Amount);
        LeanTween.cancel(content);
        content.transform.localScale = new Vector3(1.0f, 0.75f);
        LeanTween.scale(content,new Vector3(1.0f,1.0f),0.75f).setEase(LeanTweenType.easeOutElastic);  
    }

    public void CloseRewardResrouce(){
        rewardResourceCanvas.SetActive(false);
    }

    public LTSeq ShowOpenBoxCanvas(string Id){
        GameObject canvas = openBoxCanvasList.Find((OpenBoxCanvas x) => {return x.id == Id;}).canvas;
        canvas.SetActive(true); 
        return canvas.GetComponent<BoxCanvasManager>().GetAnimationSeq();
    }

    public void CloseOpenBoxCanvas(string Id){
        GameObject canvas = openBoxCanvasList.Find((OpenBoxCanvas x) => {return x.id == Id;}).canvas;
        canvas.SetActive(false);
    }
}
