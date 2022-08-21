using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemPrefab;

    public ItemController CreateAddOn(AddOnEquipData.AddOnType addOnType, Vector3 position)
    {
        //Need GameInformation to be loaded. Better to start from Main Menu.
        IAddOnData addOnData = GameInformation.Instance.addOnEquipData.GetAddOnData(addOnType);
        Transform addOnTf = PoolManager.Pools[Constants.poolOnHitEffect].Spawn(_itemPrefab, position, Quaternion.identity, transform);
        ItemController itemController = addOnTf.GetComponent<ItemController>();
        itemController.SetUpItemAddOn(addOnData.GetAddOnType, addOnData.GetSprite);
        return itemController;
    }
}
