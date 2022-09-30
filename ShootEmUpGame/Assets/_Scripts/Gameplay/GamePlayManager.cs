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
        string selectedMode = "";
        if (DataManager.Instance == null)
        {
            Debug.LogError("Should Start From mainMenu. Load Level ?");
        }
        else
        {
            selectedMode = DataManager.Instance.selectedMode;
            levelIndex = DataManager.Instance.selectedLevelIndex;
            shipIndex = DataManager.Instance.selectedShipIndex;
        }
        yield return _playerManager.Install(shipIndex);
        switch (selectedMode)
        {
            case Constants.MODE_Endless:
                {
                    _levelDesign = Level.GetEndlessLevelDataFromRank(levelIndex);
                }
                break;
            case Constants.MODE_Challenge:
                {
                    _levelDesign = Level.GetLevelDataFromChallengeShipAndIndex(shipIndex, levelIndex);
                }
                break;
            case Constants.MODE_Story:
            default:
                {
                    string file = "RandomLevelDesignData/" + levelIndex;
                    _levelDesign = Resources.Load<LevelDesignData>(file);
                }
                break;
        }
        Debug.LogError("LevelDesign " + _levelDesign.name);
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

    public void ShowGameOver(){
        LTSeq seq = LeanTween.sequence();
        seq.append(3.0f);
        seq.append(() => {
            PauseGame();
            _uiManager.ShowLoseScreen(false);
        }); 
    }

    private IEnumerator GameFlow(){
        yield return StartCoroutine(_levelDesign.StartGame());
        yield return StartCoroutine(_levelDesign.InstallWaves());
        yield return StartCoroutine(_levelDesign.EndGame());
        
        DataManager.Save();
        SceneLoader.LoadLevel(Constants.SCENENAME_MainMenu);
    }

    public IEnumerator StartGame(){
        UIManager.PlayLevelStart();
        yield return new WaitForSeconds(4.5f);
    }

    public IEnumerator EndGame(){
        yield return new WaitForSeconds(3);
        UIManager.PlayLevelEnd();
        yield return new WaitForSeconds(4.5f);
    }

    public void RewardCollect(){
        RewardResourceManager.Instance.AddGold(GamePlayManager.Instance.Collection.gold);
    }

    public void LoseGame(){
        _levelDesign.LoseGame();
    }

    public IEnumerator OutOfLife(){
        yield return new WaitForSeconds(3);
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
        if(TimeChestManager.Instance != null){
            TimeChestManager.Instance.ProgressMission("defeat_enemy",1);
        }
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
