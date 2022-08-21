using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadSingleton : MonoBehaviour
{
    public static bool isLoadFinished;
    [Header("List Singleton: ")]
    [SerializeField]
    private List<GameObject> _singletonObjects;

    private static AutoLoadSingleton _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            StartCoroutine(Load());
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }

    }

    IEnumerator Load()
    {
        for (int i = 0; i < _singletonObjects.Count; i++)
        {
            if (_singletonObjects[i] != null)
            {
                GameObject _obj = Instantiate(_singletonObjects[i]);
            }

            yield return null;
        }

        isLoadFinished = true;
    }
}
