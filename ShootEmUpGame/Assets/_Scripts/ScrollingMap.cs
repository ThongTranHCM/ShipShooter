using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingMap : MonoBehaviour
{
    [SerializeField]
    private float _scrollSpeed;
    [SerializeField]
    private float _randomWidth;
    [SerializeField]
    private float _width;
    [SerializeField]
    private float _stepSize;
    [SerializeField]
    private int _numStep;
    [SerializeField]
    private LineRenderer _prevTrace;
    // Start is called before the first frame update
    void Start()
    {
        _prevTrace = CreateTrace(_prevTrace);
    }

    private LineRenderer CreateTrace(LineRenderer line){
        int count = _numStep;
        line.positionCount = count;
        var points = new Vector3[line.positionCount];
        for (int i = 0; i < count; i++){
            points[i] = Vector3.zero + Vector3.right * Random.Range(-_randomWidth, _randomWidth);
        }
        for (int i = 0; i < count; i++){
            points[i] += Vector3.up * i * _stepSize ;
        }
        line.SetPositions(points);
        line.endWidth = line.startWidth = _width;
        return line;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position -= Vector3.up * _scrollSpeed * Time.fixedDeltaTime;
    }
}
