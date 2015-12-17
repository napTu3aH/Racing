using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class BackDrive : MonoBehaviour {
	
	CarController _CarControl;
    CarAiController _AILogic;

    [SerializeField] internal float _StayTimer;
    [SerializeField] internal float _DistanceForward = 1.1f, _DistanceBack = 1.1f;
    [SerializeField] internal Transform[] _Dots;    // 0, 1, 2 - forward, 3,4 - back
    float _StartSpeed;
    float _TimeForСheck;
    float _DistanceFromStart;

    Vector3 _StartPointBrake;

	bool _HittedToWall, _Coroutined, _Counting, _Steering, _Forward;

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
    void Staying(bool _forward)
    {
        _Forward = _forward;
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
        float _distance = 0.0f;
        Ray[] _Ray = new Ray[_Dots.Length]; // 0, 1, 2 - forward, 3,4 - back

        while (true)
        {
            for (int i = 0; i < _Ray.Length; i++)
            {
                _Ray[i].origin = _Dots[i].position; _Ray[i].direction = _Dots[i].forward;
                if (i <= 2) _distance = _DistanceForward;
                else _distance = _DistanceBack;

                _direction = _Ray[i].direction * _distance;

                if (Physics.Raycast(_Ray[i].origin, _Ray[i].direction, out _hit, _distance))
                {
                    if (_hit.collider.CompareTag("Walls") || _hit.collider.CompareTag("HitBox"))
                    {
                        if (i <= 2)
                        {
                            Staying(false);
                        }
                        else
                        {
                            Staying(true);
                        }                        
                        Debugger.Instance.DrawRay(_Ray[i].origin, _direction, Color.red);
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
            if (_Forward && _CarControl._FullTorqueOverAllWheels != 2500)
            {
                _CarControl._FullTorqueOverAllWheels = 2500f;
                _AILogic._SteerFactor = 1;
            }
            else
            if(!_Forward && _CarControl._FullTorqueOverAllWheels != -2500)
            {
                _CarControl._FullTorqueOverAllWheels = -2500f;
                _AILogic._SteerFactor = -1;
            }

            _DistanceFromStart = Vector3.Distance(transform.position, _StartPointBrake);
            if (_DistanceFromStart >= 1.2f)
            {
                _CarControl._FullTorqueOverAllWheels = _StartSpeed;
                _HittedToWall = false;
            }
            yield return null;
        }
        _AILogic._SteerFactor = 1;
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
        _Steering = true;
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