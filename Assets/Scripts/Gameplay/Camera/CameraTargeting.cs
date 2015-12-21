using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class CameraTargeting : MonoBehaviour
{
    private static CameraTargeting _CameraTargeting;
    public static CameraTargeting Instance
    {
        get
        {
            if (_CameraTargeting != null)
            {
                return _CameraTargeting;
            }
            else
            {
                _CameraTargeting = new GameObject("_CameraTargeting", typeof(CameraTargeting)).GetComponent<CameraTargeting>();
                _CameraTargeting.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _CameraTargeting;
            }
        }
    }

    [SerializeField] internal Transform _Camera;
    [SerializeField] internal Transform _Target;
    [SerializeField] internal bool _Start;
    [SerializeField] internal float _Speed = 0.5f, _Angle;

    [SerializeField] internal Vector3 _Axis;
    Vector3 _CurrentPosition, _LocalEuler;
    Quaternion _Quat;
    CarInfo _CarInfo;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (_CameraTargeting == null)
        {
            _CameraTargeting = this;
        }

        if (!_Camera) SetCamera();
    }

    internal void SetCamera()
    {        
        _Camera = Camera.main.transform;
    }

    internal void SetTarget(Transform _target, CarInfo _carInfo)
    {
        _Target = _target;
        _CarInfo = _carInfo;
        if (!_Start)
        {
            _Start = true;
            StartCoroutine(AngleCounting());
            StartCoroutine(Targeting());
        }
    }

    internal void ClearTarget()
    {
        _Target = null;
        _CarInfo = null;
    }


    IEnumerator AngleCounting()
    {
        while (_Start)
        {
            Vector3 targetDir = _Target.position - _Camera.parent.position;
            Vector3 forward = _Camera.parent.forward;
            Debug.DrawLine(_Camera.position, _Target.position, Color.red);
            _Angle = Vector3.Angle(targetDir, forward);
            yield return null;
        }
        yield return null;
    }

    IEnumerator Targeting()
    {
        float _y = 0.0f;
        while (_Target)
        {
            if (_Angle < 70.0f)
            {
                _CurrentPosition = (_Target.position - _Camera.position) * 0.25f;
                Debug.DrawRay(_Camera.position, _CurrentPosition, Color.red);
                _CurrentPosition.y = 0.0f;
                _Quat = Quaternion.LookRotation(_CurrentPosition);
                _Camera.rotation = Quaternion.Slerp(_Camera.rotation, _Quat, Time.deltaTime * _Speed);
            }
            else
            {
                _Camera.rotation = Quaternion.Slerp(_Camera.rotation, Quaternion.identity, Time.deltaTime * _Speed);
            }
            yield return null;
        }
        _Start = false;
        yield return null;
    }
}
