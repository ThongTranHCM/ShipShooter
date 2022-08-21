using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathUtil;

public class CharacterHealthbar : MonoBehaviour
{
    [SerializeField]
    private float _damping;
    [SerializeField]
    private float _maxHealth;
    private float _currentHealth;
    private LowPassFilter _lowPassHealth;
    private float _lastUpdate;
    private bool _isShowVisual;
    [SerializeField]
    private SpriteRenderer healthSprite;
    [SerializeField]
    private SpriteRenderer slowHealthSprite;
    [SerializeField]
    private SpriteRenderer emptyHealthSprite;
    [SerializeField]
    private GameObject visualObject;
    [SerializeField]
    private float showDuration;
    private bool isInstalled = false;
    public Transform transformToFollow;
    private Color _baseColor;

    void LateUpdate()
    {
        if (isInstalled)
        {
            if (transformToFollow != null && transformToFollow.gameObject.activeSelf)
            {
                transform.position = transformToFollow.position + new Vector3(0,-0.5f);
            }
        }
        if (Time.time - _lastUpdate > showDuration)
        {
            _isShowVisual = false;
            visualObject.SetActive(false);
        }
    }
    
    void FixedUpdate()
    {
        if (isInstalled)
        {
            UpdateSlowHealth();
        }
    }

    private void UpdateSlowHealth(){
            /*
            float alpha = Time.deltaTime / (_damping + Time.deltaTime);
            _currentSlowHealth = _currentSlowHealth + alpha * (_currentHealth - _currentSlowHealth);
            alpha = Time.deltaTime / (_damping/2 + Time.deltaTime);
            */
        if (isInstalled)
        {
            _lowPassHealth.Input(_currentHealth);
            float _currentSlowHealth = _lowPassHealth.Output();

            Vector2 slowHealthSize = slowHealthSprite.size;
            slowHealthSize.x = _currentSlowHealth / _maxHealth;
            slowHealthSprite.size = slowHealthSize;
        }
            /*
            var scaleBonus = (_currentSlowHealth - _currentHealth) / (_maxHealth * _lowPassHealth.GetAlpha());
            emptyHealthSprite.color = _baseColor + 5 * Color.white * scaleBonus;*/
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        _lastUpdate = Time.time;
        if (_maxHealth > 0)
        {
            Vector2 healthSize = healthSprite.size;
            healthSize.x = _currentHealth / _maxHealth;
            healthSprite.size = healthSize;
        }
    }
    public void SetCurrentHealth(float currentHealth)
    {
        _currentHealth = currentHealth;
        if (_maxHealth > 0)
        {
            Vector2 healthSize = healthSprite.size;
            healthSize.x = _currentHealth / _maxHealth;
            healthSprite.size = healthSize;
            _lastUpdate = Time.time;
        }
        if (!_isShowVisual)
        {
            _isShowVisual = true;
            visualObject.SetActive(true);
        }
    }
    public void Install(Transform _tfFollow, float maximumHealth, float currentHealth)
    {
        transformToFollow = _tfFollow;
        _maxHealth = maximumHealth;
        _currentHealth = currentHealth;
        _isShowVisual = false;
        _baseColor = emptyHealthSprite.color;
        _lowPassHealth = new LowPassFilter(_damping,_currentHealth);
        visualObject.SetActive(false);
        isInstalled = true;
    }
    public void Reset()
    {
        isInstalled = false;
    }
}
