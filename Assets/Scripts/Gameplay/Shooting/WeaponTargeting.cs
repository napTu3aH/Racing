using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

public class WeaponTargeting : MonoBehaviour
{
    private static WeaponTargeting _WeaponTargeting;
    public static WeaponTargeting Instance
    {
        get
        {
            if (_WeaponTargeting != null)
            {
                return _WeaponTargeting;
            }
            else
            {
                _WeaponTargeting = new GameObject("_WeaponTargeting", typeof(WeaponTargeting)).GetComponent<WeaponTargeting>();
                _WeaponTargeting.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _WeaponTargeting;
            }
        }
    }

    [SerializeField] internal List<CarInfo> _Cars;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!_WeaponTargeting)
        {
            _WeaponTargeting = this;
        }
        _Cars = new List<CarInfo>();
    }

    internal void AddTarget(CarInfo _car)
    {
        if (!_Cars.Contains(_car))
        {
            _Cars.Add(_car);
        }
        else
        {
            Debugger.Instance.Log("This car added!");
        }        
    }

    internal void RemoveTarget(CarInfo _car)
    {
        if (_Cars.Contains(_car))
        {
            _Cars.Remove(_car);
        }
        else
        {
            Debugger.Instance.Log("This car removed!");
        }
    }

    internal Transform SearchTarget(CarInfo _car, float _radius)
    {
        for (int i = 0; i < _Cars.Count; i++)
        {
            if (_Cars[i] != _car)
            {
                if (Vector3.Distance(_car.transform.position, _Cars[i].transform.position) < _radius)
                {
                    CarInfo _info = _Cars[i].GetComponent<CarInfo>();
                    for (int j = 0; j < _info._HitBoxs.Count; j++)
                    {
                        if (_info._HitBoxs[j]._HitBoxHealth > 0.0f)
                        {
                            return _info._HitBoxs[j].transform;
                        }
                    }
                }
            }
        }
        return null;
    }
}
