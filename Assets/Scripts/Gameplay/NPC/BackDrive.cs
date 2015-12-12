using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class BackDrive : MonoBehaviour {
	
	CarController _CarControl;
    CarAiController _AILogic;

    [SerializeField] internal float _StayTimer;
    [SerializeField] internal float _Distance = 1.1f;
    [SerializeField] internal Transform[] _Dots;
    float _StartSpeed;
    float _TimeForСheck;
    float _DistanceFromStart;

    Vector3 _StartPointBrake;

	bool _HittedToWall, _Coroutined, _Counting;

	void Awake () 
	{
        Init();
	}

    void Init()
    {
        _CarControl = GetComponent<CarController>();
        _AILogic = GetComponent<CarAiController>();
        _Dots = transform.Find("Rays").GetComponentsInChildren<Transform>();
        _StartSpeed = _CarControl._FullTorqueOverAllWheels;
        _TimeForСheck = Random.Range(1.5f, 1.6f);
        StartCoroutine(Raying());
    }



    /// <summary>
    /// Метод запуска движения NPC назад при длительном столкновении.
    /// </summary>
    public void HittedToWall()
    {
        _HittedToWall = true;
        _StayTimer = 0.0f;        
        _CarControl._FullTorqueOverAllWheels = -2500f;
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
    void Staying()
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
    void UnStaying()
    {
        if (_Counting)
        {
            _Counting = false;
            StopCoroutine(TimerStayCounting());
            _StayTimer = 0.0f;
        }
    }


    IEnumerator Raying()
    {
        RaycastHit _hit;
        Vector3 _direction = Vector3.zero;

        Ray[] _Ray = new Ray[_Dots.Length];

        while (true)
        {
            for (int i = 0; i < _Ray.Length; i++)
            {
                _Ray[i].origin = _Dots[i].position; _Ray[i].direction = _Dots[i].forward;
                _direction = _Ray[i].direction * _Distance;
                
                if (Physics.Raycast(_Ray[i].origin, _Ray[i].direction, out _hit, _Distance))
                {
                    if (_hit.collider.CompareTag("Walls") || _hit.collider.CompareTag("HitBox"))
                    {
                        Staying();
                        Debug.DrawRay(_Ray[i].origin, _direction, Color.red);
                    }
                }
            }
            yield return null;
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

    IEnumerator Steering()
    {
        _AILogic._SteerFactor = -1;
        while (_CarControl._CurrentTorque < 0.0f)
        {
            yield return null;
        }
        _AILogic._SteerFactor = 1;
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