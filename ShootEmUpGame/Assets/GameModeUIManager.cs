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
    private LevelDesignData levelDesignData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        public void OnNextShipButton(){
        PlayShipTransitionAnimation(null);
        return;
    }

    public void OnPreviousShipButton(){
        PlayShipTransitionAnimation(null);
        return;
    }

    private void PlayShipTransitionAnimation(GameObject ToObject){
        float scale = displayShipGameObject.transform.localScale.x;
        LeanTween.cancel(displayShipGameObject);
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.scaleX(displayShipGameObject, 0, shrinkDuration).setEase(LeanTweenType.easeInBack));
        //SwapModel
        seq.append(LeanTween.scaleX(displayShipGameObject, scale, expandDuration).setEase(LeanTweenType.easeOutElastic));
    }

    protected void DisplayEnemies(){
        UpdateLevelDesignData();
        List<EnemyData> enemyList = levelDesignData.GetEnemyDataList();
    }

    public virtual void ResetUI(){
        return;
    }

    protected virtual void UpdateLevelDesignData(){
        return;
    }
}
