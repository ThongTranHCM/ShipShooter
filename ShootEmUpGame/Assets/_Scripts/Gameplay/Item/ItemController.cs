using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;

public class ItemController : MonoBehaviour
{
    private AddOnEquipData.AddOnType _addOnType;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public System.Action onCollide, onOutCamera;
    public void OnCollidePlayer()
    {
        onCollide?.Invoke();
        if (_addOnType != AddOnEquipData.AddOnType.None)
        {
            GamePlayManager.Instance.PlayerManager.InstallAddOn(_addOnType);
            SoundManager.Instance.PlaySFX("collect_powerup");
            _addOnType = AddOnEquipData.AddOnType.None;
        }
        Despawn();
    }

    public void SetUpItemAddOn(AddOnEquipData.AddOnType addOnType, Sprite sprite)
    {
        _addOnType = addOnType;
        _spriteRenderer.sprite = sprite;
    }

    public void Update()
    {
        if (Constants.IsOutOfSceneGamePlay(((Vector2)transform.position), new Vector2(1, 1)))
        {
            onOutCamera?.Invoke();
            Despawn();
        }
    }

    private void Despawn()
    {
        onCollide = null;
        onOutCamera = null;
        _addOnType = AddOnEquipData.AddOnType.None;
        PoolManager.Pools[Constants.poolOnHitEffect].Despawn(transform);
    }
}
