using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GamePlayManager : MonoBehaviour
{
    private static GamePlayManager _instance;
    public static GamePlayManager Instance
    {
        get { return _instance; }
    }

    public bool canShowScene = false;
    [SerializeField]
    private UIManager _uiManager;
    public UIManager UIManager
    {
        get { return _uiManager; }
    }
    [SerializeField]
    private PlayerManager _playerManager;
    public PlayerManager PlayerManager
    {
        get { return _playerManager; }
    }
    [SerializeField]
    private CollectionController _collection;
    public CollectionController Collection
    {
        get { return _collection; }
    }
    [SerializeField]
    private ItemSpawner _itemSpawner;
    public ItemSpawner ItemSpawner
    {
        get { return _itemSpawner; }
    }
    [SerializeField]
    private EnemyContainerController _enemyContainer;
    public EnemyContainerController EnemyContainer
    {
        get { return _enemyContainer; }
    }
    [SerializeField]
    private LevelDesignData _levelDesign;

    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera
    {
        get { return _mainCamera; }
    }
    [SerializeField]
    private LevelData _levelData;
    public LevelData Level
    {
        get { return _levelData; }
    }
    [SerializeField]
    private SoundManager _soundManager;
    public SoundManager SoundManager
    {
        get { return _soundManager; }
    }
    [SerializeField]
    private FillBarManager _progresBar;
    public FillBarManager ProgressBar
    {
        get { return _progresBar; }
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _instance = this;
        _soundManager.PlayBGM("bgm_1");
        StartCoroutine(InstallGameRoutine());
    }
    private IEnumerator InstallGameRoutine()
    {
        yield return _playerManager.Install();
        if (DataManager.Instance == null)
        {
            Debug.LogError("Should Start From mainMenu. Load Level ?");
        }
        else
        {
            Debug.LogError("Load Level " + DataManager.Instance.selectedLevelIndex);
            string file = "RandomLevelDesignData/" + DataManager.Instance.selectedLevelIndex;
            _levelDesign = Resources.Load<LevelDesignData>(file);
        }
        _collection.Install();
        StartCoroutine(GameFlow());
        canShowScene = true;
    }

    public void QuitGame()
    {
        SceneLoader.LoadLevel(Constants.SCENENAME_MainMenu);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void GameOver(){
        PauseGame();
        _uiManager.ShowLoseScreen(false);
    }

    private IEnumerator GameFlow(){
        yield return StartCoroutine(_levelDesign.InstallWaves());
        yield return new WaitForSeconds(3);
        UIManager.PlayVictory();
        yield return new WaitForSeconds(4.5f);
        Debug.LogError("Fake Win");
        DataManager.Instance.LastLevelWin = Mathf.Min(3, DataManager.Instance.LastLevelWin + 1);
        DataManager.Instance.playerData.Coin += GamePlayManager.Instance.Collection.gold;
        DataManager.Instance.playerData.Diamond += 2;
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_MainMenu);
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void OnEnemyGetKilled(ThongNguyen.PlayerController.IEnemyController enemy)
    {
        _collection.AddScore((int)enemy.MaxHp / 10);
        _soundManager.PlaySFX("enemy_death");
        _levelDesign.DropOnKill(enemy);
        _playerManager.OnKillEnemy(enemy);
    }
    public void OnEnemyGetDamage(ThongNguyen.PlayerController.IEnemyController enemy, float damage, ApplyEffectData.DamageSource damageSource)
    {
        _playerManager.OnEnemyGetDamage(enemy, damage, damageSource);
        _soundManager.PlaySFX("hit");
    }
    public void OnEnemyRemove(ThongNguyen.PlayerController.IEnemyController enemy){
        _progresBar.SetValue(_levelDesign.GetProgress());
    }
}
