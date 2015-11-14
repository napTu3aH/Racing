using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс, предназначенный для вращения оружия.
/// </summary>
[RequireComponent(typeof(ShootScript))]
public class WeaponRotate : MonoBehaviour {
    
    [SerializeField] [Range(1.0f, 5.0f)] internal float _SpeedRotate;
    [SerializeField] internal ShootScript _ShootingScript;
    [SerializeField] internal List<Transform> _WeaponTransform; // 0 - x, 1 - y, 2 - z
    [SerializeField] internal float[] _Clamping; // 0 - x, 1 - y, 2 - z
    [SerializeField] internal List<BoxCollider> _WeaponCollider;
    [SerializeField] internal Transform _Target, _TargetSelf;
    
    Quaternion _QuatZ, _QuatX;
    internal bool _Targeted;
    internal CarController _PlayerCar;
    internal float _DistanceForTarget;
    [HideInInspector] public HitBox _TargetedHitBox;
    [HideInInspector] public SphereCollider _Sphere;
                                            
    void Awake ()
    {
        Init();
	}
	
    void Init()
    {
        _Sphere = GetComponent<SphereCollider>();
        _ShootingScript = GetComponent<ShootScript>();
        if (this.CompareTag("PlayerLogic"))
        {
            _PlayerCar = CarUserControl.Instance.m_Car;
        }
        else
        {
            _PlayerCar = GetComponent<CarController>();
        }
        _ShootingScript.Init();
    }

	void Update ()
    {
        WeaponRotating();
        MissingTargeting();    
	}
    #region Missing of target
    /// <summary>
    /// Метод потери цели.
    /// </summary>
    void MissingTargeting()
    {
        if (_Targeted && _Target)
        {
            _DistanceForTarget = Vector3.Distance(_PlayerCar.transform.position, _Target.position);
            if (_DistanceForTarget > _Sphere.radius)
            {
                _Target = null;
                _Targeted = false;
            }
            if (_TargetedHitBox._HitBoxHealth <= 0.0f)
            {
                _Target = null;
                _Targeted = false;
            }
        }
        else
        {
            _Targeted = false;
        }
        
    }
    #endregion

    #region Weapon rotating
    /// <summary>
    /// Метод, в котором описываются данные для вращения оружия.
    /// </summary>
    void WeaponRotating()
    {

        #region Rotation AsixX
        RotateWeapon(_WeaponTransform[0], _WeaponCollider[0], _QuatX, 1, 1, 0);
        #endregion

        #region Rotation AsixZ
        RotateWeapon(_WeaponTransform[2], _WeaponCollider[2], _QuatZ, 0, 1, 1);
        #endregion


    }
    #endregion

    #region Rotate weapon logic
    /// <summary>
    /// Метод, вращающий оружие. Необходимо указать Transform самого оружия, Collider оружия, Quaternion оружия и оси, по-которым будет осуществляться вращение.
    /// </summary>
    void RotateWeapon(Transform _weaponTransform, Collider _weaponCollider, Quaternion _quat, int _x, int _y, int _z)
    {
        if (_weaponTransform)
        {
            Vector3 _RelativePos;
            if (_Target)
            {
                _RelativePos = _Target.position - _weaponTransform.position;
            }
            else
            {
                _RelativePos = transform.forward;
            }
            _quat = Quaternion.Slerp(_quat, Quaternion.LookRotation(_RelativePos), _SpeedRotate * Time.deltaTime);
            _weaponTransform.rotation = Quaternion.Euler(_x * _quat.eulerAngles.x, _y * _quat.eulerAngles.y, _z * _quat.eulerAngles.z);
            if (_weaponCollider)
            {
                _weaponCollider.transform.rotation = _weaponTransform.rotation;
            }
        }
    }
    #endregion

    void OnTriggerStay(Collider _col)
    {
        if (!_Targeted)
        {
            if (_col.CompareTag("HitBox") && _col.GetComponent<HitBox>()._HitBoxHealth > 0.0f)
            {
                _Target = _col.transform;
                _TargetedHitBox = _col.GetComponent<HitBox>();
                _Targeted = true;
            }
        }
    }

}
