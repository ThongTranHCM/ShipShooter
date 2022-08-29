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
    [Header("System")]

    public bool canShowScene = false;
    [SerializeField]
    private UIManager _uiManager;
    public UIManager UIManager
    {
        get { return _uiManager; }
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
    private FillBarManager _progresBar;
    public FillBarManager ProgressBar
    {
        get { return _progresBar; }
    }

    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera
    {
        get { return _mainCamera; }
    }

    [Header("Level")]
    [SerializeField]
    private EnemyContainerController _enemyContainer;
    public EnemyContainerController EnemyContainer
    {
        get { return _enemyContainer; }
    }
    [SerializeField]
    private LevelDesignData _levelDesign;

    [SerializeField]
    private LevelData _levelData;
    public LevelData Level
    {
        get { return _levelData; }
    }

    [Header("Player")]

    [SerializeField]
    private PlayerManager _playerManager;
    public PlayerManager PlayerManager
    {
        get { return _playerManager; }
    }

    [SerializeField]
    private DPadController _dpadController;
    public DPadController DPad
    {
        get { return _dpadController; }
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _instance = this;
    }

    private void Start(){
        SoundManager.Instance.PlayBGM("bgm_1");
        StartCoroutine(InstallGameRoutine());
    }
    private IEnumerator InstallGameRoutine()
    {
        int levelIndex = 1;
        int shipIndex = 0;
        if (DataManager.Instance == null)
        {
            Debug.LogError("Should Start From mainMenu. Load Level ?");
        }
        else
        {
            levelIndex = DataManager.Instance.selectedLevelIndex;
            shipIndex = DataManager.Instance.selectedShipIndex;
        }
        yield return _playerManager.Install(shipIndex);
        string file = "RandomLevelDesignData/" + levelIndex;
        _levelDesign = Resources.Load<LevelDesignData>(file);
        _uiManager.SetStageText(levelIndex);
        _collection.Install();
        yield return new WaitUntil(() => _levelDesign != null);
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
        LTSeq seq = LeanTween.sequence();
        seq.append(3.0f);
        seq.append(() => {
            PauseGame();
            _uiManager.ShowLoseScreen(false);
        }); 
    }

    private IEnumerator GameFlow(){
        UIManager.PlayLevelStart();
        yield return new WaitForSeconds(4.5f);
        Debug.LogError("LevelDesign");
        yield return StartCoroutine(_levelDesign.InstallWaves());
        yield return new WaitForSeconds(3);
        UIManager.PlayLevelEnd();
        yield return new WaitForSeconds(4.5f);
        Debug.LogError("Fake Win");
        DataManager.Instance.LastLevelWin++;
        DataManager.Instance.playerData.Coin += GamePlayManager.Instance.Collection.gold;
        DataManager.Instance.playerData.Diamond += 2;
        RewardResourceManager.Instance.AddGold(GamePlayManager.Instance.Collection.gold);
        RewardResourceManager.Instance.AddDiamond(2);
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
        SoundManager.Instance.PlaySFX("enemy_death");
        _levelDesign.DropOnKill(enemy);
        _playerManager.OnKillEnemy(enemy);
    }
    public void OnEnemyGetDamage(ThongNguyen.PlayerController.IEnemyController enemy, float damage, ApplyEffectData.DamageSource damageSource)
    {
        _playerManager.OnEnemyGetDamage(enemy, damage, damageSource);
        SoundManager.Instance.PlaySFX("hit");
    }
    public void OnEnemyRemove(ThongNguyen.PlayerController.IEnemyController enemy){
        _progresBar.SetValue(_levelDesign.GetProgress());
    }
}
