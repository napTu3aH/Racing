using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageLoading : MonoBehaviour
{
    [SerializeField] internal ImageComponent[] _Images;
    [SerializeField] internal bool _isDone;
    Color _ColorStart, _ColorEnd, _ColorAlpha;
    bool _SettedColor, _AlphaColor;
    float _TimeLerp;
    int _Index;

    internal void Init()
    {
        _isDone = false;
        _Index = 0;
        ColorInited();
        ColoringImages();
    }

    void ColorInited()
    {
        if (_Images.Length > 0)
        {
            for (int i = 0; i < _Images.Length; i++)
            {
                _Images[i]._CurrentColor = _Images[i]._ColorStart;
            }
        }
    }

    void ColoringImages()
    {
        if (_Images.Length > 0 && _Index < _Images.Length)
        {
            for (int i = _Index; i < _Images.Length; i++)
            {
                if (_Images[i]._Image.color != _Images[i]._ColorEnd)
                {
                    _Index++;
                    _SettedColor = false;
                    _ColorStart = _Images[i]._ColorStart;
                    _ColorEnd = _Images[i]._ColorEnd;
                    StartCoroutine(_ColorChanger(_Images[i]._Image, _Images[i]._TimeWaiting, _Images[i]._TimeToLerpColor));
                    break;
                }
            }
        }
        else
        {
            if (!_isDone)
            {
                _isDone = true;
                if (Application.loadedLevel == 0)
                    LoadingLevel.Instance._LoadingLevelLogics._IndicatorLogic.Init(1, false);
            }
        }
    }

    internal void ColorAlpha()
    {
        _AlphaColor = true;
        _SettedColor = false;
        StartCoroutine(_ColorChanger(_Images[_Index - 1]._Image, 1.0f, _Images[_Index - 1]._TimeToLerpColor));
    }

    IEnumerator _ColorChanger(UISprite _Image, float _seconds, float _speed)
    {
        float _time = 0.0f;

        while (!_SettedColor)
        {
            _TimeLerp += RealTime.deltaTime * _speed;
            if(!_AlphaColor) _Image.color = Color.Lerp(_ColorStart, _ColorEnd, _TimeLerp);
            else _Image.color = Color.Lerp(_ColorEnd, _ColorAlpha, _TimeLerp);
            if (_TimeLerp >= 1.0f)
            {
                _TimeLerp = 0.0f;
                _SettedColor = true;
                _AlphaColor = false;
                yield return null;
            }
            yield return null;
        }
        while (_time <= _seconds)
        {
            _time += RealTime.deltaTime;
            yield return null;
        }        
        ColoringImages();
    }
}

[Serializable]
public class ImageComponent
{
    [SerializeField] internal string _Name;
    [SerializeField] internal UISprite _Image;
    [SerializeField] internal Color _ColorStart, _CurrentColor, _ColorEnd;
    [SerializeField] internal float _TimeToLerpColor = 1.0f;
    [SerializeField] internal float _TimeWaiting = 1.0f;
}
