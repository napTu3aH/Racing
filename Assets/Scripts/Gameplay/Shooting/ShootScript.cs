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

    [SerializeField] internal ParticlesSystem _ParticlesSystemScript;

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
        _ParticlesSystemScript = GetComponent<ParticlesSystem>();
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
    void Shooting(float _damage, float _timeBetweenShot)
    {
        if (_Time < _timeBetweenShot)
        {
            _Time += Time.deltaTime;
            if (_Time >= _timeBetweenShot)
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
                        _TargetedHitBox.Hitted(_damage);
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
    public void Shoot(float _damage, float _timeBetweenShot)
    {
        if (player)
        {
            if (CarUserControl.Instance._shooting)
            {
                Shooting(_damage, _timeBetweenShot);
            }
        }
        else
        {
            Shooting(_damage, _timeBetweenShot);
        }
        
    }
}
