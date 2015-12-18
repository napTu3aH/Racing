using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс, предназначенный для вращения оружия.
/// </summary>
public class WeaponRotate : MonoBehaviour {

    [SerializeField] internal ShootScript _ShootingScript;
    [SerializeField] internal Transform _Target;
    [SerializeField] internal Weapon[] _Weapons;
    [SerializeField] internal float _Radius;

    internal CarInfo _Car;
    [SerializeField] internal float _DistanceForTarget, _DistanceFromPlayer;    
    internal Transform _PlayerTransform;

    [HideInInspector] public HitBox _TargetedHitBox;

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
        StartCoroutine(WeaponRotating());
    }
	

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
            _DistanceForTarget = Vector3.Distance(transform.position, _Target.position);
            if (_DistanceForTarget > _Radius || _TargetedHitBox._HitBoxHealth <= 0.0f)
            {
                _Target = null;
                _TargetedHitBox = null;
            }
        }
    }

    IEnumerator DistanceToPlayer()
    {
        while (_Car._isAlive)
        {
            if (_PlayerTransform)
            {
                _DistanceFromPlayer = Vector3.Distance(transform.position, _PlayerTransform.position);
                _DistanceFromPlayer = 1.0f - (Mathf.Clamp(_DistanceFromPlayer, 0, _Radius) / _Radius);
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
            while (_Car._isAlive)
            {
                foreach (Weapon _wp in _Weapons)
                {
                    if (_wp._HitBox._HitBoxHealth > 0.0f)
                    {
                        SearchAndMissingTarget();
                        RotateWeapon(_wp._WeaponTransform, _wp._WeaponCollider, _wp._Quat, _wp._SpeedRotate, _wp._RotatingAxis.x, _wp._RotatingAxis.y, _wp._RotatingAxis.z);
                        if (_wp._Tower._TowerTransform)
                        {
                            if (_wp._Tower._HitBox._HitBoxHealth > 0.0f)
                            {
                                RotateWeapon(_wp._Tower._TowerTransform, _wp._Tower._TowerCollider, _wp._Tower._Quat, _wp._Tower._SpeedRotate, _wp._Tower._RotatingAxis.x, _wp._Tower._RotatingAxis.y, _wp._Tower._RotatingAxis.z);
                            }
                        }

                        float _volume = 1.0f - (Mathf.Clamp(_DistanceForTarget, 0, _Radius) / _Radius);
                        if (_Car._Player) _ShootingScript.Shoot(_wp._Muzzle, _wp._Damage, _wp._TimeBetweenShot, _wp._Clip, _volume);
                        else _ShootingScript.Shoot(_wp._Muzzle, _wp._Damage, _wp._TimeBetweenShot, _wp._Clip, _DistanceFromPlayer);
                    }
                }
                yield return null;
            }
        }
        yield return null;

    }
    #endregion

    #region Rotate weapon logic
    /// <summary>
    /// Метод, вращающий оружие. Необходимо указать Transform самого оружия, Collider оружия, Quaternion оружия и оси, по-которым будет осуществляться вращение.
    /// </summary>
    void RotateWeapon(Transform _weaponTransform, Collider _weaponCollider, Quaternion _quat, float _speedRotate, float _x, float _y, float _z)
    {
        if (_weaponTransform)
        {
            Vector3 _RelativePos;
            if (_Target)
            {
                _RelativePos = _Target.position - new Vector3(0, 2.0f, 0.0f);
            }
            else
            {
                _RelativePos = _weaponTransform.position + Vector3.forward;
            }
            //float step = 1.0f * Time.deltaTime;
            //Vector3 _newDir = Vector3.RotateTowards(_weaponTransform.forward, _RelativePos, step, 0.0f);
            Debug.DrawLine(_weaponTransform.position, _RelativePos, Color.red);
            //_quat = Quaternion.LookRotation(_newDir);
            //_weaponTransform.rotation = _quat;
            Vector3 _LocalTarget = transform.InverseTransformPoint(_RelativePos);
            Debug.DrawLine(_weaponTransform.position, _LocalTarget, Color.magenta);
            _quat = Quaternion.Slerp(_quat, Quaternion.LookRotation(_LocalTarget), _speedRotate * Time.deltaTime);
            _weaponTransform.localRotation = Quaternion.Euler(_x * _quat.eulerAngles.x, _y * _quat.eulerAngles.y, _z * _quat.eulerAngles.z);
            if (_weaponCollider)
            {
                _weaponCollider.transform.localRotation = _weaponTransform.localRotation;
            }
        }
    }
    #endregion

    public void ChangeTarget(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            HitBox _hitBox = _col.GetComponent<HitBox>();
            if (_hitBox)
            {
                if (_hitBox._HitBoxHealth > 0.0f)
                {
                    _Target = _col.transform;
                    _TargetedHitBox = _col.GetComponent<HitBox>();
                }                
            }
            
        }       
    }

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
    public Vector3 _RotatingAxis;
    [Range(1.0f, 5.0f)] public float _SpeedRotate;
    [HideInInspector] public Quaternion _Quat;
}