using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс, предназначенный для вращения оружия.
/// </summary>
public class WeaponRotate : MonoBehaviour {

    [SerializeField] internal ShootScript _ShootingScript;
    [SerializeField] internal Transform _Target, _Empty;
    [SerializeField] internal Weapon[] _Weapons;
    [SerializeField] internal float _Radius;    
    [SerializeField] internal float _DistanceForTarget, _DistanceFromPlayer;

    internal float _volumeToPlayer;
    internal CarInfo _Car;
    internal HitBox _TargetedHitBox;
    internal Transform _PlayerTransform;
    Vector3 _LocalEuler;
    bool _Started;

    internal void Init()
    {      
        _Car = GetComponent<CarInfo>();
        _ShootingScript = GetComponent<ShootScript>();
        if (!_Car._Player)
        {
            GameObject _player = GameObject.FindWithTag("Player");
            if (_player)
            {
                _PlayerTransform = _player.GetComponent<Transform>();
            }
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

        if (!_Car._Player)
        {
            StartCoroutine(DistanceToPlayer());
        }        
    }

    void FixedUpdate()
    {
        if (_Car._Visibled && !_Started)
        {
            _Started = true;
            StartCoroutine(WeaponRotating());
        }
    }

    IEnumerator DistanceToPlayer()
    {
        while (_Car._isAlive)
        {
            if (_PlayerTransform)
            {
                _DistanceFromPlayer = DistanceCounting.Instance._Distance(transform.position, _PlayerTransform.position);
                _volumeToPlayer = 1.0f - (Mathf.Clamp(_DistanceFromPlayer, 0, _Radius) / _Radius);
            }
            yield return null;
        }
        yield return null;
    }

    #region Weapon rotating
    /// <summary>
    /// Метод, в котором описываются данные для вращения оружия.
    /// </summary>
    IEnumerator WeaponRotating()
    {
        if (_Weapons.Length > 0)
        {
            while (_Car._Visibled)
            {
                if (_DistanceFromPlayer < _Radius)
                {
                    for (int i = 0; i < _Weapons.Length; i++)
                    {
                        if (_Weapons[i]._HitBox._HitBoxHealth > 0.0f)
                        {
                            SearchAndMissingTarget();
                            RotateWeapon(_Weapons[i]._WeaponTransform, _Weapons[i]._WeaponCollider, _Weapons[i]._SpeedRotate, _Weapons[i]._RotatingAxis);
                            if (_Weapons[i]._Tower != null)
                            {
                                if (_Weapons[i]._Tower._HitBox._HitBoxHealth > 0.0f)
                                {
                                    RotateWeapon(_Weapons[i]._Tower._TowerTransform, _Weapons[i]._Tower._TowerCollider, _Weapons[i]._Tower._SpeedRotate, _Weapons[i]._Tower._RotatingAxis);
                                }
                            }

                            float _volume = 1.0f - (Mathf.Clamp(_DistanceForTarget, 0, _Radius) / _Radius);
                            if (_Car._Player) _ShootingScript.Shoot(_Weapons[i]._Muzzle, _Weapons[i]._Damage, _Weapons[i]._TimeBetweenShot, _Weapons[i]._Clip, _volume);
                            else _ShootingScript.Shoot(_Weapons[i]._Muzzle, _Weapons[i]._Damage, _Weapons[i]._TimeBetweenShot, _Weapons[i]._Clip, _volumeToPlayer);
                        }
                    }
                }                
                yield return null;
            }
        }
        _Started = false;
        yield return null;

    }
    #endregion

    void SearchAndMissingTarget()
    {
        if (!_Target)
        {
            _Target = WeaponTargeting.Instance.SearchTarget(_Car, _Radius);
            if (_Target)
            {
                _TargetedHitBox = _Target.GetComponent<HitBox>();
            }
        }
        else
        {
            _DistanceForTarget = DistanceCounting.Instance._Distance(transform.position, _Target.position);
            if (_DistanceForTarget > _Radius || _TargetedHitBox._HitBoxHealth <= 0.0f)
            {
                _Target = null;
                _TargetedHitBox = null;
            }
        }
    }

    #region Rotate weapon logic
    /// <summary>
    /// Метод, вращающий оружие. Необходимо указать Transform самого оружия, Collider оружия, Quaternion оружия и оси, по-которым будет осуществляться вращение.
    /// </summary
    void RotateWeapon(Transform _weaponTransform, Collider _weaponCollider, float _speedRotate, Vector3 _rotateAxis)
    {
        if (_weaponTransform)
        {
            Vector3 _Direction;

            if (_Target)
            {
                _Direction = _Target.position;
            }
            else
            {
                _Direction = _Empty.position;
            }            
            Debugger.Instance.Line(_weaponTransform.position, _Direction, Color.red);

            _weaponTransform.LookAt(_Direction);

            _LocalEuler.x = _weaponTransform.localEulerAngles.x * _rotateAxis.x;
            _LocalEuler.y = _weaponTransform.localEulerAngles.y * _rotateAxis.y;
            _LocalEuler.z = _weaponTransform.localEulerAngles.z * _rotateAxis.z;

            _weaponTransform.localEulerAngles = _LocalEuler;

            if (_weaponCollider)
            {
                _weaponCollider.transform.localEulerAngles = _weaponTransform.localEulerAngles;
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
    public AudioClip _Clip;
    public ParticleSystem _Muzzle;
    public float _TimeBetweenShot = 0.1f;
    public float _Damage = 1.0f;
    public Vector3 _RotatingAxis;
    [Range(0.0f, 5.0f)] public float _SpeedRotate;
    public Tower _Tower;
}

[Serializable]
public class Tower
{
    public string _Name;
    public Transform _TowerTransform;
    public Collider _TowerCollider;
    public HitBox _HitBox;
    public Vector3 _RotatingAxis;
    [Range(0.0f, 5.0f)] public float _SpeedRotate;
}