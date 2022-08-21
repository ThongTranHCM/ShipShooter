using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipHealthBar : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMesh;
    private bool _isInstalled;
    public void Install(int iLifeCount)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (!_isInstalled)
        {
            _isInstalled = true;
        }
        UpdateLives(iLifeCount);
    }
    public void UpdateLives(int iLifeCount)
    {
        _textMesh.text = "x" + iLifeCount;
    }
}
