using UnityEngine;
using System.Collections;

public class Indicator : MonoBehaviour
{
    internal enum TypeIndicator
    {
        None,
        Circle,
        ProgressBar
    }
    
    [SerializeField] internal TypeIndicator _Type;
    [SerializeField] internal UISprite _Indicator;
    [SerializeField] internal TweenTransform _TweenTransform;
    [SerializeField] internal int _LevelNumber;
    [SerializeField] internal bool _Clockwise, _DontWaiting, _ShowBackground;
    [SerializeField] internal float _SpeedRotate = 1.0f, _SpeedLerping = 1.0f, _PreloadingWaitingTime = 1.0f;
    [SerializeField] internal Color _ColorStart, _ColorEnd;

    bool _isDone, _isHided, _isStartedProcess, _StartRotate, _isLoading;
    float _TimeLerp;

    internal void Init(int _lvl, bool _background)
    {
        if(!_isLoading)
        {
            _LevelNumber = _lvl;
            _ShowBackground = _background;
            _Indicator.color = _ColorStart;
            if (!_TweenTransform) _TweenTransform = _Indicator.GetComponent<TweenTransform>();
            _isHided = System.Convert.ToBoolean(1 - _Indicator.color.a);

            if (_ShowBackground)
            {
                LoadingLevel.Instance._ImageLoading.Init();
            }
            StartCoroutine(ShowIndicator());
            _isLoading = true;
        }        
    }

    IEnumerator ShowIndicator()
    {
        if (_DontWaiting)
        {
            ProcessingStart();
        }
        _TweenTransform.PlayForward();
        while (_isHided && _Type != TypeIndicator.None)
        {
            if (_Indicator) ColorChanger(_ColorStart, _ColorEnd, false);
            yield return null;
        }

        if (!_isStartedProcess)
        {
            ProcessingStart();
        }        
        yield return null;
        
    }

    void ProcessingStart()
    {
        LoadingLevel.Instance._LoadingLevelLogics.LoadingLevel(_LevelNumber);
        StartCoroutine(Progressing());
        _isStartedProcess = true;
    }

    void ColorChanger(Color _start, Color _end, bool _hiding)
    {
        _TimeLerp += RealTime.deltaTime * _SpeedLerping;
        _Indicator.color = Color.Lerp(_start, _end, _TimeLerp);
        if (_TimeLerp > 1.0f)
        {
            _TimeLerp = 0.0f;
            _isHided = _hiding;
        }
    }

    IEnumerator HideIndicator()
    {
        float _time = 0.0f;
        if (_Type != TypeIndicator.None)
        {
            while (_time <= _PreloadingWaitingTime)
            {
                _time += RealTime.deltaTime;
                yield return null;
            }            
        }

        bool _back = false;
        while (!_isHided)
        {
            if (_Indicator) ColorChanger(_ColorEnd, _ColorStart, true);
            if (!_back)
            {
                _back = true;
                _TweenTransform.PlayReverse();
            } 
            yield return null;
        }
        LoadingLevel.Instance._LoadingLevelLogics._ActivateScene = true;

        _time = 0.0f;
        while (_time <= 0.5f)
        {
            _time += RealTime.deltaTime;
            yield return null;
        }

        if (_ShowBackground) LoadingLevel.Instance._ImageLoading.ColorAlpha();
        _isDone = false;
        _isLoading = false;        
        yield return null;
    }


    IEnumerator Progressing()
    {
        while (!_isDone)
        {
            switch (_Type)
            {
                case TypeIndicator.None:
                    ProcessingLoading();
                    break;

                case TypeIndicator.Circle:
                    Circle();
                    break;

                case TypeIndicator.ProgressBar:
                    ProgressBar();
                    break;
            }
            yield return null;
        }
        yield return null;
    }

    void Circle()
    {
        if (!_StartRotate)
        {
            _StartRotate = true;
            StartCoroutine(RotateCircle());
        }
        ProcessingLoading();
    }

    IEnumerator RotateCircle()
    {
        while (_Indicator && _isLoading)
        {
            if (_Clockwise) _Indicator.transform.Rotate(0.0f, 0.0f, 1.0f * _SpeedRotate);
            else _Indicator.transform.Rotate(0.0f, 0.0f, -1.0f * _SpeedRotate);
            yield return null;
        }
        _StartRotate = false;
        yield return null;
    }

    void ProgressBar()
    {
        ProcessingLoading();
    }

    void ProcessingLoading()
    {
        float _fillValue = LoadingLevel.Instance._LoadingLevelLogics._PercentLoaded;
        if(_Type == TypeIndicator.ProgressBar) _Indicator.fillAmount = _fillValue;
        if (_fillValue == 0.9f)
        {
            _isDone = true;
            StartCoroutine(HideIndicator());
        }
    }
}
