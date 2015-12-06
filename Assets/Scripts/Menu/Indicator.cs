using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] internal Image _Indicator;
    [SerializeField] internal int _LevelNumber;
    [SerializeField] internal bool _Clockwise, _DontWaiting;
    [SerializeField] internal float _SpeedRotate = 1.0f, _SpeedLerping = 1.0f, _PreloadingWaitingTime = 1.0f;
    [SerializeField] internal Color _ColorStart, _ColorEnd;

    bool _isDone, _isHided, _isStartedProcess, _StartRotate;
    float _TimeLerp;

    internal void Init()
    {
        _Indicator.color = _ColorStart;
        _isHided = System.Convert.ToBoolean(1 - _Indicator.color.a);
        StartCoroutine(ShowIndicator());  
    }

    IEnumerator ShowIndicator()
    {
        if (_DontWaiting)
        {
            ProcessingStart();
        }
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

    internal void ProcessingStart()
    {
        LoadingLevelLogics.Instance.LoadingLevel(_LevelNumber);
        StartCoroutine(Progressing());
        _isStartedProcess = true;
    }

    void ColorChanger(Color _start, Color _end, bool _hiding)
    {
        _TimeLerp += Time.deltaTime * _SpeedLerping;
        _Indicator.color = Color.Lerp(_start, _end, _TimeLerp);
        if (_TimeLerp > 1.0f)
        {
            _TimeLerp = 0.0f;
            _isHided = _hiding;
        }
    }

    IEnumerator HideIndicator()
    {
        if (_Type == TypeIndicator.None) _PreloadingWaitingTime = 0.0f;
        yield return new WaitForSeconds(_PreloadingWaitingTime);

        while (!_isHided)
        {
            if (_Indicator) ColorChanger(_ColorEnd, _ColorStart, true);
            yield return null;
        }
        LoadingLevelLogics.Instance._ActivateScene = true;
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
        while (_Indicator && LoadingLevelLogics.Instance._PercentLoaded != 1.0f)
        {
            if (_Clockwise) _Indicator.rectTransform.Rotate(0.0f, 0.0f, 1.0f * _SpeedRotate);
            else _Indicator.rectTransform.Rotate(0.0f, 0.0f, -1.0f * _SpeedRotate);            
            yield return null;
        }
        yield return null;
    }

    void ProgressBar()
    {
        ProcessingLoading();
    }

    void ProcessingLoading()
    {
        float _fillValue = LoadingLevelLogics.Instance._PercentLoaded;
        if(_Type == TypeIndicator.ProgressBar) _Indicator.fillAmount = _fillValue;
        if (_fillValue == 0.9f)
        {
            _isDone = true;
            StartCoroutine(HideIndicator());
        }
    }
}
