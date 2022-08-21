using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneLoaderController : MonoBehaviour
{
	public CanvasGroup myCanvasGroup;
	public Slider mySliderLoading;
	public bool showFinished{get;set;}
	public bool hideFinished{get;set;}

	void Start(){
		myCanvasGroup.alpha = 0f;
		gameObject.SetActive (false);
	}

	public void Show(){
		showFinished = false;
		hideFinished = false;
		myCanvasGroup.alpha = 0f;
		mySliderLoading.value = 0;
		gameObject.SetActive (true);
        /*LeanTween.value(gameObject, 0f, 1f, 0.2f).setOnUpdate((alpha) => {
            myCanvasGroup.alpha = alpha;
        }).setOnComplete(() => {
            showFinished = true;
        });*/
        myCanvasGroup.alpha = 1;
        showFinished = true;
    }

	public void Hide() {
        showFinished = false;
        hideFinished = false;
        /*LeanTween.value(gameObject, mySliderLoading.value, 1f, 0.2f).setOnUpdate((alpha) => {
            mySliderLoading.value = alpha;
        }).setOnComplete(() => {
            LeanTween.value(gameObject, 1f, 0f, 0.2f).setOnUpdate((alpha) => {
                myCanvasGroup.alpha = alpha;
            }).setOnComplete(() => {
                hideFinished = true;
                gameObject.SetActive(false);
            });
        });*/
        myCanvasGroup.alpha = 0;
        hideFinished = true;
        gameObject.SetActive(false);
    }
}
