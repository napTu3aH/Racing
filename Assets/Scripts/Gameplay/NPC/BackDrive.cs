using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class BackDrive : MonoBehaviour {
	
	CarController _CarControl;
	CarAIControl _AILogic;
	
	public float _StayTimer;

    float _StartSpeed;
    float _TimeForСheck;
    float _DistanceFromStart;

    Vector3 _StartPointBrake;

	bool _HittedToWall, _AIStateChange, _Coroutined, _Counting;

	void Awake () 
	{
        Init();
	}

    void Init()
    {
        _CarControl = GetComponent<CarController>();
        _AILogic = GetComponent<CarAIControl>();
        _StartSpeed = _CarControl._FullTorqueOverAllWheels;
        _TimeForСheck = Random.Range(1.5f, 1.6f);
    }


    /// <summary>
    /// Метод запуска движения NPC назад при длительном столкновении.
    /// </summary>
    public void HittedToWall()
    {
        _HittedToWall = true;
        _StayTimer = 0.0f;
        _CarControl._FullTorqueOverAllWheels = -5000f;
        _DistanceFromStart = 0.0f;
        if (!_Coroutined)
        {
            _Coroutined = true;
            StartCoroutine(ResetSpeed());
        }
        
    }

    /// <summary>
    /// Метод запуска подсчёта времени застоя при столкновении.
    /// </summary>
    public void Staying()
    {
        if (!_Counting)
        {
            _Counting = true;
            StartCoroutine(TimerStayCounting());
        }
    }

    /// <summary>
    /// Метод остановки подсчёта времени застоя при отсутсвия столкновения.
    /// </summary>
    public void UnStaying()
    {
        if (_Counting)
        {
            _Counting = false;
            StopCoroutine(TimerStayCounting());
            _StayTimer = 0.0f;
        }
    }

    /// <summary>
    /// Сопрограмма возврата значений скорости при движении NPC назад.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetSpeed()
    {
        while (_HittedToWall)
        {
            _DistanceFromStart = Vector3.Distance(transform.position, _StartPointBrake);
            if (_DistanceFromStart >= 1.2f)
            {
                _CarControl._FullTorqueOverAllWheels = _StartSpeed;
                _HittedToWall = false;
            }
            yield return null;
        }
        _Coroutined = false;
        yield return null;
    }

    /// <summary>
    /// Сопрограмма подсчёта времени столкновения.
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerStayCounting()
    {
        while (_Counting)
        {
            _StayTimer += Time.deltaTime;
            if (_StayTimer >= _TimeForСheck)
            {
                _StartPointBrake = transform.position;
                HittedToWall();
                _Counting = false;
            }
            yield return null;
        }
        yield return null;
    }
}