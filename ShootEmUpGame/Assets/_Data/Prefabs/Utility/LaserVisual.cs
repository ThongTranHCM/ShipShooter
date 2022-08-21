using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtil;

public class LaserVisual : MonoBehaviour
{
    private LowPassFilter _lowPassFire;
    private float _fire;
    [SerializeField]
    private float _defaultFire;
    [SerializeField]
    private float _lpFilterValue;
    private Color _baseColor;
    [SerializeField]
    private SpriteRenderer _laserSprite;
    [SerializeField]
    private SpriteRenderer[] _sprites;
    [SerializeField]
    private bool _fullVertical;

    public void Start()
    {
        _lowPassFire = new LowPassFilter(_lpFilterValue);
        _baseColor = _laserSprite.color;
        if (_fullVertical)
        {
            for (int i = 0; i < _sprites.Length; i++)
            {
                //_sprites[i].size = new Vector3(1, 12);
            }
        }
    }

    public void OnEnable()
    {
        _fire = _defaultFire;
        Vector3 pos = transform.position;
        pos.y = -Constants.SizeOfCamera().y / 2;
        transform.position = pos;
    }
    public void SetFire(float fireValue)
    {
        _fire = fireValue;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float size = _lowPassFire.Output() / _lowPassFire.GetAlpha();
        Vector3 scale = this.transform.localScale;
        Color color = _laserSprite.color;
        color.a = size * 0.25f;
        _laserSprite.color = color;
        scale.x = size;
        this.transform.localScale = scale;
        _lowPassFire.Input(_fire);
        _fire = 0;
    }
}
