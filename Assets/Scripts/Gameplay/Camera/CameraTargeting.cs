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
    [SerializeField] internal TweenTransform _BarTop, _BarBotton;
    [SerializeField] internal bool _Start;
    [SerializeField] internal float _Speed = 0.5f, _AngleClamp;

    bool _Visibling, _Return, _AngleCounting, _Targeting;
    float _Angle;
    Vector3 _CurrentPosition, _LocalEuler;
    Quaternion _Quat, _Zero;
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
        _Camera = Camera.main.transform.parent;
        _Zero = _Camera.localRotation;
        if (!_Visibling)
        {
            _Visibling = true;
            StartCoroutine(TargetVisibling());
        }
    }

    internal void SetTarget(Transform _target, CarInfo _carInfo)
    {
        _Target = _target;
        _CarInfo = _carInfo;
    }

    internal void ClearTarget()
    {
        _Start = false;
        _Target = null;
        _CarInfo = null;
    }

    void TargetBar(bool _barred)
    {
        if (_barred)
        {
            _BarTop.PlayForward(); _BarBotton.PlayForward();
        }
        else
        {
            _BarTop.PlayReverse(); _BarBotton.PlayReverse();
        }
    }

    IEnumerator TargetVisibling()
    {
        bool _barred = false;
        while (_Visibling)
        {
            if (_CarInfo)
            {
                if (!_barred && _CarInfo._Visibled)
                {
                    _barred = true;
                    TargetBar(_barred);
                    if (!_Targeting && !_AngleCounting)
                    {
                        _AngleCounting = true;
                        _Targeting = true;
                        StartCoroutine(AngleCounting());
                        StartCoroutine(Targeting());
                    }
                }
                else
                if (_barred && !_CarInfo._Visibled)
                {
                    _barred = false;
                    if (_Targeting && _AngleCounting && !_Return)
                    {
                        _AngleCounting = false;
                        _Targeting = false;
                        _Return = true;
                        StartCoroutine(ReturnBackRotation());
                    }
                    TargetBar(_barred);
                }
            }
            else
                if (_barred)
            {
                _barred = false;
                if (_Targeting && _AngleCounting && !_Return)
                {
                    _AngleCounting = false;
                    _Targeting = false;
                    _Return = true;
                    StartCoroutine(ReturnBackRotation());
                }
                TargetBar(_barred);
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator AngleCounting()
    {
        while (_AngleCounting)
        {
            if (_Target)
            {
                Vector3 targetDir = _Target.position - _Camera.parent.position;
                Vector3 forward = _Camera.parent.forward;
                Debugger.Instance.Line(_Camera.position, _Target.position, Color.red);
                _Angle = Vector3.Angle(targetDir, forward);
            }            
            yield return null;
        }
        yield return null;
    }

    IEnumerator Targeting()
    {
        while (_Targeting)
        {
            if (_Angle < _AngleClamp)
            {
                if(_Target) _CurrentPosition = (_Target.position - _Camera.position) * 0.25f;
                Debug.DrawRay(_Camera.position, _CurrentPosition, Color.red);
                _CurrentPosition.y = 0.0f;
                _Quat = Quaternion.LookRotation(_CurrentPosition);
                _Camera.rotation = Quaternion.Slerp(_Camera.rotation, _Quat, Time.deltaTime * _Speed);
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator ReturnBackRotation()
    {
        float _timer = 0.0f;
        while (_Return)
        {
            _timer += Time.deltaTime * _Speed;
            _Camera.localRotation = Quaternion.Slerp(_Camera.localRotation, _Zero, _timer);
            if (_timer > 1.0f)
            {
                _timer = 1.0f;
                _Return = false;
            }
            yield return null;
        }
        yield return null;
    }
}
