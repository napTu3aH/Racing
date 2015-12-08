using UnityEngine;
using System.Collections;

public class LoadingLevelLogics : MonoBehaviour
{
    [SerializeField] internal Indicator _IndicatorLogic;
    [SerializeField] internal Canvas _Canvas;
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
        _Canvas = GetComponentInChildren<Canvas>();
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
        AsyncOperation _async = Application.LoadLevelAsync(_index);
        while (!_async.isDone)
        {
            _PercentLoaded = _async.progress;
            _async.allowSceneActivation = _ActivateScene;
            yield return null;
        }
        _Loading = false;
        _ActivateScene = false;
        _PercentLoaded = 1.0f;
        yield return null;
    }

}
