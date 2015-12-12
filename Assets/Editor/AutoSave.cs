using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

/// <summary>
/// Скрипт автоматического сохранения сцены и проекта.
/// </summary>
public class AutoSave : EditorWindow
{

    bool _AutoSaveScene = true, _ShowMessage = true, _isStarted = false;
    int _IntervalScene;
    DateTime _LastSaveTimeScene = DateTime.Now;
    string _ProjectPath = Application.dataPath, _ScenePath;

    [MenuItem("Window/AutoSave")]
    static void Init()
    {
        AutoSave _SaveWindow = (AutoSave)EditorWindow.GetWindow(typeof(AutoSave));
        _SaveWindow.Show();
    }

    /// <summary>
    /// Отрисовка окна.
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("Info:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Saving to:", "" + _ProjectPath);
        EditorGUILayout.LabelField("Saving scene:", "" + _ScenePath);
        GUILayout.Label("Options:", EditorStyles.boldLabel);
        _AutoSaveScene = EditorGUILayout.BeginToggleGroup("Auto save", _AutoSaveScene);
        _IntervalScene = EditorGUILayout.IntSlider("Interval (minutes)", _IntervalScene, 1, 30);
        if (_isStarted)
        {
            EditorGUILayout.LabelField("Last save:", "" + _LastSaveTimeScene);
        }
        EditorGUILayout.EndToggleGroup();
        _ShowMessage = EditorGUILayout.BeginToggleGroup("Show Message", _ShowMessage);
        EditorGUILayout.EndToggleGroup();
    }


    void Update()
    {
        _ScenePath = EditorApplication.currentScene;
        if (_AutoSaveScene && !Application.isPlaying)
        {
            if (DateTime.Now.Minute >= (_LastSaveTimeScene.Minute + _IntervalScene) || DateTime.Now.Minute == 59 && DateTime.Now.Second == 59)
            {
                SaveScene();
            }
        }
        else
        {
            _isStarted = false;
        }

    }
    /// <summary>
    /// Метод сохранения.
    /// </summary>
    void SaveScene()
    {
        EditorApplication.SaveScene(_ScenePath);
        _LastSaveTimeScene = DateTime.Now;
        _isStarted = true;
        if (_ShowMessage)
        {
            Debug.Log("AutoSave saved: " + _ScenePath + " on " + _LastSaveTimeScene);
        }
        AutoSave repaintSaveWindow = (AutoSave)EditorWindow.GetWindow(typeof(AutoSave));
        repaintSaveWindow.Repaint();
    }
}