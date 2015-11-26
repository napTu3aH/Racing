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

    [SerializeField] internal Transform _Target;
    [SerializeField] internal Weapon[] _Weapons;
    [SerializeField] internal ShootScript _ShootingScript;

    int _TargetIndex;
    float[] _Distance;
    float _MinDistance;
    CarInfo _InfoTarget, _Car;
    internal bool _Targeted;
    internal CarController _PlayerCar;
    internal float _TimeRotate, _DistanceToPlayer;
    internal Transform _PlayerTransform;
    [HideInInspector] public List<Transform> _Cars;
    [HideInInspector] public HitBox _TargetedHitBox;
    

    void Awake ()
    {
        Init();
	}
	
    void Init()
    {
        _Cars = new List<Transform>();
        _Car = GetComponent<CarInfo>();
        _PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _PlayerCar = GetComponent<CarController>();
        _ShootingScript = GetComponent<ShootScript>();

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
        MissingOfTarget();
        DistanceToPlayer();
        WeaponRotating();
    }

    void DistanceToPlayer()
    {
        if (!_Car._Player && _PlayerTransform)
        {
            _DistanceToPlayer = Vector3.Distance(transform.position, _PlayerTransform.position);
            _DistanceToPlayer = 1.0f - (Mathf.Clamp(_DistanceToPlayer, 0, 100) / 100.0f);
        }
    }

    #region Search and Targeting
    /// <summary>
    /// Метод поиска цели.
    /// </summary>
    void SearchTarget()
    {
        if (!_Targeted)
        {
            _MinDistance = Mathf.Infinity;
            if (_Cars.Count == 0)
            {
                _Cars = SpawnPlayers.Instance.PlayersTransforms();
                if (_Cars.Count > 0)
                {
                    _Distance = new float[_Cars.Count];
                }
            }
            else
            if (_Cars.Count > 0)
            {
                for (int i = 0; i < _Cars.Count; i++)
                {
                    _Distance[i] = Vector3.Distance(transform.position, _Cars[i].position);
                    if (_Distance[i] <= 75.0f)
                    {
                        Tartgeting(i);
                    }
                }
            }
        }       
    }

    /// <summary>
    /// Метод нацеливания.
    /// </summary>
    /// <param name="i">Номер машины.</param>
    void Tartgeting(int i)
    {
        if (_MinDistance >= _Distance[i] && _Cars[i] != transform)
        {
            _MinDistance = _Distance[i];

            _InfoTarget = _Cars[i].GetComponent<CarInfo>();
            for (int j = 0; j < _InfoTarget._HitBoxs.Length; j++)
            {
                if (_InfoTarget._HitBoxs[j]._HitBoxHealth > 0.0f)
                {
                    _TargetedHitBox = _InfoTarget._HitBoxs[j];
                    _Target = _TargetedHitBox.transform;
                    _TargetIndex = i;
                    _Targeted = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Метод потери цели.
    /// </summary>
    void MissingOfTarget()
    {
        if (_Targeted)
        {
            _Distance[_TargetIndex] = Vector3.Distance(transform.position, _Cars[_TargetIndex].position);
            if (_Distance[_TargetIndex] > 75.0f || _TargetedHitBox._HitBoxHealth <= 0.0f || _Target == null)
            {
                _Target = null;
                _TargetedHitBox = null;
                _Targeted = false;
            }
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
                    if (_Car._Player) _ShootingScript.Shoot(_wp._Damage, _wp._TimeBetweenShot, _wp._ShootClip, 1.0f - (Mathf.Clamp(_Distance[_TargetIndex], 0, 100) / 100.0f));
                    else _ShootingScript.Shoot(_wp._Damage, _wp._TimeBetweenShot, _wp._ShootClip, _DistanceToPlayer);
                    _ShootingScript._DistanceForShooting = _wp._DistanceForShooting;
                    RotateWeapon(_wp._WeaponTransform, _wp._WeaponCollider, _wp._Quat, _wp._SpeedRotate, 1, 1, 0, _wp._Quat.eulerAngles);
                    if (_wp._Tower._TowerTransform)
                    {
                        if (_wp._Tower._HitBox._HitBoxHealth > 0.0f)
                        {
                            RotateWeapon(_wp._Tower._TowerTransform, _wp._Tower._TowerCollider, _wp._Tower._Quat, _wp._Tower._SpeedRotate, 0, 1, 1, _wp._Quat.eulerAngles);
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
    void RotateWeapon(Transform _weaponTransform, Collider _weaponCollider, Quaternion _quat, float _speedRotate, int _x, int _y, int _z, Vector3 _vect)
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
            if (_TimeRotate < 1.0f)
            {
                _TimeRotate += _speedRotate * Time.deltaTime;
            }
            
            _quat = Quaternion.Slerp(_quat, Quaternion.LookRotation(_RelativePos), _speedRotate);
            //_vect = Vector3.Lerp(_vect, _RelativePos, _TimeRotate);
            //_weaponTransform.eulerAngles = _vect;
            _weaponTransform.rotation = Quaternion.Euler(_x * _quat.eulerAngles.x, _y * _quat.eulerAngles.y, _z * _quat.eulerAngles.z);
            if (_weaponCollider)
            {
                _weaponCollider.transform.rotation = _weaponTransform.rotation;
            }
        }
    }
    #endregion

}

[Serializable]
public class Weapon
{
    public string _Name;
    public Transform _WeaponTransform;
    public Collider _WeaponCollider;
    public HitBox _HitBox;
    public AudioClip _ShootClip;
    public float _Damage = 1.0f;
    public float _SpeedRotate;
    public float _TimeBetweenShot = 0.1f;
    public float _DistanceForShooting;    
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
    public float _SpeedRotate;
    [HideInInspector] public Quaternion _Quat;
}