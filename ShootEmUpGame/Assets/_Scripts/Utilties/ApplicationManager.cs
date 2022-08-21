using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ApplicationManager>();
                if (_instance != null)
                {
                    //Tell unity not to destroy this object when loading a new scene!
                    DontDestroyOnLoad(_instance.gameObject);
                    _instance.Init();
                }
            }

            return _instance;
        }
    }
    private static ApplicationManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    private void Init()
    {
        InitDataManager(()=> {
            Debug.LogError("Done");
        });
    }

    private void InitDataManager(System.Action onFinished)
    {
        try
        {
            DataManager.Init();
            if (onFinished != null) onFinished();
        }
        catch (System.Exception e)
        {
        }
    }
}
