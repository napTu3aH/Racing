using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashLoading : MonoBehaviour
{
    [SerializeField] internal ImageComponent[] _Images;
    [SerializeField] internal bool _isDone;
    Color _ColorStart, _ColorEnd;
    bool _SettedColor;
    float _TimeLerp;
    int _Index;

    void Start()
    {
        Init();
    }

    void Init()
    {
        _isDone = false;
        ColorInited();
        ColoringImages();
    }

    void ColorInited()
    {
        if (_Images.Length > 0)
        {
            for (int i = 0; i < _Images.Length; i++)
            {
                _Images[i]._Image.color = _Images[i]._ColorStart;
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
            _isDone = true;
            LoadingLevelLogics.Instance._IndicatorLogic.Init();
        }
    }

    IEnumerator _ColorChanger(Image _Image, float _seconds, float _speed)
    {
        while (!_SettedColor)
        {
            _TimeLerp += Time.deltaTime * _speed;
            _Image.color = Color.Lerp(_ColorStart, _ColorEnd, _TimeLerp);
            if (_TimeLerp >= 1.0f)
            {
                _TimeLerp = 0.0f;
                _SettedColor = true;
                yield return null;
            }
            yield return null;
        }
        yield return new WaitForSeconds(_seconds);
        ColoringImages();
    }
}

[Serializable]
public class ImageComponent
{
    [SerializeField] internal string _Name;
    [SerializeField] internal Image _Image;
    [SerializeField] internal Color _ColorStart, _ColorEnd;
    [SerializeField] internal float _TimeToLerpColor = 1.0f;
    [SerializeField] internal float _TimeWaiting = 1.0f;
}
