using UnityEngine;
using System.Collections;

public class GameplayInfo : MonoBehaviour {

    public static GameplayInfo Inscante;

    public float _AllPoints
    {
        get { return _allPoints; }
    }

    public float _GameplayTime
    {
        get { return _GameplayingTime; }
    }
    public int _CountKill
    {
        set { _CountingKill = value; }
        get { return _CountingKill; }
    }

    public float _Points
    {
        set { _LastPoints = value; }
        get { return _LastPoints; }
    }

    public float _PointsNow;

    int _CountingKill;
    float _GameplayingTime, _allPoints;

    public float _LastPoints, _PointsTimeLerp;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Inscante = this;
    }

    public void Kills()
    {
        _CountingKill++;
    }

    void TimeGameplay()
    {
        _GameplayingTime += Time.deltaTime;

        if (_PointsTimeLerp < 1.0f)
        {
            _PointsTimeLerp += Time.deltaTime * 0.1f;
        }
        else
        {
            _PointsTimeLerp = 0.0f;
        }
        
    }

    void PointsCounting()
    {
        if (_LastPoints < _PointsNow)
        {
            _LastPoints = Mathf.Lerp(_LastPoints, _PointsNow, _PointsTimeLerp);
        }
        else
        if(_LastPoints > _PointsNow)
        {
            _LastPoints = _PointsNow;
        }

        TimeGameplay();
    }

    /*void Update()
    {
        TimeGameplay();
        PointsCounting();
    }*/
}
