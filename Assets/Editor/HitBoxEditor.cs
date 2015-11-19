using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HitBox))]
[CanEditMultipleObjects]
public class HitBoxEditor : Editor
{
    MonoScript _Script;
    SerializedProperty _CarInfo;
    SerializedProperty _ArmorFactor, _HitBoxHealth, _UnDamagedFactor, _HealthFactor;
    SerializedProperty _Colors, _CurrentColour;


    public void OnEnable()
    {
        _Script = MonoScript.FromMonoBehaviour((HitBox)target);

        _CarInfo = serializedObject.FindProperty("_CarInfo");
        _ArmorFactor = serializedObject.FindProperty("_ArmorFactor");
        _HitBoxHealth = serializedObject.FindProperty("_HitBoxHealth");
        _UnDamagedFactor = serializedObject.FindProperty("_UnDamagedFactor");
        _HealthFactor = serializedObject.FindProperty("_HealthFactor");
        _Colors = serializedObject.FindProperty("_Colors");
        _CurrentColour = serializedObject.FindProperty("_CurrentColour");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); 
        HitBox _HitBox = (HitBox)target;
        _Script = EditorGUILayout.ObjectField("Script:", _Script, typeof(MonoScript), false) as MonoScript;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_CarInfo);
        EditorGUILayout.Space();
        ProgressBar(_HitBoxHealth.floatValue / _HealthFactor.floatValue, "Health for HitBox: ");
        _HitBoxHealth.floatValue = EditorGUILayout.FloatField("Health", _HitBoxHealth.floatValue);
        _ArmorFactor.floatValue = EditorGUILayout.FloatField("Armor Factor", _ArmorFactor.floatValue);
        _UnDamagedFactor.floatValue = EditorGUILayout.FloatField("Undamaged Factor", _UnDamagedFactor.floatValue);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_Colors, true);
        EditorGUILayout.ColorField("Current Color",_CurrentColour.colorValue);
        serializedObject.ApplyModifiedProperties();
    }

    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label+value.ToString());
        EditorGUILayout.Space();
    }
}
