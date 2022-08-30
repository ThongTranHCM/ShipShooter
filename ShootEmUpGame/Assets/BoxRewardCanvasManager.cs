using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRewardCanvasManager : MonoBehaviour
{
    private static BoxRewardCanvasManager instance;
    public static BoxRewardCanvasManager Instance{
        get { return instance;} 
    }
    [System.Serializable]
    private class BoxType{
        public string id;
        public GameObject gameObject;
    }
    [SerializeField]
    List<BoxType> boxList;
    [SerializeField]
    GameObject box;
    [SerializeField]
    Transform start;
    [SerializeField]
    Transform stop;
    void Awake(){
        instance = this;
        instance.Close();
    }

    private LTSeq GetAnimationSeq(){
        Vector3 stretch = Vector3.up * 2 + Vector3.right * 0.5f + Vector3.forward;
        Vector3 normal = Vector3.one;
        Vector3 squash = Vector3.up * 0.9f + Vector3.right * 1.11f + Vector3.forward;
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.move(box,start,0f));
        seq.append(LeanTween.scale(box, stretch, 0f));
        seq.append(() => {
            LeanTween.move(box,stop,1.0f).setEase(LeanTweenType.easeOutBounce);
            LeanTween.scale(box,normal,1.0f).setEase(LeanTweenType.easeOutBounce);
        });
        seq.append(1.0f);
        seq.append(() => {
            LeanTween.rotateZ(box,5f,0.075f).setEase(LeanTweenType.easeShake).setRepeat(8);
            LeanTween.scale(box,squash,0.6f).setEase(LeanTweenType.easeOutSine);
        });
        seq.append(0.6f);
        seq.append(LeanTween.scale(box,stretch,0.1f).setEase(LeanTweenType.easeInBack));
        seq.append(() => {gameObject.SetActive(false);});
        return seq;
    }

    public LTSeq Show(string BoxId){
        gameObject.SetActive(true);
        SetBox(BoxId);
        return GetAnimationSeq();
    }

    public void Close(){
        gameObject.SetActive(false);
    }

    private void SetBox(string BoxId){
        foreach(Transform child in box.transform){
            Object.Destroy(child.gameObject);
        }
        foreach(BoxType element in boxList){
            if(element.id == BoxId){
                Instantiate(element.gameObject, box.transform.position, Quaternion.identity, box.transform);
                return;
            }
        }
        return;
    }
}
