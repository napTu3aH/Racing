using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class WeaponShooting : MonoBehaviour
{
    private static WeaponShooting _WeaponShooting;
    public static WeaponShooting Instance
    {
        get
        {
            if (_WeaponShooting != null)
            {
                return _WeaponShooting;
            }
            else
            {
                _WeaponShooting = new GameObject("_WeaponShooting", typeof(WeaponShooting)).GetComponent<WeaponShooting>();
                _WeaponShooting.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _WeaponShooting;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (_WeaponShooting == null)
        {
            _WeaponShooting = this;
        }
    }

    internal void Shoot(ShootScript _SS, AudioClip _clip, float _volume, ParticleSystem _muzzle, float _damage)
    {
        Ray _ray = new Ray(_SS._Barrel.position, _SS._Barrel.forward);
        RaycastHit _hit;
        if (_SS._player) AudioController.Instance.PlayOneShot(_clip, 0.25f);
        else AudioController.Instance.PlayOneShot(_clip, 0.25f * _volume);
        _muzzle.Play();

        if (Physics.Raycast(_ray, out _hit, _SS._DistanceForShooting))
        {
            if (_hit.collider.CompareTag("HitBox"))
            {
                _SS._TmpHitBox = _hit.collider.GetComponent<HitBox>();
                if (_SS._TmpHitBox == null) _SS._TmpHitBox = _SS._WeaponRotateScript._TargetedHitBox;
                if (!_SS._CarInfo._HitBoxs.Contains(_SS._TmpHitBox))
                {
                    if (_SS._TargetedHitBox != _SS._TmpHitBox)
                    {
                        _SS._TargetedHitBox = _SS._TmpHitBox;
                        _SS._WeaponRotateScript.SetTarget(_SS._TargetedHitBox.transform, _SS._TargetedHitBox);
                    }
                    else
                    {
                        _SS._TargetedHitBox.Hitted(_damage, _SS._CarInfo);
                        AudioController.Instance.PlayOneShot(ParticlesHitting.Instance._HitShootSound[Random.Range(0, ParticlesHitting.Instance._HitShootSound.Length)], 0.5f * _volume);
                        ParticlesHitting.Instance.ShootHit(_hit.point, Quaternion.LookRotation(_hit.normal, Vector3.up), _SS._TargetedHitBox._CarInfo._Visibled);
                    }
                }
            }
            Debugger.Instance.Line(_ray.origin, _hit.point);
        }
    }
}
