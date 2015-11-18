using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

[CustomEditor(typeof(CarInfo))]
[CanEditMultipleObjects]
public class CarInfoEditor : Editor
{
    MonoScript _Script;
    SerializedProperty _Player;
    SerializedProperty _TopSpeed;
    SerializedProperty _HitBoxParent, _HitBoxs;

    public void OnEnable()
    {
        _Script = MonoScript.FromMonoBehaviour((CarInfo)target);

        _Player = serializedObject.FindProperty("_Player");
        _TopSpeed = serializedObject.FindProperty("_TopSpeed");
        _HitBoxParent = serializedObject.FindProperty("_HitBoxParent");
        _HitBoxs = serializedObject.FindProperty("_HitBoxs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CarInfo _CarInfo = (CarInfo)target;        
        _Script = EditorGUILayout.ObjectField("Script:", _Script, typeof(MonoScript), false) as MonoScript;
        EditorGUILayout.Space();
        ProgressBar(_CarInfo._Health / 100.0f, "Health: ");
        EditorGUILayout.Space();
        _CarInfo._isAlive = EditorGUILayout.Toggle("Is Alive", _CarInfo._isAlive);
        _CarInfo._CurrentHealth = EditorGUILayout.FloatField("Current Health", _CarInfo._CurrentHealth);
        _CarInfo._PercentHealthFactor = EditorGUILayout.FloatField("Percent Health Factor", _CarInfo._PercentHealthFactor);
        EditorGUILayout.Space();
        _TopSpeed.floatValue = EditorGUILayout.FloatField("Top Speed", _TopSpeed.floatValue);
        EditorGUILayout.Space();
        _Player.boolValue = EditorGUILayout.Toggle("Player", _Player.boolValue);
        if (!_Player.boolValue)
        {
            _CarInfo._ID = EditorGUILayout.IntField("ID NPC", _CarInfo._ID);
            _CarInfo._TimeUpdateFactor = EditorGUILayout.FloatField("Time Update Factor", _CarInfo._TimeUpdateFactor);
            _CarInfo._TimeUpdate = EditorGUILayout.FloatField("Time Update", _CarInfo._TimeUpdate);
        }
        else
        {
            _CarInfo._TimeUpdateFactor = 0.0f;
        }
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_HitBoxParent);
        EditorGUILayout.PropertyField(_HitBoxs, true);      
        serializedObject.ApplyModifiedProperties();
    }

    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
