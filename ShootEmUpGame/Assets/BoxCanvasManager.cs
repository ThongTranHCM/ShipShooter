using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCanvasManager : MonoBehaviour
{
    [SerializeField]
    GameObject box;
    [SerializeField]
    Transform start;
    [SerializeField]
    Transform stop;
    public LTSeq GetAnimationSeq(){
        Vector3 stretch = Vector3.up * 2 + Vector3.right * 0.5f + Vector3.forward;
        Vector3 normal = Vector3.one;
        Vector3 squeeze = Vector3.one * 0.85f;
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.move(box,start,0f));
        seq.append(LeanTween.scale(box, stretch, 0f));
        seq.append(() => {
            LeanTween.move(box,stop,1.0f).setEase(LeanTweenType.easeOutBounce);
            LeanTween.scale(box,normal,1.0f).setEase(LeanTweenType.easeOutBounce);
        });
        seq.append(1.0f);
        seq.append(() => {
            LeanTween.rotateZ(box,5f,0.05f).setEase(LeanTweenType.easeShake).setRepeat(20);
            LeanTween.scale(box,squeeze,1.0f).setEase(LeanTweenType.easeOutSine);
        });
        seq.append(1.0f);
        seq.append(LeanTween.scale(box,stretch,0.1f).setEase(LeanTweenType.easeInBack));
        seq.append(() => {gameObject.SetActive(false);});
        return seq;
    }
}
