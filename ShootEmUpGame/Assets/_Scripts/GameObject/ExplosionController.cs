using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    [SerializeField]
    private GameObject _lightGameObject;
    [SerializeField]
    private GameObject _darkGameObject;
    [SerializeField]
    private float _speed = 0;
    [SerializeField]
    private float _intensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _lightGameObject.transform.localScale = Vector3.one * (1 + _intensity * Mathf.Cos(Time.time * _speed));
        _darkGameObject.transform.localScale = Vector3.one * (1 - _intensity * Mathf.Cos(Time.time * _speed));
    }
}
