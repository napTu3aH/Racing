﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingLevelLogics : MonoBehaviour
{
    [SerializeField] internal Indicator _IndicatorLogic;
    [SerializeField] internal UIPanel _UIPanel;
    [SerializeField] internal float _PercentLoaded;
    [SerializeField] internal bool _ActivateScene;
    bool _Loading;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _IndicatorLogic = GetComponent<Indicator>();
        _UIPanel = GetComponentInChildren<UIPanel>();
    }

    public void LoadingLevel(int _index)
    {
        if (!_Loading)
        {
            _Loading = true;
            StartCoroutine(Load(_index));
        }        
    }

    IEnumerator Load(int _index)
    {        
        AsyncOperation _async = SceneManager.LoadSceneAsync(_index);
        while (!_async.isDone)
        {
            _PercentLoaded = _async.progress;
            _async.allowSceneActivation = _ActivateScene;
            yield return null;
        }
        _Loading = false;
        _UIPanel.enabled = false;
        _UIPanel.enabled = true;
        _ActivateScene = false;
        _PercentLoaded = 1.0f;
        yield return null;
    }

}
