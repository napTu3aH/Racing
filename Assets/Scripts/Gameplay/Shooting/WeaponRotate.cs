using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс, предназначенный для вращения оружия.
/// </summary>
[RequireComponent(typeof(ShootScript))]
public class WeaponRotate : MonoBehaviour {

    [SerializeField] internal ShootScript _ShootingScript;
    [SerializeField] internal Transform _Target;
    [SerializeField] internal Weapon[] _Weapons;
    [SerializeField] internal float _Radius;
    internal List<Transform> _CarsTransform;
    internal bool _Targeted;
    internal CarController _PlayerCar;
    internal float _DistanceForTarget;
    
    [HideInInspector] public HitBox _TargetedHitBox;
                                            
    void Awake ()
    {
        Init();
	}
	
    void Init()
    {
        _ShootingScript = GetComponent<ShootScript>();
        if (this.CompareTag("PlayerLogic"))
        {
            _PlayerCar = CarUserControl.Instance.m_Car;
        }
        else
        {
            _PlayerCar = GetComponent<CarController>();
        }

        if (_Weapons.Length != 0)
        {
            for (int i = 0; i < _Weapons.Length; i++)
            {
                _Weapons[i]._HitBox = _Weapons[i]._WeaponCollider.GetComponent<HitBox>();
                _Weapons[i]._Tower._HitBox = _Weapons[i]._Tower._TowerCollider.GetComponent<HitBox>();
                _Weapons[i]._WeaponTransform = transform.SearchChildWithName(_Weapons[i]._Name);
                _Weapons[i]._Tower._TowerTransform = transform.SearchChildWithName(_Weapons[i]._Tower._Name);   
            }
        }
        
        _ShootingScript.Init();
    }

	void Update ()
    {
        SearchTarget();
        MissingTargeting();
        WeaponRotating(); 
	}

    void SearchTarget()
    {
        if (!_Targeted)
        {
            if (_CarsTransform == null)
            {
                _CarsTransform = SpawnPlayers.Instance._CarsOnScene;
            }
                        
            for (int i = 0; i < _CarsTransform.Count; i++)
            {
                if (_CarsTransform[i] != transform)
                {
                    if (Vector3.Distance(transform.position, _CarsTransform[i].position) < _Radius)
                    {
                        CarInfo _info = _CarsTransform[i].GetComponent<CarInfo>();
                        for (int j = 0; j < _info._HitBoxs.Length; j++)
                        {
                            if (_info._HitBoxs[j]._HitBoxHealth > 0.0f)
                            {
                                _Target = _info._HitBoxs[j].transform;
                                _TargetedHitBox = _info._HitBoxs[j];
                                _Targeted = true;
                            }
                        }
                    }
                }                
            }
        }
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
            if (_DistanceForTarget > _Radius)
            {
                _Target = null;
                _TargetedHitBox = null;
                _Targeted = false;
            }
            if (_TargetedHitBox._HitBoxHealth <= 0.0f)
            {
                _Target = null;
                _TargetedHitBox = null;
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
        if (_Weapons.Length > 0)
        {
            foreach (Weapon _wp in _Weapons)
            {
                if (_wp._HitBox._HitBoxHealth > 0.0f)
                {
                    _ShootingScript.Shoot(_wp._Damage, _wp._TimeBetweenShot);
                    RotateWeapon(_wp._WeaponTransform, _wp._WeaponCollider, _wp._Quat, _wp._SpeedRotate, 1, 1, 0);
                    if (_wp._Tower._TowerTransform)
                    {
                        if (_wp._Tower._HitBox._HitBoxHealth > 0.0f)
                        {
                            RotateWeapon(_wp._Tower._TowerTransform, _wp._Tower._TowerCollider, _wp._Tower._Quat, _wp._Tower._SpeedRotate, 0, 1, 1);
                        }
                    }
                }
                
            }
        }
        

    }
    #endregion

    #region Rotate weapon logic
    /// <summary>
    /// Метод, вращающий оружие. Необходимо указать Transform самого оружия, Collider оружия, Quaternion оружия и оси, по-которым будет осуществляться вращение.
    /// </summary>
    void RotateWeapon(Transform _weaponTransform, Collider _weaponCollider, Quaternion _quat, float _speedRotate, int _x, int _y, int _z)
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
            _quat = Quaternion.Slerp(_quat, Quaternion.LookRotation(_RelativePos), _speedRotate * Time.deltaTime);
            _weaponTransform.rotation = Quaternion.Euler(_x * _quat.eulerAngles.x, _y * _quat.eulerAngles.y, _z * _quat.eulerAngles.z);
            if (_weaponCollider)
            {
                _weaponCollider.transform.rotation = _weaponTransform.rotation;
            }
        }
    }
    #endregion

    /*void OnTriggerStay(Collider _col)
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
    }*/

}

[Serializable]
public class Weapon
{
    public string _Name;
    public Transform _WeaponTransform;
    public Collider _WeaponCollider;
    public HitBox _HitBox;
    public float _TimeBetweenShot = 0.1f;
    public float _Damage = 1.0f;
    [Range(1.0f, 5.0f)] public float _SpeedRotate;
    [HideInInspector] public Quaternion _Quat;
    public Tower _Tower;
}

[Serializable]
public class Tower
{
    public string _Name;
    public Transform _TowerTransform;
    public Collider _TowerCollider;
    public HitBox _HitBox;
    [Range(1.0f, 5.0f)] public float _SpeedRotate;
    [HideInInspector] public Quaternion _Quat;
}