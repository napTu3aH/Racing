using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс, отвечающий за выстрелы орудий.
/// </summary>
[RequireComponent(typeof(WeaponRotate))]
public class ShootScript : MonoBehaviour
{
    [Header("Weapon rotate")]
    [SerializeField] internal WeaponRotate _WeaponRotateScript;

    [Header("Transforms")]
    [SerializeField] internal Transform _Weapon;
    [SerializeField] internal Transform _Barrel;

    [Header("Target")]
    [SerializeField] internal HitBox _TargetedHitBox;
    [SerializeField] internal float _DistanceForShooting;


    float _Time;
    internal bool _player; // true - player, false - NPC
    internal CarInfo _CarInfo;
    internal HitBox _TmpHitBox;

    void Awake()
    {
        //Init();
    }
    public void Init()
    {
        _CarInfo = GetComponent<CarInfo>();
        _WeaponRotateScript = GetComponent<WeaponRotate>();
        _DistanceForShooting = _WeaponRotateScript._Radius;
        if (transform.CompareTag("Player"))
        {
            _player = true;
        }
        try
        {
            _Weapon = transform.SearchChildWithTag("Weapon");
            _Barrel = _Weapon.SearchChildWithTag("Barrel");
        }
        catch
        {
            Debug.Log("Weapon don't finded!");
        }
    }

    /// <summary>
    /// Метод, описывающий логику выстрела.
    /// </summary>
    void Shooting(ParticleSystem _muzzle, float _damage, float _timeBetweenShot, AudioClip _clip, float _volume)
    {
        if (_Time < _timeBetweenShot)
        {
            _Time += Time.deltaTime;
            if (_Time >= _timeBetweenShot)
            {
                WeaponShooting.Instance.Shoot(this, _clip, _volume, _muzzle,_damage);
                _Time = 0.0f;
            }
        }
        
    }
    /// <summary>
    /// Метод, отвечающий за выстрелы орудий.
    /// </summary>
    public void Shoot(ParticleSystem _muzzle, float _damage, float _timeBetweenShot, AudioClip _clip, float _volume)
    {
        if (_player)
        {
            if (CarUserControl.Instance._shooting)
            {
                Shooting(_muzzle, _damage, _timeBetweenShot, _clip, _volume);
            }
        }
        else
        {
            if (_WeaponRotateScript._TargetedHitBox)
                Shooting(_muzzle, _damage, _timeBetweenShot, _clip, _volume);
        }
        
    }
}
