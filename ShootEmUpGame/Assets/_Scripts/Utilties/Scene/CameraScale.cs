using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScale : MonoBehaviour
{
    [Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode.")]
    [SerializeField] protected Vector2 m_ReferenceResolution = new Vector2(8, 6);
    [Range(0, 1)]
    [SerializeField] private float _widthOrHeight;
    [SerializeField]
    private Camera _camera;

    public void OnValidate()
    {
        if (_camera == null) _camera = GetComponent<Camera>();
    }
    public void MatchSize()
    {
        float unitsPerPixel = m_ReferenceResolution[0] / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        unitsPerPixel = m_ReferenceResolution[1] / Screen.height;
        float desiredHalfWidth = 0.5f * unitsPerPixel * Screen.width;
        float logWeightedAverage = Mathf.Lerp(desiredHalfHeight, desiredHalfWidth, _widthOrHeight);
        _camera.orthographicSize = logWeightedAverage;
        Constants.CalculateSizeCamera();
    }

    public void Awake()
    {
        MatchSize();
    }
}
