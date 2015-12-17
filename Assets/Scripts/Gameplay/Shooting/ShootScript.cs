using UnityEngine;
using System.Collections;
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
    bool player; // true - player, false - NPC
    CarInfo _CarInfo;

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
            player = true;
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

    /*void Update()
    {
        Shoot();
    }*/
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
                Ray _ray = new Ray(_Barrel.position, _Barrel.forward);
                RaycastHit _hit;
                if (player) AudioController.Instance.PlayOneShot(_clip, 0.25f);
                else AudioController.Instance.PlayOneShot(_clip, 0.25f * _volume);
                _muzzle.Play();
                if (Physics.Raycast(_ray, out _hit, _DistanceForShooting))
                {
                    if (!_TargetedHitBox || _hit.collider.name != _TargetedHitBox.name)
                    {
                        if (!_CarInfo._HitBoxs.Contains(_hit.collider.GetComponent<HitBox>()))
                        {
                            _TargetedHitBox = _hit.collider.GetComponent<HitBox>();
                            if (_TargetedHitBox)
                            {
                                _WeaponRotateScript._Target = _TargetedHitBox.transform;
                            }
                        }                        
                        
                    } else
                    {
                        _TargetedHitBox.Hitted(_damage, _CarInfo);
                        AudioController.Instance.PlayOneShot(ParticlesHitting.Instance._HitShootSound[Random.Range(0, ParticlesHitting.Instance._HitShootSound.Length)], 0.5f * _volume);
                        ParticlesHitting.Instance.ShootHit(_hit.point, Quaternion.LookRotation(_hit.normal, Vector3.up), _TargetedHitBox._CarInfo._Visibled);
                    }
                    Debugger.Instance.Line(_ray.origin, _hit.point);
                }
                _Time = 0.0f;
            }
        }
        
    }
    /// <summary>
    /// Метод, отвечающий за выстрелы орудий.
    /// </summary>
    public void Shoot(ParticleSystem _muzzle, float _damage, float _timeBetweenShot, AudioClip _clip, float _volume)
    {
        if (player)
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
