using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryModeUIController : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _txtLevel;

    public void OnEnable()
    {
        _txtLevel.text = "Level " + (DataManager.Instance.LastLevelWin + 1);
    }
}
