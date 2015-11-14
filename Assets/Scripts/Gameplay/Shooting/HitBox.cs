using UnityEngine;
using System.Collections;

/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal float _ArmorFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
   // [SerializeField] internal float _DamageFactor = 0.01f;
    internal Collider _Collider;
    void Awake()
    {
        _CarInfo = transform.root.GetComponent<CarInfo>();
        _Collider = GetComponent<Collider>();
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
    }

    /// <summary>
    /// Передача урона игроку.
    /// </summary>
    public void Hitted(float _damage)
    {
        Damage(_damage);

        if (_CarInfo._Health <= 0.0f && _CarInfo._isAlive)
        {
            _CarInfo.DiePlayer();
        }
    }

    void Damage(float _damage)
    {
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
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
        _CarInfo._Car.TopSpeed = _CarInfo._TopSpeed *(_CarInfo._Health / 100.0f);
        Debugger.Instance.Log("Damaged " + transform.root.tag + " in " + transform.name + " component: " + _tmp + " Health: "+ _CarInfo._Health+"%");
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            _col.GetComponent<HitBox>().Hitted(_CarInfo._CarSpeed);     
        }
    }
}
