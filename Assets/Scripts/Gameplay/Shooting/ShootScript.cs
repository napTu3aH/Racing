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

    [Header("Characteristics")]
    [SerializeField] internal float _TimeBetweenShot = 0.1f;
    [SerializeField] internal float _Damage = 1.0f;
    [SerializeField] internal float _DistanceForShooting;


    float _Time;
    bool player; // true - player, false - NPC

    void Awake()
    {
        //Init();
    }
    public void Init()
    {
        _WeaponRotateScript = GetComponent<WeaponRotate>();
        _DistanceForShooting = _WeaponRotateScript._Sphere.radius;
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

    void Update()
    {
        Shoot();
    }
    /// <summary>
    /// Метод, описывающий логику выстрела.
    /// </summary>
    void Shooting()
    {
        if (_Time < _TimeBetweenShot)
        {
            _Time += Time.deltaTime;
            if (_Time >= _TimeBetweenShot)
            {
                Ray _ray = new Ray(_Barrel.position, _Barrel.forward);
                RaycastHit _hit;

                if (Physics.Raycast(_ray, out _hit, _DistanceForShooting))
                {
                    if (!_TargetedHitBox || _hit.collider.name != _TargetedHitBox.name)
                    {
                        _TargetedHitBox = _hit.collider.GetComponent<HitBox>();
                    } else
                    {
                        _TargetedHitBox.Hitted(_Damage);
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
    void Shoot()
    {
        if (player)
        {
            if (CarUserControl.Instance._shooting)
            {
                Shooting();
            }
        }
        else
        {
            Shooting();
        }
        
    }
}
