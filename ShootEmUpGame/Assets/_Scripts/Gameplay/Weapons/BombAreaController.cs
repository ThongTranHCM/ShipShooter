using System.Collections;
using UnityEngine;
using ThongNguyen.PlayerController;
using MathUtil;

public class BombAreaController : BulletController
{
    [SerializeField]
    private SpriteRenderer _areaSprite;
    [SerializeField]
    private Transform _tfVisual;
    [SerializeField]
    private float _bombTime;

    private bool isPaused;
    private float _startTime;
    [SerializeField]
    private float _size;
    private IEnumerator bombTask;
    private int curBulletIndex = 0;
    private LowPassFilter _lowPassFire;
    private Color _baseColor;
    private float _Fire;
    
    public override void Install(IGunController gun, BulletData gunBulletData, Vector2 startPos, Vector2 des)
    {
        _lowPassFire = new LowPassFilter(2f);
        _baseColor = _areaSprite.color;
        Run();
        _startTime = Time.time;
        base.Install(gun, gunBulletData, startPos, des);
    }

    public void Run()
    {
        isPaused = false;
        bombTask = NewBomb();
        StartCoroutine(bombTask);
    }

    private void StopBomb()
    {
        isPaused = true;
        bombTask = null;
        GamePoolManager.PoolManager.Pools[Constants.poolNameBulletsOfPlayer].Despawn(transform);
    }

    void Update()
    {
        float size = _lowPassFire.Output() / _lowPassFire.GetAlpha();
        size = Mathf.Clamp(size, 0.6f, 1);
        Debug.LogError("Bomb " + gameObject.name);
        Color color = _areaSprite.color;
        color.a = size * 0.25f;
        _areaSprite.color = color;
        _tfVisual.localScale = new Vector3(size, size, 1);
        _lowPassFire.Input(_Fire);
        _Fire = 0;
        if (!isPaused && (Time.time - _startTime > _bombTime))
        {
            StopBomb();
        }
    }

    private IEnumerator NewBomb()
    {
        curBulletIndex = 0;
        Collider2D[] arrayCollider = new Collider2D[40];
        while (!isPaused)
        {
            int count = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position, _size, arrayCollider);
            for (int i = 0; i < count; i++)
            {
                Collider2D collider = arrayCollider[i];
                if (collider != null)
                {
                    TopDownCharacterController character = collider.transform.GetComponent<TopDownCharacterController>();
                    if (character != null)
                    {
                        character.SetUpHit(bulletData, character.transform.position);
                    }
                }
            }
            if (curBulletIndex > 2)
            {
                curBulletIndex = 0;
                yield return Yielder.Get(0.2f);
                _Fire = 1;
            }
            else
            {
                curBulletIndex++;
                yield return Yielder.Get(1);
                _Fire = 1;
            }
            yield return null;
        }
    }
}