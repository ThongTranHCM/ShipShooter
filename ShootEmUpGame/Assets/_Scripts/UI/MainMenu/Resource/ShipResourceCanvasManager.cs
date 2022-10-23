using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject resourcePanel;

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
