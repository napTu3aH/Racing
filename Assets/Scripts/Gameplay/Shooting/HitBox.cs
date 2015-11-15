using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    CarController _Car;
    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal float _ArmorFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
    [SerializeField] internal float _UnDamagedFactor = 0.0f;
    internal Collider _Collider;

    void Awake()
    {
        _CarInfo = transform.root.GetComponent<CarInfo>();
        _Car = transform.root.GetComponent<CarController>();
        _Collider = GetComponent<Collider>();
        Counting();
    }

    /// <summary>
    /// Передача урона игроку.
    /// </summary>
    public void Hitted(float _damage)
    {
        Damage(_damage);
        Counting();
        
        if (_CarInfo._Health <= 0.0f && _CarInfo._isAlive)
        {
            _CarInfo.DiePlayer();
        }
    }

    void Counting()
    {
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
        _UnDamagedFactor = _ArmorFactor - 1;

        if (transform.name == "Left")
        {
            _Car._FactorLeft = _UnDamagedFactor;
        }
        if (transform.name == "Right")
        {
            _Car._FactorRight = _UnDamagedFactor;
        }

    }

    void Damage(float _damage)
    {
        float _tmp = _damage / _ArmorFactor;
        
        if (_HitBoxHealth > _tmp)
        {
            _HitBoxHealth -= _tmp;
            if (_HitBoxHealth < _tmp)
            {
                _HitBoxHealth = 0.0f;
                _ArmorFactor = 1.0f;
            } 
        }

        _CarInfo._CurrentHealth -= _tmp;
        _CarInfo._Health = _CarInfo._CurrentHealth / _CarInfo._PercentHealthFactor;
        _Car.TopSpeed = _CarInfo._TopSpeed *(_CarInfo._Health / 100.0f);
        Debugger.Instance.Log("Damaged " + transform.root.tag + " in " + transform.name + " component: " + _tmp + " Health: "+ _CarInfo._Health+"%");
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            if (_CarInfo._Player)
            {
                GameSettings.Instance.Vibrate();
            }            
            _col.GetComponent<HitBox>().Hitted(_CarInfo._CarSpeed);     
        }
    }
}
