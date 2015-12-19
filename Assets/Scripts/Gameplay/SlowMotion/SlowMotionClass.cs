using UnityEngine;
using System.Collections;

public class SlowMotionClass : MonoBehaviour
{
    private static SlowMotionClass _SlowMotionClass;
    public static SlowMotionClass Instance
    {
        get
        {
            if (_SlowMotionClass != null)
            {
                return _SlowMotionClass;
            }
            else
            {
                _SlowMotionClass = new GameObject("_SlowMotionClass", typeof(SlowMotionClass)).GetComponent<SlowMotionClass>();
                _SlowMotionClass.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _SlowMotionClass;
            }
        }
    }
    [Range (0.0f, 1.0f)] public float _SlowValue = 0.1f;
    public float _SmoothRate = 0.05f;
    internal float _SlowingFactor = 1.0f;
    bool _Slowed, _SlowCoroutine;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!_SlowMotionClass)
        {
            _SlowMotionClass = this;
        }
    }

    /// <summary>
    /// Метод замедления времени.
    /// </summary>
    internal void SlowMotion(bool _Slow)
    {
        if (!_Slow)
        {
            _Slowed = !_Slowed;
            if (!_SlowCoroutine)
            {
                _SlowCoroutine = true;
                StartCoroutine(Slowing());
            }
        }
    }

    /// <summary>
    /// Метод изменения значения кадра.
    /// </summary>
    void FrameChangeValue()
    {
        Time.timeScale = _SlowingFactor;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        GameSettings.Instance._SoundSource.pitch = _SlowingFactor;
        GameSettings.Instance._MusicSource.pitch = Mathf.Clamp(_SlowingFactor + 0.4f, -1.0f, 1.0f);
    }

    /// <summary>
    /// Сопрограмма плавного замедления времени.
    /// </summary>
    IEnumerator Slowing()
    {

        while (_Slowed && _SlowingFactor != _SlowValue)
        {
            if (_SlowingFactor > _SlowValue)
            {
                _SlowingFactor -= _SmoothRate;
            }
            else
            if (_SlowingFactor < _SlowValue)
            {
                _SlowingFactor = _SlowValue;
            }
            FrameChangeValue();
            yield return null;
        }

        while (!_Slowed && _SlowingFactor != 1.0f)
        {
            if (_SlowingFactor < 1.0f)
            {
                _SlowingFactor += _SmoothRate;
            }
            else
            if (_SlowingFactor > 1.0f)
            {
                _SlowingFactor = 1.0f;
            }
            FrameChangeValue();
            yield return null;
        }

        _SlowCoroutine = false;
        yield return null;
    }
}
