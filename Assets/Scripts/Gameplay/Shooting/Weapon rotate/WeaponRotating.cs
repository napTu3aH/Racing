using UnityEngine;
using System.Collections;

public class WeaponRotating : MonoBehaviour
{
    private static WeaponRotating _WeaponRotating;
    public static WeaponRotating Instance
    {
        get
        {
            if (_WeaponRotating != null)
            {
                return _WeaponRotating;
            }
            else
            {
                _WeaponRotating = new GameObject("_WeaponRotating", typeof(WeaponRotating)).GetComponent<WeaponRotating>();
                _WeaponRotating.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _WeaponRotating;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (_WeaponRotating == null)
        {
            _WeaponRotating = this;
        }
    }

    internal void SearchAndMissingTarget(WeaponRotate _WR)
    {
        if (!_WR._Target)
        {
            _WR._Target = WeaponTargeting.Instance.SearchTarget(_WR._Car, _WR._Radius);
            if (_WR._Target)
            {
                _WR._TargetedHitBox = _WR._Target.GetComponent<HitBox>();
                if (_WR._Car._Player) CameraTargeting.Instance.SetTarget(_WR._Target, _WR._TargetedHitBox._CarInfo);
            }
        }
        else
        {
            _WR._DistanceForTarget = DistanceCounting.Instance._Distance(_WR.transform.position, _WR._Target.position);
            if (_WR._DistanceForTarget > _WR._Radius || _WR._TargetedHitBox._HitBoxHealth <= 0.0f)
            {
                _WR._Target = null;
                _WR._TargetedHitBox = null;
                if (_WR._Car._Player) CameraTargeting.Instance.ClearTarget();
            }
        }
    }

    internal void RotateWeapon(WeaponRotate _WP, Transform _weaponTransform, Collider _weaponCollider, float _speedRotate, Vector3 _rotateAxis)
    {
        if (_weaponTransform)
        {
            Vector3 _Direction;

            if (_WP._Target)
            {
                _Direction = _WP._Target.position;
            }
            else
            {
                _Direction = _WP._Empty.position;
            }
            Debugger.Instance.Line(_weaponTransform.position, _Direction, Color.red);

            _weaponTransform.LookAt(_Direction);

            _WP._LocalEuler.x = _weaponTransform.localEulerAngles.x * _rotateAxis.x;
            _WP._LocalEuler.y = _weaponTransform.localEulerAngles.y * _rotateAxis.y;
            _WP._LocalEuler.z = _weaponTransform.localEulerAngles.z * _rotateAxis.z;

            _weaponTransform.localEulerAngles = _WP._LocalEuler;

            if (_weaponCollider)
            {
                _weaponCollider.transform.localEulerAngles = _weaponTransform.localEulerAngles;
            }
        }
    }
}
