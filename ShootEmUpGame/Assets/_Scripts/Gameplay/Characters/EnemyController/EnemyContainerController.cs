using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThongNguyen.PlayerController;

/**
 * Summary: Lớp quản lý các enemy được summon ra trong lúc chơi
 * */
public class EnemyContainerController : MonoBehaviour
{
    public List<IEnemyController> listEnemies { get; set; }
    public EnemyContainerController()
    {
        listEnemies = new List<IEnemyController>(200);
    }

    public void AddEnemy(IEnemyController _enemyController)
    {
        listEnemies.Add(_enemyController);
    }

    public void RemoveEnemy(IEnemyController _enemyController)
    {
        listEnemies.Remove(_enemyController);
    }
}