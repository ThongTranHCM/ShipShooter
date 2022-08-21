using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnRelease : MonoBehaviour
{
    [SerializeField]
    private GameObject _pausePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended){
            _pausePanel.SetActive(true);
        } else {
            _pausePanel.SetActive(false);
        }
    }
}
