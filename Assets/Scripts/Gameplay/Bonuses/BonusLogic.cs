using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
public class BonusLogic : MonoBehaviour
{
    [SerializeField] internal BonusEffect _Bonus;
    [SerializeField] internal float _FactorObject;
    [SerializeField] internal AudioClip _Clip;
    [SerializeField] internal float _GrowFactor;

    bool _Grow, _Uppded;
    float _Timer;
    Vector3 _ScaleFactor, _LittleScale;

    internal enum BonusEffect
    {
        Damage,
        Speed,
        Armor,
        Repair
    }

    /*void Awake()
    {
        StartCoroutine(_Rotate());
    }*/

    internal void Init(int _number, float _factor, AudioClip _clip)
    {
        _Bonus = (BonusEffect)_number;
        _FactorObject = _factor;
        _Clip = _clip;
        StartCoroutine(_Rotate());
    }


    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBox") && !_Uppded)
        {
            HitBox _box = _col.GetComponent<HitBox>();
            if (_box)
            {
                CarInfo _car = _box._CarInfo;
                GettingBonus(_car);
                _Uppded = true;
            }
        }
    }

    void GettingBonus(CarInfo _car)
    {
        switch (_Bonus)
        {
            case BonusEffect.Armor:
                break;

            case BonusEffect.Damage:
                break;

            case BonusEffect.Repair:
                foreach (HitBox _hitBox in _car._HitBoxs)
                {
                    _hitBox.Repair();
                }
                break;

            case BonusEffect.Speed:
                break;
        }
        AudioController.Instance.PlayOneShot(_Clip, 1.0f);
        Destroy(gameObject);
    }

    IEnumerator _Rotate()
    {
        _ScaleFactor = transform.localScale;
        _LittleScale = new Vector3(_ScaleFactor.x / 2.0f, _ScaleFactor.y / 2.0f, _ScaleFactor.z / 2.0f);

        while (true)
        {
            _Timer += Time.deltaTime * _GrowFactor;
            if (!_Grow)
            {
                transform.localScale = Vector3.Lerp(_ScaleFactor, _LittleScale, _Timer);
                if (_Timer >= 1.0f)
                {
                    _Timer = 0.0f;
                    _Grow = true;
                }
            }
            else
            if (_Grow)
            {
                transform.localScale = Vector3.Lerp(_LittleScale, _ScaleFactor, _Timer);
                if (_Timer >= 1.0f)
                {
                    _Timer = 0.0f;
                    _Grow = false;
                }
            }

            transform.Rotate(0.0f, 1.0f, 0.0f);
            yield return null;
        }        
    }
}
