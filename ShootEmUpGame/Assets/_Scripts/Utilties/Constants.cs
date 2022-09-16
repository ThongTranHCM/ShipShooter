using UnityEngine;
public static class Constants
{
    public const string SCENENAME_GamePlay = "GamePlay";
    public const string SCENENAME_GamePlay_Tutorial = "GamePlay";
    public const string SCENENAME_MainMenu = "MainMenu";

    public const string MODE_Story = "Normal";
    public const string MODE_Endless = "Endless";
    public const string MODE_Challenge = "Challenge";

    public const string tagEnemy = "Enemy";
    public const string tagPlayer = "Player";
    public const string tagItem = "Item";

    public const string kQuitWarning = "You want to quit?";

    public const string poolNameBulletsOfPlayer = "BulletsOfPlayer";
    public const string poolNameBulletsOfEnemy = "BulletsOfEnemy";
    public const string poolNameEnemyGun = "EnemyGun";
    public const string poolMonster = "Monsters";
    public const string poolOnHitEffect = "OnHitEffect";

    static Vector2 sizeOfCamera = Vector2.zero;

    public static T GetAssest<T>(string _path) where T : Object
    {
        if (_path.Equals(""))
        {
            return null;
        }
        return Resources.Load<T>(_path);
    }

    public static Vector2 SizeOfCamera()
    {
        if (sizeOfCamera == Vector2.zero)
        {
            CalculateSizeCamera();
        }
        return sizeOfCamera;
    }

    public static void CalculateSizeCamera()
    {
        Vector2 A = new Vector2();
        A.y = Camera.main.orthographicSize * 2;
        A.x = (Camera.main.aspect * Camera.main.orthographicSize) * 2;
        sizeOfCamera = A;
    }

    public static bool IsOutOfSceneGamePlay(Vector3 point, Vector2 size)
    {
        Vector3 posCam = GamePlayManager.Instance.MainCamera.transform.position;
        Vector3 sizeGamePlay = SizeOfCamera();
        if (point.x + size.x / 2 < posCam.x - sizeGamePlay.x / 2
            || point.x - size.x / 2 > posCam.x + sizeGamePlay.x / 2
            || point.y + size.y / 2 < posCam.y - sizeGamePlay.y / 2
            || point.y - size.y / 2 > posCam.y + sizeGamePlay.y / 2)
        {
            return true;
        }
        return false;
    }
    public static Vector2 GetTopLeftScreen()
    {
        Vector3 posCam = GamePlayManager.Instance.MainCamera.transform.position;
        Vector3 sizeGamePlay = SizeOfCamera();
        return new Vector2(posCam.x - sizeGamePlay.x/2, posCam.y - sizeGamePlay.y/2);
    }
    public static Vector2 GetBottomRightScreen()
    {
        Vector3 posCam = GamePlayManager.Instance.MainCamera.transform.position;
        Vector3 sizeGamePlay = SizeOfCamera();
        return new Vector2(posCam.x + sizeGamePlay.x / 2, posCam.y + sizeGamePlay.y / 2);
    }
}
