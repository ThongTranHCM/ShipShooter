using System.Collections;
using UnityEngine;
using ThongNguyen.PlayerController;
using MathUtil;

public class LaserController : IGunController
{
    [Header("Laser")]
    public SpriteRenderer laserSprite;
    public LaserVisual laserVisual;

    private bool isPaused;
    [SerializeField]
    private Vector2 size;
    private IEnumerator laserTask;
    private int curBulletIndex = 0;
    public override void Install()
    {
        Run();
        base.Install();
    }

    public void Run()
    {
        isPaused = false;
        laserTask = NewLaser();
        StartCoroutine(laserTask);
    }

    private void StopLaser()
    {
        isPaused = true;
        laserTask = null;
    }

    private IEnumerator NewLaser()
    {
        curBulletIndex = 0;
        Collider2D[] arrayCollider = new Collider2D[40];
        while (!isPaused)
        {
            int count = Physics2D.OverlapBoxNonAlloc((Vector2)laserSprite.transform.position, size, 0, arrayCollider);
            for (int i = 0; i < count; i++)
            {
                Collider2D collider = arrayCollider[i];
                if (collider != null)
                {
                    TopDownCharacterController character = collider.transform.GetComponent<TopDownCharacterController>();
                    if (character != null)
                    {
                        character.SetUpHit(gunData.bulletData, character.transform.position);
                    }
                }
            }
            if (curBulletIndex > gunData.numOfBulletPerWave)
            {
                curBulletIndex = 0;
                yield return Yielder.Get(gunData.timeDelayForEachWave);
                laserVisual.SetFire(1);
            }
            else
            {
                curBulletIndex++;
                yield return Yielder.Get(1 / gunData.attackSpeed);
                laserVisual.SetFire(1);
            }
            yield return null;
        }
    }
}