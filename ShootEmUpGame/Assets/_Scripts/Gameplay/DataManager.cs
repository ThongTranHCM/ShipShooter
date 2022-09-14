using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

[SerializeField]
public class DataManager
{
    public static DataManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }
    public static bool isInited
    {
        get { return _instance != null; }
    }

    private static DataManager _instance;


    public static void Init()
    {
        if (_instance == null)
        {
            LoadData();
        }
    }

    public static string GetApplicationPersistentPath()
    {
        string path = "";
#if UNITY_ANDROID && !UNITY_EDITOR
		try {
			IntPtr obj_context = AndroidJNI.FindClass ("android/content/ContextWrapper");
			IntPtr method_getFilesDir = AndroidJNIHelper.GetMethodID (obj_context, "getFilesDir", "()Ljava/io/File;");

			using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
					IntPtr file = AndroidJNI.CallObjectMethod (obj_Activity.GetRawObject (), method_getFilesDir, new jvalue[0]);
					IntPtr obj_file = AndroidJNI.FindClass ("java/io/File");
					IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID (obj_file, "getAbsolutePath", "()Ljava/lang/String;");   

					path = AndroidJNI.CallStringMethod (file, method_getAbsolutePath, new jvalue[0]);                    

					if (path != null) {
						Debug.Log ("Got internal path: " + path);
					} else {
						Debug.Log ("Using fallback path");
						path = Application.persistentDataPath;
					}
				}
			}
		} catch (Exception e) {
			Debug.Log (e.ToString ());
		}
#else
        path = Application.persistentDataPath;
#endif
        Debug.Log(path);
        return path;
    }

    private static void LoadData()
    {
        Debug.LogError("LoadData");
        if (_instance != null)
        {
            Debug.Log("Already loaded");
        }
        else
        {
            string persistantDataPath = GetApplicationPersistentPath();
            string filePath = persistantDataPath + "/PlayerInfo1.dat";
            RegisterJsonImporter();

            string loadFilePath = null;

            if (System.IO.File.Exists(filePath))
            {
                loadFilePath = filePath;
            }
            if (loadFilePath == null)
            {
                ResetToDefault();
            }
            else
            {
                string encrypted = System.IO.File.ReadAllText(loadFilePath);
                string json = encrypted;
                _instance = JsonFieldOnlyMapper.ToObject<DataManager>(json);

            }
        }
    }

    private static volatile bool isSaving = false;
    public static volatile bool isChangeCurrency = false;
    public static volatile bool isChangeResources = false;
    public static volatile bool isChangeProgress = false;
    public static void Save(bool saveInBackground = false, string path = "")
    {
        if (path == "")
            path = GetApplicationPersistentPath();

        if (isSaving) return;
        isSaving = true;

        string filePath = path + "/PlayerInfo1.dat";
        string filePathTemp = path + "/PlayerInfo1.tmp";

        try
        {
            RegisterJsonImporter();
            string json = GenerateDataJsonString();

            string encrypted = json;
            Debug.LogError("Json " + json);

            Exception error = null;
            try
            {
                System.IO.File.WriteAllText(filePathTemp, encrypted);
            }
            catch (Exception e)
            {
                error = e;
                throw;
            }
            Debug.LogError("Throw " + (error != null));

            // Throwed exception, may not need this If 
            if (error == null)
            {
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                System.IO.File.Move(filePathTemp, filePath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        finally
        {
            Debug.LogError("Saving ");
            isSaving = false;
            if (isChangeCurrency || isChangeResources || isChangeProgress)
            {
                MainMenuController.UpdateUIBaseOnData();
                isChangeCurrency = false;
                isChangeResources = false;
                isChangeProgress = false;
            }
        }
    }
    public static string GenerateDataJsonString()
    {
        RegisterJsonImporter();
        return JsonFieldOnlyMapper.ToJson(_instance);
    }

    static void RegisterJsonImporter()
    {
        JsonFieldOnlyMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
        JsonFieldOnlyMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
        JsonFieldOnlyMapper.RegisterImporter<int, long>((int value) =>
        {
            return (long)value;
        });
    }
    public static void ResetToDefault()
    {
        _instance = new DataManager();
        int shipDefault = 0;
        _instance.playerData.GetShipProgress(shipDefault).shipLevel = 1;
    }
    #region Ship/AddOns
    public PlayerData playerData
    {
        get
        {
            if (_playerData == null)
            {
#if TEST
				Debug.Log("--- Create New PlayerData! ---");
#endif
                _playerData = new PlayerData();
                _playerData.InitData();
            }
            return _playerData;
        }
        set
        {
            _playerData = value;
        }
    }
    public AddOnUserData addOnUserData
    {
        get
        {
            if (_addOnUserData == null)
            {
#if TEST
				Debug.Log("--- Create New PlayerData! ---");
#endif
                _addOnUserData = new AddOnUserData();
                _addOnUserData.InitData();
            }
            return _addOnUserData;
        }
        set
        {
            _addOnUserData = value;
        }
    }
    public TimeChestManager.Data timeChestManagerData
    {
        get
        {
            if (_timeChestManagerData == null)
            {
#if TEST
				Debug.Log("--- Create New timeChestData! ---");
#endif
                _timeChestManagerData = new TimeChestManager.Data();
                _timeChestManagerData.InitData();
            }
            return _timeChestManagerData;
        }
        set
        {
            _timeChestManagerData = value;
        }
    }
    public DailyDealManager.Data dailyDealManagerData
    {
        get
        {
            if (_dailyDealManagerData == null)
            {
#if TEST
				Debug.Log("--- Create New timeChestData! ---");
#endif
                _dailyDealManagerData = new DailyDealManager.Data();
                _dailyDealManagerData.InitData();
            }
            return _dailyDealManagerData;
        }
        set
        {
            _dailyDealManagerData = value;
        }
    }
    public DailyOfferManager.Data dailyOfferManagerData
    {
        get
        {
            if (_dailyOfferManagerData == null)
            {
#if TEST
				Debug.Log("--- Create New timeChestData! ---");
#endif
                _dailyOfferManagerData = new DailyOfferManager.Data();
                _dailyOfferManagerData.InitData();
            }
            return _dailyOfferManagerData;
        }
        set
        {
            _dailyOfferManagerData = value;
        }
    }
    private PlayerData _playerData;
    private AddOnUserData _addOnUserData;
    private TimeChestManager.Data _timeChestManagerData;
    private DailyDealManager.Data _dailyDealManagerData;
    private DailyOfferManager.Data _dailyOfferManagerData;
    #endregion
    #region Player Progression
    private int _lastLevelWin;
    public int LastLevelWin
    {
        get { return _lastLevelWin; }
        set { _lastLevelWin = value; }
    }
    private int _lastShipIndex;
    public int LastShipIndex
    {
        get { return _lastShipIndex; }
        set { _lastShipIndex = value; }
    }
    #endregion
    #region Temporary Data, Used in short period of time, Generated By Client
    [System.NonSerialized] public bool dataSubmitScoreStoryMode;
    [System.NonSerialized] public int selectedLevelIndex;
    [System.NonSerialized] public int selectedShipIndex;
    [System.NonSerialized] public int selectedShipLevel;
    [System.NonSerialized] public int dailyDealStartTime;
    #endregion
}