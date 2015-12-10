using UnityEngine;
using System.Collections;

public class GameplayInfo : MonoBehaviour {

    public static GameplayInfo Inscante;

    [Header("Points")]
    [SerializeField] internal float _LastPoints;
    [SerializeField] internal float _SpeedLerpPoints = 1.0f;
    [SerializeField] internal UILabel _ScoreText;

    [Header("Kills")]
    [SerializeField] internal int _CountKill;
    [SerializeField] internal UILabel _KillText;
    bool _Counting;
    float _TimeLerp;
    internal float _PointsNow;
    
    void Awake()
    {
        Init();
    }

    void Init()
    {
        Inscante = this;
        _KillText.text = "Kills: " + _CountKill;
        _ScoreText.text = "Points: " + _LastPoints.ToString("0");
    }

    public void Kills()
    {
        _CountKill++;
        _KillText.text = "Kills: " + _CountKill;
    }

    public void StartCounting(float _points)
    {
        _PointsNow += _points;
        if (!_Counting)
        {
            _Counting = true;
            StartCoroutine(Counting());
        }
    }


    IEnumerator Counting()
    {
        while (_Counting)
        {
            _TimeLerp += _SpeedLerpPoints * Time.deltaTime;
            if (_TimeLerp < 1.0f)
            {
                _LastPoints = Mathf.Lerp(_LastPoints, _PointsNow, _TimeLerp);
            }
            else
            if (_TimeLerp > 1.0f)
            {
                _TimeLerp = 0.0f;
                _LastPoints = _PointsNow;
                _Counting = false;
                yield return null;
            }
            _ScoreText.text = "Points: " + _LastPoints.ToString("0");
            yield return null;
        }
        yield return null;
    }
}
