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

    [SerializeField] internal ParticlesSystemHitting _ParticlesSystemScript;

    CarInfo _InCarInfo;
    float _Time;
    bool player; // true - player, false - NPC

    void Awake()
    {
        //Init();
    }
    public void Init()
    {
        _WeaponRotateScript = GetComponent<WeaponRotate>();
        _InCarInfo = GetComponent<CarInfo>();
        if (transform.CompareTag("Player"))
        {
            player = true;
        }
        _ParticlesSystemScript = GetComponent<ParticlesSystemHitting>();
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
    void Shooting(float _damage, float _timeBetweenShot, AudioClip _shootClip, float _clipVolume)
    {
        if (_Time < _timeBetweenShot)
        {
            _Time += Time.deltaTime;

            if (_Time >= _timeBetweenShot)
            {
                Ray _ray = new Ray(_Barrel.position, _Barrel.forward);
                RaycastHit _hit;

                _ParticlesSystemScript._Muzzels[0].Play();
                if(player) AudioController.Instance.PlayOneShot(_shootClip, 0.25f);
                else AudioController.Instance.PlayOneShot(_shootClip, 0.25f * _clipVolume);
                if (Physics.Raycast(_ray, out _hit, _DistanceForShooting))
                {
                    if (!_TargetedHitBox || _hit.collider.name != _TargetedHitBox.name)
                    {
                        _TargetedHitBox = _hit.collider.GetComponent<HitBox>();
                    } else
                    {
                        _TargetedHitBox.Hitted(_damage, _InCarInfo);
                        AudioController.Instance.PlayOneShot(_ParticlesSystemScript._HitShootSound[Random.Range(0, _ParticlesSystemScript._HitShootSound.Length)], 0.5f * _clipVolume);
                        _ParticlesSystemScript.ShootHit(_hit.point, Quaternion.LookRotation(_hit.normal, Vector3.up));
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
    public void Shoot(float _damage, float _timeBetweenShot, AudioClip _shootClip, float _clipVolume)
    {
        if (player)
        {
            if (CarUserControl.Instance._shooting)
            {
                Shooting(_damage, _timeBetweenShot, _shootClip, _clipVolume);
            }
        }
        else
        {
            if (_WeaponRotateScript._TargetedHitBox)
                Shooting(_damage, _timeBetweenShot, _shootClip, _clipVolume);
        }
        
    }
}
