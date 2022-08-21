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
    private string _labelText;
    private IGunController targetGun;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        targetGun = (target as IGunController);
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
}
