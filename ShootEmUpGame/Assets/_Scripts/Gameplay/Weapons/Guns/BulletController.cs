using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePoolManager;

public class BulletController : MonoBehaviour
{
    public bool isBulletOfEnemy;
    public enum StyleMove
    {
        normal, follow
    }
    protected StyleMove _styleMove;
    public enum State
    {
        none, move
    }
    protected State _state;
    protected Vector3 _startPos;
    protected Vector3 _destination;
    [SerializeField]
    private bool _isDestroyed;
    public float _speed { get; set; }
    protected float timeToMove;
    private IEnumerator _moveTween;
    public bool autoDestroyWhenOutOfCamera;
    [System.NonSerialized]
    public bool isInstalled;
    [System.NonSerialized]
    public GameObject targetLocked;
    protected float zAngle; // góc lưu lại để dí target
    public FindTargetController findTargetController;

    protected IEnumerator actionFollowTarget, actionWaitAndStopFollowingTarget;
    protected float baseRotSpeed;
    static float posZ = 0f;


    public float timeFollowTarget; // = -1 là dí tới khi mục tiêu bị tiêu diệt
    public BulletData bulletData;
    #region UNITY FUNCTIONS
    protected void Awake()
    {
        baseRotSpeed = bulletData.rotSpeed;
        ResetData();
    }
    void OnEnable()
    {
        if (findTargetController != null)
            StartCoroutine(DoActionAutoFindTarget());
    }
    #endregion

    public virtual void Install(IGunController gun, BulletData gunBulletData)
    {
        if (!isInstalled)
        {
            _speed = gun.BulletSpeed();
            bulletData.Init(gunBulletData);
            bulletData.ApplyEfect.callbackSelfDestruction += SelfDestruction;
            transform.localScale = new Vector3(1, 1, 1);
            SetPosition();
            isInstalled = true;
        }
    }

    public virtual void Install(IGunController gun, BulletData gunBulletData, Vector2 startPos, Vector2 des)
    {
        if (!isInstalled)
        {
            _speed = gun.BulletSpeed();
            _destination = des;
            _startPos = startPos;
            bulletData.Init(gunBulletData);
            bulletData.ApplyEfect.callbackSelfDestruction += SelfDestruction;
            transform.localScale = new Vector3(1, 1, 1);
            SetPosition();
            isInstalled = true;
        }
    }

    public virtual void Install(BulletData otherBulletData, Vector2 startPos, Vector2 des)
    {
        if (!isInstalled)
        {
            _destination = des;
            _startPos = startPos;
            if (bulletData != null)
            {
                bulletData.Init(otherBulletData);
                bulletData.ApplyEfect.callbackSelfDestruction += SelfDestruction;
                bulletData.ApplyEfect.callbackShowEffectHit = AddEffectHit;
                bulletData.ApplyEfect.callbackShowEffectCritical = AddEffectCritical;
            }
            SetPosition();
            isInstalled = true;
        }
    }

    public void SetPosition()
    {
        _startPos.z = posZ;
        _destination.z = posZ;
        posZ += 0.2f;
        if (posZ > 50f)
        {
            posZ = 0f;
        }
    }
    public void SetUpFollow(GameObject target, float tzAngle = -1)
    {
        if (!isInstalled)
        {
            return;
        }
        if (_state != State.move)
        {
            _state = State.move;
            _styleMove = StyleMove.follow;

            autoDestroyWhenOutOfCamera = true;

            targetLocked = target;
            if (tzAngle != -1)
            {
                zAngle = tzAngle;
            }
            else
            {
                zAngle = transform.eulerAngles.z;
            }

            if (autoDestroyWhenOutOfCamera && gameObject.activeSelf)
            {
                StartCoroutine(DoActionDestroyWhenOutOfCamera());
            }

            if (actionWaitAndStopFollowingTarget == null)
            {
                actionWaitAndStopFollowingTarget = DoActionWaitAndStopFollowingTarget();
                StartCoroutine(actionWaitAndStopFollowingTarget);
            }
            if (actionFollowTarget == null)
            {
                actionFollowTarget = DoActionFollow();
                StartCoroutine(actionFollowTarget);
            }
        }
    }


    protected IEnumerator DoActionWaitAndStopFollowingTarget()
    {
        if (_state != State.move || _styleMove != StyleMove.follow)
        {
            actionWaitAndStopFollowingTarget = null;
            yield break;
        }
        if (timeFollowTarget == -1 || targetLocked == null || !targetLocked.activeSelf)
        {
            actionWaitAndStopFollowingTarget = null;
            yield break;
        }
        yield return new WaitForSeconds(timeFollowTarget);
        _styleMove = StyleMove.normal;
        actionWaitAndStopFollowingTarget = null;
    }
    protected IEnumerator DoActionFollow()
    {
        while (_state == State.move)
        {
            yield return Yielder.FixedUpdate;

            if (_styleMove == StyleMove.follow)
            {
                if (targetLocked != null)
                {
                    if (!targetLocked.activeSelf) // Có target, nhưng target không active
                    {
                        targetLocked = null;
                        if (bulletData.rotSpeed == -1)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, zAngle);
                        }
                        else
                        {
                            Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot,  bulletData.rotSpeed * Time.fixedDeltaTime);
                        }
                    }
                    else // Có target & đang active -> dí target
                    {
                        Vector3 dir = targetLocked.transform.position - transform.position;
                        dir.Normalize();
                        zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

                        if (bulletData.rotSpeed == -1)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, zAngle);
                        }
                        else
                        {
                            Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, bulletData.rotSpeed * Time.fixedDeltaTime);
                        }
                    }
                }
                else // Nếu không có target để focus thì bay thẳng
                {
                    if (bulletData.rotSpeed == -1)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, zAngle);
                    }
                    else
                    {
                        Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, bulletData.rotSpeed * Time.fixedDeltaTime);
                    }
                }
            }

            Vector3 pos = transform.position;
            Vector3 velocity = new Vector3(0, _speed * Time.fixedDeltaTime, 0); // di chuyển theo y
            pos += (transform.rotation * velocity);
            transform.position = pos;
        }
        actionFollowTarget = null;
    }
    public virtual void SetUpMove()
    {
        if (!isInstalled)
        {
            return;
        }
        timeToMove = Vector2.Distance(_startPos, _destination) / _speed;

        _moveTween = MoveToDestination(timeToMove, () =>
        {
            _moveTween = null;
            SelfDestruction();
        });

        if (autoDestroyWhenOutOfCamera)
        {
            StartCoroutine(DoActionDestroyWhenOutOfCamera());
        }
        StartCoroutine(_moveTween);
    }

    IEnumerator MoveToDestination(float time, System.Action onComplete)
    {
        float t = 0;
        while (t < time)
        {
            transform.position = Vector3.Lerp(_startPos, _destination, (t / time));
            yield return Yielder.FixedUpdate;
            t += Time.fixedDeltaTime;
        }
        transform.position = _destination;
        onComplete?.Invoke();
    }

    protected IEnumerator DoActionDestroyWhenOutOfCamera()
    {
        while (true)
        {
            yield return null;
            if (Constants.IsOutOfSceneGamePlay(transform.position, transform.localScale))
            {
                if (_moveTween != null)
                {
                    StopCoroutine(_moveTween);
                    _moveTween = null;
                }

                SelfDestruction();
                yield break;
            }
        }
    }


    IEnumerator DoActionAutoFindTarget()
    {
        while (true)
        {
            yield return Yielder.FixedUpdate;
            if (targetLocked == null || !targetLocked.activeSelf)
            {
                if (findTargetController != null)
                    targetLocked = findTargetController.FindTarget();
            }
        }
    }
    protected void OnDespawned()
    {
        ResetData();
    }

    public virtual void SelfDestruction()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (bulletData != null && bulletData.ApplyEfect != null
            && bulletData.ApplyEfect.callbackShotMiss != null)
        {
            bulletData.ApplyEfect.callbackShotMiss?.Invoke();
        }
        StopAllCoroutines();
        _moveTween = null;
        if (isBulletOfEnemy)
        {
            PoolManager.Pools[Constants.poolNameBulletsOfEnemy].Despawn(gameObject.transform);
        }
        else
        {
            PoolManager.Pools[Constants.poolNameBulletsOfPlayer].Despawn(gameObject.transform);
        }
    }

    public void ResetData()
    {
        StopAllCoroutines();
        _styleMove = StyleMove.normal;
        _state = State.none;
        transform.localScale = new Vector3(1, 1, 1);

        bulletData.rotSpeed = baseRotSpeed;
        isInstalled = false;
        targetLocked = null;
        actionFollowTarget = actionWaitAndStopFollowingTarget = null;

        if (_moveTween != null)
        {
            _moveTween = null;
        }
    }

    public void AddEffectHit(Transform tf, Vector2 posHit) { }
    public void AddEffectCritical(Transform tf, Vector2 posHit) { }
}
