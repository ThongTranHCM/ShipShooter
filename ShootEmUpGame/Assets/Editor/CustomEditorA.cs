using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IGunController))]
public class CustomEditorA : Editor
{
    private bool _isChecked;
    private List<BulletParticleSystem> _newParticles;
    private List<BulletParticleSystem> _existParticles;
    SerializedProperty m_BulletParticleSystemsProperty;
    SerializedProperty m_BulletControllerProperty;
    SerializedProperty m_shootTransformsProperty;
    SerializedProperty m_orientateProperty;
    SerializedProperty m_orientateOncePerWaveProperty;
    SerializedProperty m_orientateOncePerShotProperty;
    SerializedProperty m_bulletPrefabProperty;
    SerializedProperty m_resetOnPlay;
    private string _labelText;
    private bool showOrientate;
    private IGunController targetGun;
    string[] _options = new string[3] { "Random", "Fixed", "Default" };

    public void OnEnable()
    {
        m_BulletParticleSystemsProperty = serializedObject.FindProperty("bulletParticleSystems");
        m_BulletControllerProperty = serializedObject.FindProperty("bulletControllers");
        m_shootTransformsProperty = serializedObject.FindProperty("shootTransforms");
        m_orientateProperty = serializedObject.FindProperty("_isOrientate");
        m_orientateOncePerWaveProperty = serializedObject.FindProperty("_isOrientateOncePerWave");
        m_orientateOncePerShotProperty = serializedObject.FindProperty("_isOrientateOncePerShot");
        m_resetOnPlay = serializedObject.FindProperty("_resetOnPlay");
        m_bulletPrefabProperty = serializedObject.FindProperty("_bulletPrefab");
        IGunController xGun = (target as IGunController);
        showOrientate = false;
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        targetGun = (target as IGunController);
        EditorGUILayout.PropertyField(m_BulletParticleSystemsProperty);
        EditorGUILayout.PropertyField(m_BulletControllerProperty);
        EditorGUILayout.PropertyField(m_shootTransformsProperty);
        //---------------------------------------

        Rect r = EditorGUILayout.BeginHorizontal("Button", GUILayout.MinHeight(45));
        GUILayout.Label("Gun Pos");
        Rect line = new Rect(r.x, r.y + 20, r.width, 20);
        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(line.x, line.y, 20, 20), "X");

        int _selectedX = targetGun.isRandomPositionX ? 0 : (targetGun.isFixedPositionX ? 1 : 2);
        int _selectedY = targetGun.isRandomPositionY ? 0 : (targetGun.isFixedPositionY ? 1 : 2);
        if (_selectedX == 1)
        {
            _selectedX = EditorGUI.Popup(new Rect(line.x + 60, line.y, (line.width / 2 - 110), 20), _selectedX, _options);
            targetGun.fixedX = EditorGUI.FloatField(new Rect(line.x + line.width / 2 - 40, line.y, 40, 20), targetGun.fixedX);
        }
        else
        {
            _selectedX = EditorGUI.Popup(new Rect(line.x + 60, line.y, (line.width / 2 - 70), 20), _selectedX, _options);
        }

        if (EditorGUI.EndChangeCheck())
        {
            switch (_selectedX)
            {
                case 0:
                    targetGun.isRandomPositionX = true;
                    targetGun.isFixedPositionX = false;
                    break;
                case 1:
                    targetGun.isRandomPositionX = false;
                    targetGun.isFixedPositionX = true;
                    break;
                case 2:
                    targetGun.isRandomPositionX = false;
                    targetGun.isFixedPositionX = false;
                    break;
            }
        }
        //---------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(new Rect(line.x + (line.width / 2), line.y, 20, 20), "Y");
        if (_selectedY == 1)
        {
            _selectedY = EditorGUI.Popup(new Rect(line.x + (line.width / 2) + 60, line.y, (line.width / 2 - 110), 20), _selectedY, _options);
            targetGun.fixedY = EditorGUI.FloatField(new Rect(line.x + line.width - 40, line.y, 40, 20), targetGun.fixedY);
        }
        else
        {
            _selectedY = EditorGUI.Popup(new Rect(line.x + (line.width / 2) + 60, line.y, (line.width / 2 - 70), 20), _selectedY, _options);
        }

        if (EditorGUI.EndChangeCheck())
        {
            switch (_selectedY)
            {
                case 0:
                    targetGun.isRandomPositionY = true;
                    targetGun.isFixedPositionY = false;
                    break;
                case 1:
                    targetGun.isRandomPositionY = false;
                    targetGun.isFixedPositionY = true;
                    break;
                case 2:
                    targetGun.isRandomPositionY = false;
                    targetGun.isFixedPositionY = false;
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();
        /// ---------------------
        targetGun.stopGunAtInstall = EditorGUILayout.Toggle("Stop Gun At Install ", targetGun.stopGunAtInstall);
        Vector2 size = new Vector2(targetGun.sizeX, targetGun.sizeY);
        size = EditorGUILayout.Vector2Field("Size ", size);
        targetGun.sizeX = size.x;
        targetGun.sizeY = size.y;
        showOrientate = EditorGUILayout.BeginFoldoutHeaderGroup(showOrientate, "Orientate");
        if (showOrientate) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_orientateProperty);
            EditorGUILayout.PropertyField(m_orientateOncePerWaveProperty);
            EditorGUILayout.PropertyField(m_orientateOncePerShotProperty);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.PropertyField(m_bulletPrefabProperty);
        EditorGUILayout.PropertyField(m_resetOnPlay);

        if (GUILayout.Button("Check Bullet Particle Systems"))
        {
            CheckingParticles();
        }
        if (_isChecked)
        {
            GUIStyle textStyle = EditorStyles.label;
            textStyle.wordWrap = true;
            EditorGUILayout.LabelField(_labelText);
            if (_newParticles.Count > 0)
            {
                if (GUILayout.Button("Add missing Bullet Particle Systems"))
                {
                    for (int i = 0; i < _newParticles.Count; i++)
                    {
                        _existParticles.AddRange(_newParticles);
                        targetGun.bulletParticleSystems = _existParticles.ToArray();
                    }
                    _isChecked = false;
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void CheckingParticles()
    {
        _isChecked = true;
        _existParticles = new List<BulletParticleSystem>();
        _newParticles = new List<BulletParticleSystem>();
        for (int i = 0; i < targetGun.bulletParticleSystems.Length; i++)
        {
            _existParticles.Add(targetGun.bulletParticleSystems[i]);
        }
        targetGun.GetComponentsInChildren<BulletParticleSystem>(_newParticles);
        for (int i = 0; i < _newParticles.Count;)
        {
            if (_existParticles.Contains(_newParticles[i]))
            {
                _newParticles.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        if (_newParticles.Count > 0)
        {
            _labelText = "Adding: \n";
            for (int i = 0; i < _newParticles.Count - 1; i++)
            {
                _labelText += _newParticles[i].gameObject.name + ", ";
            }
            _labelText += _newParticles[_newParticles.Count - 1].gameObject.name + ".\n";
        }
        else
        {
            _labelText = "Nothing to update";
        }
    }

    public static void OnGUIGun(IGunController targetGun, SerializedObject serializedGun)
    {
        SerializedProperty s_BulletParticleSystemsProperty = serializedGun.FindProperty("bulletParticleSystems");
        SerializedProperty s_BulletControllerProperty = serializedGun.FindProperty("bulletControllers");
        SerializedProperty s_shootTransformsProperty = serializedGun.FindProperty("shootTransforms");
        SerializedProperty s_orientateProperty = serializedGun.FindProperty("_isOrientate");
        SerializedProperty s_orientateOncePerWaveProperty = serializedGun.FindProperty("_isOrientateOncePerWave");
        SerializedProperty s_orientateOncePerShotProperty = serializedGun.FindProperty("_isOrientateOncePerShot");
        SerializedProperty s_resetOnPlay = serializedGun.FindProperty("_resetOnPlay");
        SerializedProperty s_bulletPrefabProperty = serializedGun.FindProperty("_bulletPrefab");
        EditorGUILayout.PropertyField(s_BulletParticleSystemsProperty);
        EditorGUILayout.PropertyField(s_BulletControllerProperty);
        EditorGUILayout.PropertyField(s_shootTransformsProperty);
        string[] _options = new string[3] { "Random", "Fixed", "Default" };
        int _selectedX = targetGun.isRandomPositionX ? 0 : (targetGun.isFixedPositionX ? 1 : 2);
        int _selectedY = targetGun.isRandomPositionY ? 0 : (targetGun.isFixedPositionY ? 1 : 2);
        //---------------------------------------

        Rect r = EditorGUILayout.BeginHorizontal("Button", GUILayout.MinHeight(45));
        GUILayout.Label("Gun Pos");
        Rect line = new Rect(r.x, r.y + 20, r.width, 20);
        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(new Rect(line.x, line.y, 20, 20), "X");
        if (_selectedX == 1)
        {
            _selectedX = EditorGUI.Popup(new Rect(line.x + 60, line.y, (line.width / 2 - 110), 20), _selectedX, _options);
            targetGun.fixedX = EditorGUI.FloatField(new Rect(line.x + line.width / 2 - 40, line.y, 40, 20), targetGun.fixedX);
        }
        else
        {
            _selectedX = EditorGUI.Popup(new Rect(line.x + 60, line.y, (line.width / 2 - 70), 20), _selectedX, _options);
        }

        if (EditorGUI.EndChangeCheck())
        {
            switch (_selectedX)
            {
                case 0:
                    targetGun.isRandomPositionX = true;
                    targetGun.isFixedPositionX = false;
                    break;
                case 1:
                    targetGun.isRandomPositionX = false;
                    targetGun.isFixedPositionX = true;
                    break;
                case 2:
                    targetGun.isRandomPositionX = false;
                    targetGun.isFixedPositionX = false;
                    break;
            }
        }
        //---------------------------------------
        EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(new Rect(line.x + (line.width / 2), line.y, 20, 20), "Y");
        if (_selectedY == 1)
        {
            _selectedY = EditorGUI.Popup(new Rect(line.x + (line.width / 2) + 60, line.y, (line.width / 2 - 110), 20), _selectedY, _options);
            targetGun.fixedY = EditorGUI.FloatField(new Rect(line.x + line.width - 40, line.y, 40, 20), targetGun.fixedY);
        }
        else
        {
            _selectedY = EditorGUI.Popup(new Rect(line.x + (line.width / 2) + 60, line.y, (line.width / 2 - 70), 20), _selectedY, _options);
        }

        if (EditorGUI.EndChangeCheck())
        {
            switch (_selectedY)
            {
                case 0:
                    targetGun.isRandomPositionY = true;
                    targetGun.isFixedPositionY = false;
                    break;
                case 1:
                    targetGun.isRandomPositionY = false;
                    targetGun.isFixedPositionY = true;
                    break;
                case 2:
                    targetGun.isRandomPositionY = false;
                    targetGun.isFixedPositionY = false;
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();
        /// ---------------------
        targetGun.stopGunAtInstall = EditorGUILayout.Toggle("Stop Gun At Install ", targetGun.stopGunAtInstall);
        Vector2 size = new Vector2(targetGun.sizeX, targetGun.sizeY);
        size = EditorGUILayout.Vector2Field("Size ", size);
        targetGun.sizeX = size.x;
        targetGun.sizeY = size.y;

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(s_orientateProperty);
        EditorGUILayout.PropertyField(s_orientateOncePerWaveProperty);
        EditorGUILayout.PropertyField(s_orientateOncePerShotProperty);
        EditorGUI.indentLevel--;
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.PropertyField(s_bulletPrefabProperty);
        EditorGUILayout.PropertyField(s_resetOnPlay);
    }
}

[CustomEditor(typeof(LaserController))]
public class CustomLaserEditor : Editor
{
    SerializedProperty m_laserSpriteProperty;
    SerializedProperty m_laserVisualProperty;
    SerializedProperty m_laserSizeProperty;

    public void OnEnable()
    {
        m_laserSpriteProperty = serializedObject.FindProperty("laserSprite");
        m_laserVisualProperty = serializedObject.FindProperty("laserVisual");
        m_laserSizeProperty = serializedObject.FindProperty("size");
    }
    public override void OnInspectorGUI()
    {
        IGunController targetGun = (target as IGunController);
        CustomEditorA.OnGUIGun(targetGun, serializedObject);
        EditorGUILayout.PropertyField(m_laserSpriteProperty);
        EditorGUILayout.PropertyField(m_laserVisualProperty);
        EditorGUILayout.PropertyField(m_laserSizeProperty);
    }
}
[CustomEditor(typeof(StaticChargeGunController))]
public class CustomStaticGunEditor : Editor
{
    SerializedProperty m_tfVisualProperty;
    SerializedProperty m_findTargetProperty;
    SerializedProperty m_chargeTimeProperty;
    SerializedProperty m_showTimeProperty;
    SerializedProperty m_distanceProperty;

    public void OnEnable()
    {
        m_tfVisualProperty = serializedObject.FindProperty("_tfVisual");
        m_findTargetProperty = serializedObject.FindProperty("_findTarget");
        m_chargeTimeProperty = serializedObject.FindProperty("_chargeTime");
        m_showTimeProperty = serializedObject.FindProperty("_showTime");
        m_distanceProperty = serializedObject.FindProperty("_distance");
    }
    public override void OnInspectorGUI()
    {
        IGunController targetGun = (target as IGunController);
        CustomEditorA.OnGUIGun(targetGun, serializedObject);
        EditorGUILayout.PropertyField(m_tfVisualProperty);
        EditorGUILayout.PropertyField(m_findTargetProperty);
        EditorGUILayout.PropertyField(m_chargeTimeProperty);
        EditorGUILayout.PropertyField(m_showTimeProperty);
        EditorGUILayout.PropertyField(m_distanceProperty);
    }
}