using UnityEngine;
using System.Collections;

public class Aeroplane : MonoBehaviour
{
    [SerializeField] internal GameObject _Plane;
    [SerializeField] internal Transform _StartPoint, _EndPoint;
    [SerializeField] internal Material _PlaneMaterial;
    [SerializeField] internal float _SpeedFlight, _SpeedColoring;
    [SerializeField] internal int _CountDropping, _OffsetDropping;

    int _CountDrop;
    Color _ColorAlpha, _ColorNormal;
    AudioSource _Source;
    float _Distance, _DistanceToDrop, _TimeFlight, _TimeChangeColor;
    bool _Alive = true, _Coloring, _Ended;

    void Start()
    {
        Init();
    }

    void Init()
    {
        _Plane = gameObject;
        transform.position = _StartPoint.position;
        transform.rotation = Quaternion.LookRotation(_EndPoint.position - transform.position);
        _Source = GetComponent<AudioSource>();
        _ColorAlpha = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _ColorNormal = Color.white;
        _PlaneMaterial.color = _ColorAlpha;

        SetDistance();
        StartCoroutine(Flight());    
    }

    void SetDistance()
    {
        _Distance = Vector3.Distance(_StartPoint.position, _EndPoint.position);
        _CountDrop = _CountDropping;
        int _OffSet = (_CountDropping % 2 == 0) ? 1 : 2;
        if (_CountDropping > 0) _DistanceToDrop = _Distance / (_CountDropping + _OffSet);
        StartCoroutine(Dropping());
    }

    void AudioSourceVolume()
    {
        float _volume;
        if (_TimeFlight < 0.5f)
        {
            _volume = _TimeFlight;
        }
        else
        {
            _volume = 1.0f - _TimeFlight;
        }
        _Source.volume = GameSettings.Instance._SoundSlider.value * (2 * _volume);
    }

    IEnumerator Colored(Color _colorFrom, Color _colorTo)
    {
        _TimeChangeColor = 0.0f;
        while (_TimeChangeColor < 1.0f)
        {
            _TimeChangeColor += Time.deltaTime * _SpeedColoring;
            _PlaneMaterial.color = Color.Lerp(_colorFrom, _colorTo, _TimeChangeColor);
            yield return null;
        }
        if (_Ended)
        {
            _Ended = false;
            _Plane.SetActive(false);
            _CountDropping = _CountDrop;
            _Source.volume = 0.0f;
        }
        yield return null;
    }

    IEnumerator Flight()
    {
        StartCoroutine(Colored(_ColorAlpha, _ColorNormal));
        while (_Alive)
        {
            _TimeFlight += Time.deltaTime * _SpeedFlight;
            if (_TimeFlight > 0.85f && !_Ended)
            {
                StartCoroutine(Colored(_ColorNormal, _ColorAlpha));
                _Ended = true;
            }
            if (_TimeFlight > 1.0f)
            {
                _Alive = false;
            }
            AudioSourceVolume();
            transform.position = Vector3.Lerp(_StartPoint.position, _EndPoint.position, _TimeFlight);
            yield return null;
        }
        yield return null;
    }

    IEnumerator Dropping()
    {
        while (_CountDropping > 0)
        {
            _Distance = Vector3.Distance(transform.position, _EndPoint.position);
            if (_Distance < _DistanceToDrop * _CountDropping)
            {
                _CountDropping--;
                DropBonuse();
            }
            yield return null;
        }
        yield return null;
    }

    void DropBonuse()
    {
        Debugger.Instance.Log("DROP!");
        BonusesLogic.Instance.SpawnBonus(transform, Vector3.zero);
    }
}
