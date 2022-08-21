using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    private Transform _tfCenter;
    private float _distance;
    [SerializeField]
    private float _angle;
    [SerializeField]
    private float _rotateAngle;
    [SerializeField]
    private float _speed;

    private void Start()
    {
        _tfCenter = transform.parent;
        _distance = (transform.position - _tfCenter.position).magnitude;
        _angle *= Mathf.Deg2Rad;
        _speed *= Mathf.Deg2Rad;
    }

    public void FixedUpdate()
    {
        _angle += Time.deltaTime * _speed;
        transform.position = _tfCenter.position + _distance * new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), 0);
        transform.eulerAngles = new Vector3(0, 0, -_angle * Mathf.Rad2Deg + _rotateAngle);
    }
}
