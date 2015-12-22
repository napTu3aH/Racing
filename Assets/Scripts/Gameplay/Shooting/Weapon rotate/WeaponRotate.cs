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
    internal Vector3 _LocalEuler;
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
            StartCoroutine(Rotating());
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
    IEnumerator Rotating()
    {
        float _volume = 1.0f;
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
                            WeaponRotating.Instance.SearchAndMissingTarget(this);
                            WeaponRotating.Instance.RotateWeapon(this, _Weapons[i]._WeaponTransform, _Weapons[i]._WeaponCollider, _Weapons[i]._SpeedRotate, _Weapons[i]._RotatingAxis);
                            if (_Weapons[i]._Tower != null)
                            {
                                if (_Weapons[i]._Tower._HitBox._HitBoxHealth > 0.0f)
                                {
                                    WeaponRotating.Instance.RotateWeapon(this, _Weapons[i]._Tower._TowerTransform, _Weapons[i]._Tower._TowerCollider, _Weapons[i]._Tower._SpeedRotate, _Weapons[i]._Tower._RotatingAxis);
                                }
                            }

                            _volume = 1.0f - (Mathf.Clamp(_DistanceForTarget, 0, _Radius) / _Radius);
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

    internal void SetTarget(Transform _target, HitBox _hitBox)
    {
        _Target = _target;
        _TargetedHitBox = _hitBox;
        if (_Car._Player) CameraTargeting.Instance.SetTarget(_Target, _TargetedHitBox._CarInfo);
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