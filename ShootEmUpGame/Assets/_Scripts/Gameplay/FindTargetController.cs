using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;

public class FindTargetController : MonoBehaviour
{
    public enum TargetType
    {
        player, randomEnemy, closestEnemy
    }
    [SerializeField]
    protected TargetType _targetType;
    [SerializeField]
    protected bool _lockTargetOnAwake;
    GameObject _targetLocked;
    IEnumerator _lockTargetAction;

    void OnEnable()
    {
        if (_lockTargetOnAwake)
        {
            if (_lockTargetAction != null)
            {
                StopAutoLockTarget();
            }
            SetUpAutoLockTarget();
        }
    }

    /**
	 * Hàm trả về có lock target hay không
	 * */
    public bool HasLockedTarget
    {
        get
        {
            if (_lockTargetAction == null)
            {
                return false;
            }
            return true;
        }
    }

    public void SetUpAutoLockTarget()
    {
        if (_lockTargetAction == null)
        {
            _lockTargetAction = DoActionLockTarget();
            StartCoroutine(_lockTargetAction);
        }
    }

    IEnumerator DoActionLockTarget()
    {
        while (true)
        {
            yield return null;
            if (_targetLocked == null)
            {
                _targetLocked = FindTarget();
            }
            else
            {
                if (!_targetLocked.activeSelf)
                {
                    _targetLocked = null;
                }
                else if (Constants.IsOutOfSceneGamePlay(_targetLocked.transform.position, Vector2.one))
                {
                    _targetLocked = null;
                }
            }
        }
    }

    public void StopAutoLockTarget()
    {
        StopCoroutine(_lockTargetAction);
        _lockTargetAction = null;
    }

    public GameObject TargetLocked
    {
        get
        {
            return _targetLocked;
        }
    }

    public GameObject FindTarget()
    {
        if (_targetType == TargetType.closestEnemy)
        {
            return FindClosestEnemy();
        }
        else if (_targetType == TargetType.randomEnemy)
        {
            return FindRandomEnemy();
        }
        else if (_targetType == TargetType.player)
        {
            return FindPlayer();
        }
        return null;
    }

    GameObject FindClosestEnemy()
    {
        if (GamePlayManager.Instance.EnemyContainer != null)
        { // tìm quái thường để bắn
            float minDistance = -1;
            int iSave = -1;
            for (int i = 0; i < GamePlayManager.Instance.EnemyContainer.listEnemies.Count; i++)
            {
                IEnemyController tmpEnemy = GamePlayManager.Instance.EnemyContainer.listEnemies[i];
                if (tmpEnemy == null)
                {
                    GamePlayManager.Instance.EnemyContainer.listEnemies.RemoveAt(i);
                    i--;
                    continue;
                }

                if (tmpEnemy.gameObject.activeSelf
                    && !Constants.IsOutOfSceneGamePlay(((Vector2)tmpEnemy.transform.position), Vector2.one))
                {
                    float tmpSqrDistance = Vector2.SqrMagnitude((Vector2)transform.position - (Vector2)tmpEnemy.transform.position);
                    if (minDistance == -1)
                    {
                        iSave = i;
                        minDistance = tmpSqrDistance;
                    }
                    else
                    {
                        if (tmpSqrDistance < minDistance)
                        {
                            iSave = i;
                            minDistance = tmpSqrDistance;
                        }
                    }
                }
            }

            if (minDistance != -1 && iSave != -1)
            {
                return GamePlayManager.Instance.EnemyContainer.listEnemies[iSave].gameObject;
            }
        }
        return null;
    }

    GameObject FindRandomEnemy()
    {
        if (GamePlayManager.Instance.EnemyContainer != null)
        { // tìm quái thường để bắn
            List<IEnemyController> enemies = GamePlayManager.Instance.EnemyContainer.listEnemies;
            if (enemies.Count > 0)
            {
                int iRandom = Random.Range(0, enemies.Count);
                return enemies[iRandom].gameObject;
            }
        }
        return null;
    }

    GameObject FindPlayer()
    {
        return GamePlayManager.Instance.PlayerManager._shipController.gameObject;
    }
}
