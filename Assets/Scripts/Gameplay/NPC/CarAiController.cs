using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class CarAiController : MonoBehaviour
{
    [SerializeField] internal int _ID;
    [SerializeField] internal Transform _CurrentPoint;

    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal Rigidbody _Rigidbody;
    [SerializeField] internal CarController _CarController;

    [SerializeField] internal float _SteerSensitivity = 0.05f;
    [SerializeField] internal float _AccelSensitivity = 0.04f;
    [SerializeField] internal float _BrakeSensitivity = 1f;
    [SerializeField] internal float _LateralWanderSpeed = 0.1f;
    [SerializeField] internal float _LateralWanderDistance = 3f;  
    [SerializeField] internal float _AccelWanderSpeed = 0.1f;
    [Range(0, 1)] [SerializeField] internal float _AccelWanderAmount = 0.1f;

    internal float _TimeUpdateFactor, _Distance, _SteerFactor;
    float _RandomPerlin;
    bool _Moving, _UpdatedPath;

    void Start()
    {
        Init();
    }

    internal void Init()
    {
        _CarController = GetComponent<CarController>();
        _Rigidbody = GetComponent<Rigidbody>();
        _CarInfo = GetComponent<CarInfo>();
        _RandomPerlin = Random.value * 100;
        _SteerFactor = 1;
    }

    internal void SetTarget(Transform _target)
    {
        Init();
        _CurrentPoint = _target;
        if (!_Moving && _CurrentPoint)
        {
            _Moving = true;
            StartCoroutine(AIMoving());
            StartCoroutine(NPC_Updater());
        }
    }


    IEnumerator AIMoving()
    {
        while (_CurrentPoint)
        {
            Vector3 _ForwardDirection = transform.forward;
            Vector3 _OffsetTargetPos = _CurrentPoint.position;

            if (_Rigidbody.velocity.magnitude > _CarController.MaxSpeed * 0.1f)
            {
                _ForwardDirection = _Rigidbody.velocity;
            }

            float _DesiredSpeed = _CarController.MaxSpeed;
            _OffsetTargetPos += _CurrentPoint.right * (Mathf.PerlinNoise(Time.time * _LateralWanderSpeed, _RandomPerlin) * 2 - 1) * _LateralWanderDistance;
            // использование различной чувствительности в зависимости от того, ускоряется ли, или тормозит AI
            float _AccelBrakeSensitivity = (_DesiredSpeed < _CarController.CurrentSpeed) ? _BrakeSensitivity : _AccelSensitivity;            
            // добиваемся желаемой скорости.
            float _Accel = Mathf.Clamp((_DesiredSpeed - _CarController.CurrentSpeed) * _AccelBrakeSensitivity, -1, 1);
            _Accel *= (1 - _AccelWanderAmount) + (Mathf.PerlinNoise(Time.time * _AccelWanderSpeed, _RandomPerlin) * _AccelWanderAmount);
            // вычисляем локальную позицию цели
            Vector3 _LocalTarget = transform.InverseTransformPoint(_OffsetTargetPos);
            // получаем угол до цели
            float _TargetAngle = Mathf.Atan2(_LocalTarget.x, _LocalTarget.z) * Mathf.Rad2Deg;
            // получаем рулевой угол до цели
            float _Steer = Mathf.Clamp(_TargetAngle * _SteerSensitivity, -1, 1) * Mathf.Sign(_CarController.CurrentSpeed) * _SteerFactor;
            // передаем данные в CarController.
            _CarController.Move(_Steer, _Accel, _Accel, 0f);
            yield return null;
        }
        _Moving = false;
        yield return null;
    }

    IEnumerator NPC_Updater()
    {
        while (_CarInfo._isAlive && _CurrentPoint)
        {
            if (!_UpdatedPath)
            {
                _UpdatedPath = true;
                Invoke("UpdatePath", _TimeUpdateFactor);
            }
            yield return null;
        }
        yield return null;
    }

    void UpdatePath()
    {
        if (_CurrentPoint)
        {
            UpdateDistance();
            NPCCalculatePath.Instance.PathUpdate(this, _CurrentPoint);
        }
    }

    void UpdateDistance()
    {
        //NPCCalculatePath.Instance.DistaceUpdate(_ID);
        _Distance = Vector3.Distance(transform.position, _CurrentPoint.position);
        _TimeUpdateFactor = (_Distance / 100.0f);
        _UpdatedPath = false;
    }

}
