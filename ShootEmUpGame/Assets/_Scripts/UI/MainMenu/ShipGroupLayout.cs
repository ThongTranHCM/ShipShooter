using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGroupLayout : MonoBehaviour
{
    [SerializeField]
    private GameObject _shipUIPrefab;

    private List<ShipSelectUIController> _listShipSelectUI;

    [SerializeField]
    private bool _updateParentSize;
    [SerializeField]
    private UnityEngine.UI.GridLayoutGroup _gridLayout;

    public System.Action<int> callbackUIClick;

    public void OnEnable()
    {
        if (_listShipSelectUI == null) _listShipSelectUI = new List<ShipSelectUIController>();
        Install();
    }

    public void Install()
    {
        List<DOShipData> listShipData = GameInformation.Instance.shipData;
        List<int> listQualifiedShip = new List<int>();
        DOShipData shipData;
        PlayerData.ShipProgressData shipProgress;

        for (int i = 0; i < listShipData.Count; i++)
        {
           listQualifiedShip.Add(i);
        }
        UpdateListUISize(listQualifiedShip.Count);
        for (int i = 0; i < listQualifiedShip.Count; i++)
        {
            shipData = listShipData[listQualifiedShip[i]];
            shipProgress = DataManager.Instance.playerData.GetShipProgress(i);
            ShipSelectUIController _shipSelectItem = _listShipSelectUI[i];
            int tmp = i;
            _shipSelectItem.Install(shipData.spritePresentShip, shipProgress.shipLevel, shipData.shipName);
            _shipSelectItem.onBtnClick = () => OnShipItemClick(tmp);
        }
        SelectShip(0);
    }

    public void SelectShip(int index)
    {
        for (int i = 0; i < _listShipSelectUI.Count; i++)
        {
            if (index == i)
            {
                _listShipSelectUI[i].ShowAsSelected();
            }
            else
            {
                _listShipSelectUI[i].ShowAsShip();
            }
        }
    }

    public void UpdateListUISize(int listSize)
    {
        int currentListSize = _listShipSelectUI.Count;
        while (currentListSize > listSize)
        {
            currentListSize--;
            Destroy(_listShipSelectUI[listSize].gameObject);
            _listShipSelectUI.RemoveAt(listSize);
        }
        while (currentListSize < listSize)
        {
            currentListSize++;
            GameObject shipItem = Instantiate<GameObject>(_shipUIPrefab, transform);
            ShipSelectUIController shipUIItem = shipItem.GetComponent<ShipSelectUIController>();
            _listShipSelectUI.Add(shipUIItem);
        }
        if (_updateParentSize)
        {
            UpdateParentSize();
        }
    }

    private void OnShipItemClick(int id)
    {
        Debug.LogError("ShipItem " + id);
        callbackUIClick?.Invoke(id);
    }

    private void UpdateParentSize()
    {
        RectTransform rectTf = transform.parent as RectTransform;

        float width = rectTf.rect.width;
        int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - _gridLayout.padding.horizontal + _gridLayout.spacing.x + 0.001f) / (_gridLayout.cellSize.x + _gridLayout.spacing.x)));
        int minRows = Mathf.CeilToInt(_listShipSelectUI.Count / (float)cellCountX);

        float minSpace = _gridLayout.padding.vertical + (_gridLayout.cellSize.y + _gridLayout.spacing.y) * minRows - _gridLayout.spacing.y;
        rectTf.SetHeight(minSpace);
    }
}